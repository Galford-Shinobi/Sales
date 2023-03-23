﻿using Azure;
using Microsoft.EntityFrameworkCore;
using Sales.API.Helpers;
using Sales.API.Services;
using Sales.Shared.DataBase;
using Sales.Shared.Entities;
using Sales.Shared.Enums;
using Sales.Shared.Responses;

namespace Sales.API.DataSeeding
{
    public class SeedDb
    {
        private readonly SalesDbContext _context;
        private readonly IApiService _apiService;
        private readonly IUserHelper _userHelper;

        public SeedDb(SalesDbContext context, IApiService apiService, IUserHelper userHelper)
        {
            _context = context;
            _apiService = apiService;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", UserType.Admin);
            await CheckUserAsync("911", "Draco", "Master", "dracomaster@yopmail.com", "555 666 7789", "Izanaki, Amaterasu", UserType.Admin);
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Deportes" });
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Tecnología " });
                _context.Categories.Add(new Category { Name = "Lenceria" });
                _context.Categories.Add(new Category { Name = "Erótica" });
                _context.Categories.Add(new Category { Name = "Comida" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Jugetes" });
                _context.Categories.Add(new Category { Name = "Mascotas" });
                _context.Categories.Add(new Category { Name = "Autos" });
                _context.Categories.Add(new Category { Name = "Cosmeticos" });
                _context.Categories.Add(new Category { Name = "Hogar" });
                _context.Categories.Add(new Category { Name = "Jardín" });
                _context.Categories.Add(new Category { Name = "Ferreteria" });
                _context.Categories.Add(new Category { Name = "Video Juegos" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {

                var city = await _context.Cities.FirstOrDefaultAsync(x => x.Name == "Higashikurume-shi");
                if (city == null)
                {
                    city = await _context.Cities.FirstOrDefaultAsync();
                }

                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType,
                };

                await _userHelper.AddUserAsync(user, "User123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }


       

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                GenericResponse<object> responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");
                if (responseCountries.IsSuccess)
                {
                    List<CountryResponse> countries = (List<CountryResponse>)responseCountries.Result!;
                    foreach (CountryResponse countryResponse in countries)
                    {
                        Country country = await _context.Countries!.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!)!;
                        if (country == null)
                        {
                            country = new() { Name = countryResponse.Name!, States = new List<State>() };
                            GenericResponse<object> responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");
                            if (responseStates.IsSuccess)
                            {
                                List<StateResponse> states = (List<StateResponse>)responseStates.Result!;
                                foreach (StateResponse stateResponse in states!)
                                {
                                    State state = country.States!.FirstOrDefault(s => s.Name == stateResponse.Name!)!;
                                    if (state == null)
                                    {
                                        state = new() { Name = stateResponse.Name!, Cities = new List<City>() };
                                        GenericResponse<object> responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");
                                        if (responseCities.IsSuccess)
                                        {
                                            List<CityResponse> cities = (List<CityResponse>)responseCities.Result!;
                                            foreach (CityResponse cityResponse in cities)
                                            {
                                                if (cityResponse.Name == "Mosfellsbær" || cityResponse.Name == "Șăulița")
                                                {
                                                    continue;
                                                }
                                                City city = state.Cities!.FirstOrDefault(c => c.Name == cityResponse.Name!)!;
                                                if (city == null)
                                                {
                                                    state.Cities.Add(new City() { Name = cityResponse.Name! });
                                                }
                                            }
                                        }
                                        if (state.CitiesNumber > 0)
                                        {
                                            country.States.Add(state);
                                        }
                                    }
                                }
                            }
                            if (country.StatesNumber > 0)
                            {
                                _context.Countries.Add(country);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }
        //}

        //private async Task AddCategoryAsync(string name)
        //{
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images", $"{name}.png");
        //    Guid imageId = await _blobHelper.UploadBlobAsync(path, "categories");
        //    _context.Categories.Add(new Category { Name = name, ImageId = imageId });
        //}

        //private async Task ChecksCountriesAsync()
        //{
        //    if (!_context.Countries.Any())
        //    {
        //        _context.Countries.Add(new Country
        //        {
        //            Name = "Colombia",
        //            States = new List<State>
        //            {
        //                new State
        //                {
        //                    Name = "Antioquia",
        //                    Cities = new List<City>
        //                    {
        //                        new City { Name = "Medellín" },
        //                        new City { Name = "Envigado" },
        //                        new City { Name = "Itagüí" }
        //                    }
        //                },
        //                new State
        //                {
        //                    Name = "Bogotá",
        //                    Cities = new List<City>
        //                    {
        //                        new City { Name = "Bogotá" }
        //                    }
        //                },
        //                new State
        //                {
        //                    Name = "Valle del Cauca",
        //                    Cities = new List<City>
        //                    {
        //                        new City { Name = "Calí" },
        //                        new City { Name = "Buenaventura" },
        //                        new City { Name = "Palmira" }
        //                    }
        //                }
        //            }
        //        });
        //        _context.Countries.Add(new Country
        //        {
        //            Name = "USA",
        //            States = new List<State>
        //            {
        //                new State
        //                {
        //                    Name = "California",
        //                    Cities = new List<City>
        //                    {
        //                        new City { Name = "Los Angeles" },
        //                        new City { Name = "San Diego" },
        //                        new City { Name = "San Francisco" }
        //                    }
        //                },
        //                new State
        //                {
        //                    Name = "Illinois",
        //                    Cities = new List<City>
        //                    {
        //                        new City { Name = "Chicago" },
        //                        new City { Name = "Springfield" }
        //                    }
        //                }
        //            }
        //        });
        //        await _context.SaveChangesAsync();
        //    }
        //}

    }
}
