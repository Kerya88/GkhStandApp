using System.Net;
using System.Net.Mail;

namespace GkhStandApp.Services
{
    public class EmailService(ICryptoService cryptoService) : IEmailService
    {
        readonly ICryptoService _cryptoService = cryptoService;

        readonly string EmailServer = "mail.nic.ru";
        readonly int Port = 25;
        readonly string Login = "no-reply@gzhi-samara.ru";
        readonly string Password = "rD7s5QwM6BkFs";
        readonly bool UseSSL = true;

        public async Task SendVerificationCodeAsync(string email)
        {
            var code = GenerateVerificationCode();

            var message = new MailMessage(Login, email)
            {
                Subject = "Код проверки для прохождения опроса",
                Body = $"Ваш проверочный код: {code}. Не сообщайте этот код третьим лицам"
            };

            using (var client = new SmtpClient(EmailServer, Port))
            {
                client.Credentials = new NetworkCredential(Login, Password);
                client.EnableSsl = UseSSL;

                try
                {
                    client.Send(message);
                }
                catch
                {
                    client.EnableSsl = !UseSSL;

                    client.Send(message);
                }
            }
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            var code = new List<int>();
            for (var i = 0; i < 6; i++)
            {
                code.Add(random.Next(10));
            }

            return string.Join("", code);
        }

        private class StorageProxy(string email, string code, string hash)
        {
            public string Email { get; set; } = email;
            public string Code { get; set; } = code;
            public string Hash { get; set; } = hash;
            public DateTime ExpiredAt { get; set; } = DateTime.Now.AddMinutes(5);
        }
    }
}
