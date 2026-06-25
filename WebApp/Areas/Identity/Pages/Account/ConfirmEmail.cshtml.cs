// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESports.Domain.Models; // Importação correta do MyUser
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace WebApp.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Modelo encarregue de processar e validar o token de ativação de conta enviado por e-mail.
    /// Altera o estado do utilizador na base de dados para confirmado.
    /// </summary>
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<MyUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ConfirmEmailModel"/> injetando o gestor de utilizadores Identity.
        /// </summary>
        public ConfirmEmailModel(UserManager<MyUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Mensagem de estado temporária exibida na vista Razor para feedback visual do utilizador.
        /// </summary>
        [TempData]
        public string? StatusMessage { get; set; }

        /// <summary>
        /// Processa a validação do token quando o utilizador clica no link enviado no e-mail.
        /// </summary>
        /// <param name="userId">O identificador único (ID) do utilizador.</param>
        /// <param name="code">O token de segurança codificado em Base64Url.</param>
        /// <returns>A página de sucesso ou redirecionamento em caso de dados inválidos.</returns>
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Regra do professor: Mensagem limpa controlada em ambiente de produção
                return StatusCode(500, $"Não foi possível localizar o utilizador com o identificador '{userId}'.");
            }

            try
            {
                // Descodifica o código de validação recebido por parâmetro seguro
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user, code);

                // Mensagens de feedback localizadas e amigáveis para a interface em Português
                StatusMessage = result.Succeeded
                    ? "Obrigado por confirmar o seu e-mail. A sua conta encontra-se ativa."
                    : "Erro ao tentar confirmar o seu e-mail. O token poderá ter expirado.";
            }
            catch (Exception)
            {
                StatusMessage = "Ocorreu um erro interno ao processar a validação da conta.";
                return StatusCode(500, StatusMessage);
            }

            return Page();
        }
    }
}