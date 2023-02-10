using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Shared.Applications.Interfaces;

namespace Sales.Shared.Applications.Logic
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddScoped<ICountriesRepository, CountriesRepository>();
            //Services.AddScoped<IBodegaRepository, BodegaRepository>();
            //Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            //Services.AddScoped<IIvaRepository, IvaRepository>();
            //Services.AddScoped<IMedidumRepository, MedidumRepository>();
            //Services.AddScoped<IProductoRepository, ProductoRepository>();
            //Services.AddScoped<IRolRepository, RolRepository>();
            //Services.AddScoped<IUserFactoryRepository, UserFactoryRepository>();
            //Services.AddScoped<IProveedorRepository, ProveedorRepository>();
            //Services.AddScoped<IGenderRepository, GenderRepository>();
        }
    }
}
