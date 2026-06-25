// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Text;
using System.Threading.Tasks;
using ESports.Domain.Models; // Importação correta do MyUser
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace WebApp.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Modelo de suporte à página de confirmação do pedido de registo.
    /// Informa o utilizador sobre o envio do e-mail de ativação e gere o estado de simulação do fluxo.
    /// </summary>
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly IEmailSender _sender;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="RegisterConfirmationModel"/> com os serviços necessários.
        /// </summary>
        public RegisterConfirmationModel(UserManager<MyUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        /// <summary>
        /// O endereço de correio eletrónico associado ao registo pendente.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Define se a interface deve exibir um link de atalho para confirmação direta.
        /// Configurado rigidamente como falso para obrigar à simulação real via e-mail na consola.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        /// O URL gerado para a confirmação de e-mail (utilizado internamente se aplicável).
        /// </summary>
        public string? EmailConfirmationUrl { get; set; }

        /// <summary>
        /// Processa os dados recebidos via GET após a submissão do formulário de registo.
        /// </summary>
        /// <param name="email">O email do utilizador recém-criado.</param>
        /// <param name="returnUrl">O URL de retorno após a operação.</param>
        /// <returns>O resultado da página ou um redirecionamento adequado.</returns>
        public async Task<IActionResult> OnGetAsync(string email, string? returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            returnUrl ??= Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Regra do professor: Retorna erro limpo estruturado sem expor a stack trace da base de dados
                return StatusCode(500, $"Não foi possível carregar o utilizador com o e-mail '{email}'.");
            }

            Email = email;

            // Alteração de Engenharia: Definido para false para forçar o aluno a validar o e-mail real enviado para a consola
            DisplayConfirmAccountLink = false;

            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

            return Page();
        }
    }
}