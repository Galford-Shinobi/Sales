using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.Shared.DataBase;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    public class CountriesController : BaseApiController
    {
        private readonly SalesDbContext _salesDbContext;

        public CountriesController(SalesDbContext salesDbContext)
        {
            _salesDbContext = salesDbContext;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                return Ok(await _salesDbContext.Countries.ToListAsync());
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
                var country = await _salesDbContext.Countries.FirstOrDefaultAsync(x => x.Id == id);
                if (country is null)
                {
                    return NotFound();
                }

                return Ok(country);
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
