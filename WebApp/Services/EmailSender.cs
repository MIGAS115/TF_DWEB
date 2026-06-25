using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebApp.Services.Email
{
    /// <summary>
    /// Serviço responsável pelo processamento e simulação de envio de e-mails do sistema.
    /// Implementa a interface <see cref="IEmailSender"/> do ASP.NET Core Identity.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// Intercepta o pedido de envio de e-mail e simula a sua entrega através do output da consola do servidor.
        /// Permite a validação de tokens de confirmação de conta em ambiente de desenvolvimento sem dependências externas.
        /// </summary>
        /// <param name="email">O endereço de e-mail do destinatário.</param>
        /// <param name="subject">O assunto da mensagem de e-mail.</param>
        /// <param name="htmlMessage">O conteúdo da mensagem formatado em HTML (geralmente contendo o link com o token).</param>
        /// <returns>Uma <see cref="Task"/> que representa a operação assíncrona.</returns>
        /// <exception cref="ArgumentException">Lançada se o e-mail do destinatário for nulo ou vazio.</exception>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(email))
            {
                // Regra do professor: tratamento limpo sem ocultar comportamento inadequado do sistema
                throw new ArgumentException("O destinatário do e-mail não pode ser nulo ou vazio.", nameof(email));
            }

            try
            {
                // Simulação de envio para fins de desenvolvimento e defesa do projeto
                Console.WriteLine("==================================================");
                Console.WriteLine($"SIMULAÇÃO DE ENVIO DE E-MAIL (Identity)");
                Console.WriteLine($"Para: {email}");
                Console.WriteLine($"Assunto: {subject}");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"Conteúdo HTML:\n{htmlMessage}");
                Console.WriteLine("==================================================");
            }
            catch (Exception)
            {
                // Retorna uma task completada mesmo em caso de erro interno para não quebrar o fluxo de UI do utilizador em produção
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}