using Azure;
using Microsoft.EntityFrameworkCore;
using Sales.Shared.DataBase;
using Sales.Shared.Entities;
using Sales.Shared.Enums;
using Sales.Shared.Responses;

namespace Sales.API.Helpers.platform
{
    public class OrdersHelper : IOrdersHelper   
    {
        private readonly SalesDbContext _context;

        public OrdersHelper(SalesDbContext context)
        {
            _context = context;
        }
        public async Task<GenericResponse<object>> ProcessOrderAsync(string email, string remarks)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return new GenericResponse<object>
                {
                    IsSuccess = false,
                    Message = "Usuario no válido"
                };
            }

            var temporalSales = await _context.TemporalSales
                .Include(x => x.Product)
                .Where(x => x.User!.Email == email)
                .ToListAsync();
            GenericResponse<object> response = await CheckInventoryAsync(temporalSales);
            if (!response.IsSuccess)
            {
                return response;
            }

            Sale sale = new()
            {
                Date = DateTime.UtcNow,
                User = user,
                Remarks = remarks,
                SaleDetails = new List<SaleDetail>(),
                OrderStatus = OrderStatus.Nuevo
            };

            foreach (var temporalSale in temporalSales)
            {
                sale.SaleDetails.Add(new SaleDetail
                {
                    Product = temporalSale.Product,
                    Quantity = temporalSale.Quantity,
                    Remarks = temporalSale.Remarks,
                });

                Product? product = await _context.Products.FindAsync(temporalSale.Product!.Id);
                if (product != null)
                {
                    product.Stock -= temporalSale.Quantity;
                    _context.Products.Update(product);
                }

                _context.TemporalSales.Remove(temporalSale);
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return response;
        }

        private async Task<GenericResponse<object>> CheckInventoryAsync(List<TemporalSale> temporalSales)
        {
            GenericResponse<object> response = new()
            {
                IsSuccess = true
            };

            foreach (var temporalSale in temporalSales)
            {
                Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == temporalSale.Product!.Id);
                if (product == null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = $"El producto {temporalSale.Product!.Name}, ya no está disponible";
                    return response;
                }
                if (product.Stock < temporalSale.Quantity)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = $"Lo sentimos no tenemos existencias suficientes del producto {temporalSale.Product!.Name}, para tomar su pedido. Por favor disminuir la cantidad o sustituirlo por otro.";
                    return response;
                }
            }
            return response;
        }
    }
}
