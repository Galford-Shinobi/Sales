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
            await ChecksCountriesAsync();
        }

        
        private async Task ChecksCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States = new List<State>
                    {
                        new State
                        {
                            Name = "Antioquia",
                            Cities = new List<City>
                            {
                                new City { Name = "Medellín" },
                                new City { Name = "Envigado" },
                                new City { Name = "Itagüí" }
                            }
                        },
                        new State
                        {
                            Name = "Bogotá",
                            Cities = new List<City>
                            {
                                new City { Name = "Bogotá" }
                            }
                        },
                        new State
                        {
                            Name = "Valle del Cauca",
                            Cities = new List<City>
                            {
                                new City { Name = "Calí" },
                                new City { Name = "Buenaventura" },
                                new City { Name = "Palmira" }
                            }
                        }
                    }
                });
                _context.Countries.Add(new Country
                {
                    Name = "USA",
                    States = new List<State>
                    {
                        new State
                        {
                            Name = "California",
                            Cities = new List<City>
                            {
                                new City { Name = "Los Angeles" },
                                new City { Name = "San Diego" },
                                new City { Name = "San Francisco" }
                            }
                        },
                        new State
                        {
                            Name = "Illinois",
                            Cities = new List<City>
                            {
                                new City { Name = "Chicago" },
                                new City { Name = "Springfield" }
                            }
                        }
                    }
                });
                await _context.SaveChangesAsync();
            }
        }

    }
}
