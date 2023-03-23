using Azure;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Sales.Shared.Responses;

namespace Sales.API.Helpers.platform
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GenericResponse<object> SendMail(string toName, string toEmail, string subject, string body)
        {
            try
            {
                var from = _configuration["Mail:From"];
                var name = _configuration["Mail:Name"];
                var smtp = _configuration["Mail:Smtp"];
                var port = _configuration["Mail:Port"];
                var password = _configuration["Mail:Password"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(name, from));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port!), false);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new GenericResponse<object> { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new GenericResponse<object>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    Result = ex
                };
            }
        }

        public GenericResponse<object> SendMailAttachments(string toName,string to, string subject, string body, string FilePath)
        {
            try
            {
                var from = _configuration["Mail:From"];
                var name = _configuration["Mail:Name"];
                var smtp = _configuration["Mail:Smtp"];
                var port = _configuration["Mail:Port"];
                var sSLPort = _configuration["Mail:SSLPort"];
                var password = _configuration["Mail:Password"];
                var MailOrders = _configuration["Mail:MailOrders"];



                MimeMessage message = new MimeMessage();
                //message.From.Add(MailboxAddress.Parse(from));
                message.From.Add(new MailboxAddress(name, from));
                message.To.Add(new MailboxAddress(MailOrders, to));
                message.Cc.Add(new MailboxAddress("marcos.nava@comtecom.com.mx", "marcosnavaramirez@gmail.com"));
                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                bodyBuilder.Attachments.Add(@FilePath, new ContentType("application", "xlsx"));

                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port!), SecureSocketOptions.None);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return new GenericResponse<object> { IsSuccess = true };
            }
            catch (Exception exMail)
            {

                return new GenericResponse<object>
                {
                    IsSuccess = false,
                    ErrorMessage = exMail.Message,
                    Result = exMail
                };
            }
        }
    }
}
