using Sales.Shared.Responses;

namespace Sales.API.Services
{
    public interface IApiService
    {
        Task<GenericResponse<object>> GetListAsync<T>(string servicePrefix, string controller);
    }
}
