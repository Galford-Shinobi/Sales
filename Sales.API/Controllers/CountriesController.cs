using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.Shared.Applications.Interfaces;
using Sales.Shared.DataBase;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    public class CountriesController : BaseApiController
    {
        private readonly SalesDbContext _salesDbContext;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesController(SalesDbContext salesDbContext, ICountriesRepository countriesRepository)
        {
            _salesDbContext = salesDbContext;
            _countriesRepository = countriesRepository;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                return Ok(await _countriesRepository.GetAllCountryAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
               "Error retrieving data from the database");
            }
            
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                var country = await _countriesRepository.GetOnlyCountryoAsync(id);
                //if (country is null)
                //{
                //    return NotFound();
                //}
                if (!country.IsSuccess)
                {
                    return NotFound(country.ErrorMessage);
                }
                return Ok(country.Result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
              "Error retrieving data from the database");
            }
            
        }

        [HttpPost]
        public async Task<ActionResult> Post(Country country)
        {
            try
            {
                _salesDbContext.Add(country);
                await _salesDbContext.SaveChangesAsync();
                return Ok(country);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
              "Error retrieving data from the database");
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Country country)
        {
            try
            {
                _salesDbContext.Update(country);
                await _salesDbContext.SaveChangesAsync();
                return Ok(country);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
             "Error retrieving data from the database");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var afectedRows = await _salesDbContext.Countries
               .Where(x => x.Id == id)
               .ExecuteDeleteAsync();

                if (afectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
            "Error retrieving data from the database");
            }
        }

    }
}
