using Sales.Shared.DataBase;
using Sales.Shared.Entities;

namespace Sales.API.DataSeeding
{
    public class SeedDb
    {
        private readonly SalesDbContext _context;

        public SeedDb(SalesDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country { Name = "Colombia" });
                _context.Countries.Add(new Country { Name = "Perú" });
                _context.Countries.Add(new Country { Name = "México" });
                _context.Countries.Add(new Country { Name = "Alemania" });
                _context.Countries.Add(new Country { Name = "Croacia" });
                _context.Countries.Add(new Country { Name = "Finlandia" });
                _context.Countries.Add(new Country { Name = "Irlanda" });
                _context.Countries.Add(new Country { Name = "Luxemburgo" });

                await _context.SaveChangesAsync();
            }
        }
    }
}
