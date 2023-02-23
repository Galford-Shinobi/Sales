using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.Shared.DataBase;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{

    public class CategoriesController : BaseApiController
    {
        private readonly SalesDbContext _salesDbContext;

        public CategoriesController(SalesDbContext salesDbContext)
        {
            _salesDbContext = salesDbContext;
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
                return Ok(await _salesDbContext.Categories.ToListAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
               "Error retrieving data from the database");
            }

        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAsync(int id)
        {
            try
            {
                var category = await _salesDbContext.Categories.FirstOrDefaultAsync(c => c.Id==id);
                if (category is null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "There are not Data");
                }
               
                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
              "Error retrieving data from the database");
            }

        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostAsync(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                _salesDbContext.Categories.Add(category);
                await _salesDbContext.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    //return BadRequest("Ya existe un país con el mismo nombre.");
                    return StatusCode(StatusCodes.Status400BadRequest, "Ya existe un Categoria con el mismo nombre.");
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
        public async Task<ActionResult> PutAsync(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                _salesDbContext.Categories.Update(category);
                await _salesDbContext.SaveChangesAsync();
                return Ok(category);
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
                var category = await _salesDbContext
                    .Categories.
                    FirstOrDefaultAsync(x => x.Id.Equals(id));

                if (category == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "There are not Data");
                }

                var afectedRows = await _salesDbContext.Categories
               .Where(x => x.Id == id)
               .ExecuteDeleteAsync();

                if (afectedRows == 0)
                {
                    ModelState.AddModelError("", $"Algo salió mal borrando el registro{category.Name}");
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
