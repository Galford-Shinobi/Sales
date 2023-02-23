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

        private async Task ChecksCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States = new List<State>()
                    {
                    new State()
                    {
                        Name = "Antioquia",
                        Cities = new List<City>() {
                        new City() { Name = "Medellín" },
                        new City() { Name = "Itagüí" },
                        new City() { Name = "Envigado" },
                        new City() { Name = "Bello" },
                        new City() { Name = "Rionegro" },
                    }
                    },
                    new State()
                    {
                    Name = "Bogotá",
                    Cities = new List<City>() {
                    new City() { Name = "Usaquen" },
                    new City() { Name = "Champinero" },
                    new City() { Name = "Santa fe" },
                    new City() { Name = "Useme" },
                    new City() { Name = "Bosa" },
                    }
                    },
                    }
                });
                _context.Countries.Add(new Country
                {
                    Name = "Estados Unidos",
                    States = new List<State>()
                    {
                    new State()
                    {
                    Name = "Florida",
                    Cities = new List<City>() {

                    new City() { Name = "Orlando" },
                    new City() { Name = "Miami" },
                    new City() { Name = "Tampa" },
                    new City() { Name = "Fort Lauderdale" },
                    new City() { Name = "Key West" },
                    }
                    },
                    new State()
                    {
                    Name = "Texas",
                    Cities = new List<City>() {
                    new City() { Name = "Houston" },
                    new City() { Name = "San Antonio" },
                    new City() { Name = "Dallas" },
                    new City() { Name = "Austin" },
                    new City() { Name = "El Paso" },
                    }
                    },
            }
                });
            }
            await _context.SaveChangesAsync();
        }



    }
}
