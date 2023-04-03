using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.Shared.DataBase;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/temporalSales")]
    public class TemporalSalesController : ControllerBase
    {
        private readonly SalesDbContext _context;

        public TemporalSalesController(SalesDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post(TemporalSaleDTO temporalSaleDTO)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == temporalSaleDTO.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity!.Name);
            if (user == null)
            {
                return NotFound();
            }

            var temporalSale = new TemporalSale
            {
                Product = product,
                Quantity = temporalSaleDTO.Quantity,
                Remarks = temporalSaleDTO.Remarks,
                User = user
            };

            try
            {
                _context.Add(temporalSale);
                await _context.SaveChangesAsync();
                return Ok(temporalSaleDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.TemporalSales
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .Where(x => x.User!.Email == User.Identity!.Name)
                .ToListAsync());
        }

        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCount()
        {
            return Ok(await _context.TemporalSales
                .Where(x => x.User!.Email == User.Identity!.Name)
                .SumAsync(x => x.Quantity));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get(int id)
        {
            return Ok(await _context.TemporalSales
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Put(TemporalSaleDTO temporalSaleDTO)
        {
            var currentTemporalSale = await _context.TemporalSales.FirstOrDefaultAsync(x => x.Id == temporalSaleDTO.Id);
            if (currentTemporalSale == null)
            {
                return NotFound();
            }

            currentTemporalSale!.Remarks = temporalSaleDTO.Remarks;
            currentTemporalSale.Quantity = temporalSaleDTO.Quantity;

            _context.Update(currentTemporalSale);
            await _context.SaveChangesAsync();
            return Ok(temporalSaleDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var temporalSale = await _context.TemporalSales.FirstOrDefaultAsync(x => x.Id == id);
            if (temporalSale == null)
            {
                return NotFound();
            }

            _context.Remove(temporalSale);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
