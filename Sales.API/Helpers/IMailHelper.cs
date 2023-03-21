
using Sales.Shared.Responses;

namespace Sales.API.Helpers
{
    public interface IMailHelper
    {
        GenericResponse<object> SendMail(string toName, string toEmail, string subject, string body);
    }
}
