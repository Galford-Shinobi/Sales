using Sales.Shared.Responses;

namespace Sales.API.Helpers
{
    public interface IOrdersHelper
    {
        Task<GenericResponse<object>> ProcessOrderAsync(string email, string remarks);
    }
}
