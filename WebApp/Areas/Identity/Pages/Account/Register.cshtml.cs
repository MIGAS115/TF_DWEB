// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace WebApp.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Modelo de suporte à página de registo de novos utilizadores na plataforma de e-Sports.
    /// Gerencia a criação de instâncias de segurança IdentityUser e o envio do token de confirmação por e-mail.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="RegisterModel"/> com os serviços injetados pelo pipeline do sistema.
        /// </summary>
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Propriedade que encapsula os dados introduzidos no formulário de registo.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = default!;

        /// <summary>
        /// O URL de redirecionamento após a conclusão bem-sucedida do registo.
        /// </summary>
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Lista de esquemas de autenticação externa ativos (se aplicável).
        /// </summary>
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        /// <summary>
        /// Classe que define as regras de validação de dados para o formulário de registo.
        /// Utiliza a sintaxe tokenizada obrigatória para as mensagens de erro em pt-PT.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Endereço de correio eletrónico do novo utilizador.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório.")]
            [EmailAddress(ErrorMessage = "O {0} introduzido não é um endereço válido.")]
            [Display(Name = "Correio Eletrónico (Email)")]
            public string Email { get; set; } = default!;

            /// <summary>
            /// Palavra-passe definida para proteção da conta.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório.")]
            [StringLength(100, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")]
            public string Password { get; set; } = default!;

            /// <summary>
            /// Confirmação da palavra-passe para prevenir erros de digitação.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Palavra-passe")]
            [Compare("Password", ErrorMessage = "A palavra-passe e a sua confirmação não coincidem.")]
            public string? ConfirmPassword { get; set; }
        }

        /// <summary>
        /// Processa os pedidos HTTP GET iniciais da página de registo.
        /// </summary>
        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        /// <summary>
        /// Processa a submissão do formulário de registo via HTTP POST.
        /// Executa a criação do utilizador e despacha o e-mail com o token de ativação.
        /// </summary>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilizador criou uma nova conta com palavra-passe.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme)!;

                    // Envio real mapeado para a consola do nosso EmailSender customizado
                    await _emailSender.SendEmailAsync(Input.Email, "Confirme o seu e-mail - IPT eSports",
                        $"Por favor, confirme a sua conta na plataforma <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se chegámos aqui, algo falhou; reapresenta o formulário com os erros capturados
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao instanciar a classe de utilizador de segurança.");
                throw new InvalidOperationException($"Não foi possível criar uma instância de '{nameof(IdentityUser)}'. " +
                    $"Garanta que a classe não é abstrata e possui um construtor sem parâmetros.");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("O suporte padrão de interface requer um repositório de utilizadores com suporte para e-mail.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}