using Microsoft.EntityFrameworkCore;
using Sales.Shared.Applications.Interfaces;
using Sales.Shared.DataBase;
using Sales.Shared.Entities;
using Sales.Shared.Responses;

namespace Sales.Shared.Applications.Logic
{
    public class CountriesRepository : GenericRepositoryFactory<Country>, ICountriesRepository
    {
        private readonly SalesDbContext _salesDbContext;

        public CountriesRepository(SalesDbContext salesDbContext) : base(salesDbContext)
        {
            _salesDbContext = salesDbContext;
        }

        public async Task<GenericResponse<Country>> DeactivateCountrytoAsync(Country model)
        {
            try
            {
                var OnlyCountry = await _salesDbContext.Countries.FirstOrDefaultAsync(c => c.Id == model.Id);
               
                _salesDbContext.Countries.Remove(OnlyCountry);
                await SaveAllAsync();
                return new GenericResponse<Country> { IsSuccess = true, Result = OnlyCountry };

            }
            catch (Exception ex)
            {
                return new GenericResponse<Country> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<GenericResponse<Country>> DeleteCountrytoAsync(int id)
        {
            try
            {
                var country = await _salesDbContext.Countries.FirstOrDefaultAsync(c => c.Id == id);
                if (country == null)
                {
                    return new GenericResponse<Country> { IsSuccess = false, Message = "No hay Datos!" };
                }

                _salesDbContext.Countries.Remove(country);
                if (!await SaveAllAsync())
                {
                    return new GenericResponse<Country> { IsSuccess = false, Message = "La operacion no realizada!" };
                }

                return new GenericResponse<Country> { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Country> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<List<Country>> GetAllCountryAsync()
        {
            return await _salesDbContext.Countries.ToListAsync();
        }

        public async Task<GenericResponse<Country>> GetOnlyCountryoAsync(int id)
        {
            try
            {
                var Only = await _salesDbContext.Countries.FirstOrDefaultAsync(c => c.Id.Equals(id));
                if (Only == null)
                {
                    return new GenericResponse<Country> { IsSuccess = false, Message = "No hay Datos!" };
                }
               
                return new GenericResponse<Country> { IsSuccess = true, Result = Only };

            }
            catch (Exception ex)
            {
                return new GenericResponse<Country> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<GenericResponse<Country>> OnlyCountryoGetAsync(int id)
        {
            try
            {
                var OnlyConcepto = await _salesDbContext.Countries.FirstOrDefaultAsync(c => c.Id.Equals(id));
                if (OnlyConcepto == null)
                {
                    return new GenericResponse<Country> { IsSuccess = false, Message = "No hay Datos!" };
                }

                return new GenericResponse<Country> { IsSuccess = true, Result = OnlyConcepto };

            }
            catch (Exception ex)
            {
                return new GenericResponse<Country> { IsSuccess = false, Message = ex.Message };
            }
        }

        private async Task<bool> SaveAllAsync()
        {
            return await _salesDbContext.SaveChangesAsync() > 0;
        }
    }
}
