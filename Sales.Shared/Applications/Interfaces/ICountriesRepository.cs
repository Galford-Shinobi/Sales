using Sales.Shared.Entities;
using Sales.Shared.Responses;

namespace Sales.Shared.Applications.Interfaces
{
    public interface ICountriesRepository : IGenericRepositoryFactory<Country>
    {
        Task<List<Country>> GetAllCountryAsync();
        Task<List<Country>> GetFullCountryAsync();
        Task<GenericResponse<Country>> GetOnlyCountryoAsync(int id);
        Task<GenericResponse<Country>> OnlyCountryoGetAsync(int id);
        Task<GenericResponse<Country>> DeleteCountrytoAsync(int id);
        Task<GenericResponse<Country>> DeactivateCountrytoAsync(Country model);
    }
}
