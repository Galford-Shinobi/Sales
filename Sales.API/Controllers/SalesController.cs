﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Helpers;
using Sales.Shared.DataBase;
using Sales.Shared.DTOs;
using Sales.Shared.Enums;

namespace Sales.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/sales")]
    public class SalesController : ControllerBase
    {
        private readonly IOrdersHelper _ordersHelper;
        private readonly SalesDbContext _context;
        private readonly IUserHelper _userHelper;

        public SalesController(IOrdersHelper ordersHelper, SalesDbContext context, IUserHelper userHelper)
        {
            _ordersHelper = ordersHelper;
            _context = context;
            _userHelper = userHelper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post(SaleDTO saleDTO)
        {
            var response = await _ordersHelper.ProcessOrderAsync(User.Identity!.Name!, saleDTO.Remarks);
            if (response.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get([FromQuery] PaginationDTO pagination)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity!.Name);
            if (user == null)
            {
                return BadRequest("User not valid.");
            }

            var queryable = _context.Sales
                .Include(s => s.User!)
                .Include(s => s.SaleDetails!)
                .ThenInclude(sd => sd.Product)
                .AsQueryable();

            var isAdmin = await _userHelper.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == User.Identity!.Name);
            }

            return Ok(await queryable
                .OrderByDescending(x => x.Date)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity!.Name);
            if (user == null)
            {
                return BadRequest("User not valid.");
            }

            var queryable = _context.Sales
                .AsQueryable();

            var isAdmin = await _userHelper.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == User.Identity!.Name);
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }
    }
}