﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Helpers;
using Sales.Shared.Applications.Interfaces;
using Sales.Shared.DataBase;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CountriesController : BaseApiController
    {
        private readonly SalesDbContext _salesDbContext;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesController(SalesDbContext salesDbContext, ICountriesRepository countriesRepository)
        {
            _salesDbContext = salesDbContext;
            _countriesRepository = countriesRepository;
        }

        [AllowAnonymous]
        [HttpGet("combo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCombo()
        {
            return Ok(await _salesDbContext.Countries.ToListAsync());
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "PorDefecto20Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _salesDbContext.Countries
                .Include(x => x.States)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryable = _salesDbContext.Countries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        //[HttpGet]
        //[ResponseCache(Duration = 20)]
        //[ResponseCache(CacheProfileName = "PorDefecto20Segundos")]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult> GetAsync()
        //{
        //    try
        //    {
        //        return Ok(await _countriesRepository.GetAllCountryAsync());
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //       "Error retrieving data from the database");
        //    }
            
        //}

        [HttpGet("full")]
        [ResponseCache(CacheProfileName = "PorDefecto20Segundos")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFullAsync()
        {
            return Ok(await _countriesRepository.GetFullCountryAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
