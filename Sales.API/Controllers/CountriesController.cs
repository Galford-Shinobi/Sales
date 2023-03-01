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
        //[ResponseCache(Duration = 20)]
        [ResponseCache(CacheProfileName = "PorDefecto20Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAsync()
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

        [HttpGet("full")]
        [ResponseCache(CacheProfileName = "PorDefecto20Segundos")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFullAsync()
        {
            return Ok(await _countriesRepository.GetFullCountryAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            try
            {
                var country = await _countriesRepository.GetOnlyCountryoAsync(id);
                
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
        [ProducesResponseType(201, Type = typeof(Country))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostAsync(Country country)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                _salesDbContext.Add(country);
                await _salesDbContext.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    //return BadRequest("Ya existe un país con el mismo nombre.");
                    return StatusCode(StatusCodes.Status400BadRequest, "Ya existe un país con el mismo nombre.");
                }
                return StatusCode(StatusCodes.Status400BadRequest, dbUpdateException.Message);
                //return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                //return BadRequest(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, exception.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(201, Type = typeof(Country))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PutAsync(Country country)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                _salesDbContext.Update(country);
                await _salesDbContext.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    //return BadRequest("Ya existe un país con el mismo nombre.");
                    return StatusCode(StatusCodes.Status400BadRequest, "Ya existe un país con el mismo nombre.");
                }
                return StatusCode(StatusCodes.Status400BadRequest, dbUpdateException.Message);
                //return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                //return BadRequest(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, exception.Message);
            }

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                var country = await _salesDbContext
                    .Countries.
                    FirstOrDefaultAsync(x => x.Id.Equals(id));

                if (country == null) 
                {
                    return NotFound();
                }

                var afectedRows = await _salesDbContext.Countries
               .Where(x => x.Id == id)
               .ExecuteDeleteAsync();

                if (afectedRows == 0)
                {
                    ModelState.AddModelError("", $"Algo salió mal borrando el registro{country.Name}");
                    return StatusCode(500, ModelState);
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
