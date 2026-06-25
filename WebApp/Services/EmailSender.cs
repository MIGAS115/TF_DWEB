using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace WebApp.Services.Email
{
    /// <summary>
    /// Serviço responsável pelo processamento e envio real de e-mails do sistema através do Mailtrap.
    /// Implementa a interface <see cref="IEmailSender"/> do ASP.NET Core Identity.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EmailSender"/> acedendo às configurações globais do sistema.
        /// </summary>
        /// <param name="configuration">Instância de configuração para extração segura de credenciais.</param>
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Envia de forma assíncrona o e-mail de confirmação através do servidor SMTP configurado.
        /// Protege as chaves de acesso prevenindo falhas críticas no browser em produção.
        /// </summary>
        /// <param name="email">O endereço de e-mail do destinatário.</param>
        /// <param name="subject">O assunto da mensagem.</param>
        /// <param name="htmlMessage">O conteúdo em formato HTML com o token de ativação.</param>
        /// <returns>Uma <see cref="Task"/> que representa a conclusão da operação.</returns>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("O destinatário do e-mail não pode ser nulo ou vazio.", nameof(email));
            }

            try
            {
                // Extração segura dos dados a partir do ficheiro de configuração (appsettings)
                var host = _configuration["Mailtrap:Host"] ?? "sandbox.smtp.mailtrap.io";
                var port = int.Parse(_configuration["Mailtrap:Port"] ?? "2525");
                var username = _configuration["Mailtrap:Username"];
                var password = _configuration["Mailtrap:Password"];

                // Configuração do cliente SMTP nativo do .NET
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                // Construção da mensagem estruturada
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("noreply@ipt-esports.pt", "IPT eSports Platform"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                // Envio real e assíncrono do e-mail
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception)
            {
                // Regra do professor: Nunca expor stack traces ou quebrar a UI por falhas em serviços externos
                // O fluxo termina sem rebentar a aplicação para o utilizador final
                await Task.CompletedTask;
            }
        }
    }
}