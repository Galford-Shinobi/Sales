using Firebase.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Helpers;
using Sales.API.Helpers.platform;
using Sales.Shared.DataBase;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly SalesDbContext _context;
        private readonly IFileStorage _fileStorage;
        private readonly IFireBaseService _fireBaseService;
        private readonly IConfiguration _configuration;

        public ProductsController(SalesDbContext context, IFileStorage fileStorage, IFireBaseService fireBaseService, IConfiguration configuration)
        {
            _context = context;
            _fileStorage = fileStorage;
            _fireBaseService = fireBaseService;
            _configuration = configuration;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductCategories)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Products
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAsync(int id)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductCategories!)
                .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostAsync([FromBody]ProductDTO productDTO)
        {
            string nombreImagen = "";
            var StorageCarpeta_Products = _configuration["Configuracion:FireBase_StorageCarpeta_Producto"];
            try
            {
                Product newProduct = new()
                {
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    Price = productDTO.Price,
                    Stock = productDTO.Stock,
                    ProductCategories = new List<ProductCategory>(),
                    ProductImages = new List<ProductImage>()
                };

                foreach (var productImage in productDTO.ProductImages!)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = ".png"; //Path.GetExtension();
                    nombreImagen = string.Concat(nombre_en_codigo, extension);

                    var photoProduct = Convert.FromBase64String(productImage);
                    newProduct.ProductImages.Add(new ProductImage { Image = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "products") });


                    var fileFromBase64ToStream = FirebaseStorageService.ConvertBase64ToStream(productImage);
                    var fileStream = fileFromBase64ToStream.ReadAsStream();
                    newProduct.ProductImages.Add(new ProductImage { ImageFireBase = await _fireBaseService.SubirStorageAsync(fileStream, StorageCarpeta_Products!, nombreImagen) });
                }

                foreach (var productCategoryId in productDTO.ProductCategoryIds!)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == productCategoryId);
                    newProduct.ProductCategories.Add(new ProductCategory { Category = category! });
                }

                _context.Add(newProduct);
                await _context.SaveChangesAsync();
                return Ok(productDTO);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe una ciudad con el mismo nombre.");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }


        [HttpPost("addImages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostAddImagesAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            if (product.ProductImages is null)
            {
                product.ProductImages = new List<ProductImage>();
            }

            for (int i = 0; i < imageDTO.Images.Count; i++)
            {
                if (!imageDTO.Images[i].StartsWith("https://storageblazorimage.blob.core.windows.net/products/"))
                {
                    var photoProduct = Convert.FromBase64String(imageDTO.Images[i]);
                    imageDTO.Images[i] = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "products");
                    product.ProductImages!.Add(new ProductImage { Image = imageDTO.Images[i] });
                }
            }

            _context.Update(product);
            await _context.SaveChangesAsync();
            return Ok(imageDTO);
        }

        [HttpPost("removeLastImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostRemoveLastImageAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            if (product.ProductImages is null || product.ProductImages.Count == 0)
            {
                return Ok();
            }

            var lastImage = product.ProductImages.LastOrDefault();
            await _fileStorage.RemoveFileAsync(lastImage!.Image, "products");
            product.ProductImages.Remove(lastImage);
            _context.Update(product);
            await _context.SaveChangesAsync();
            imageDTO.Images = product.ProductImages.Select(x => x.Image).ToList();
            return Ok(imageDTO);
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PutAsync([FromBody] ProductDTO productDTO)
        {
            try
            {
                var product = await _context.Products
                    .Include(x => x.ProductCategories)
                    .FirstOrDefaultAsync(x => x.Id == productDTO.Id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = productDTO.Name;
                product.Description = productDTO.Description;
                product.Price = productDTO.Price;
                product.Stock = productDTO.Stock;
                product.CodeBar = productDTO.CodeBar;
                product.ProductCategories = productDTO.ProductCategoryIds!.Select(x => new ProductCategory { CategoryId = x }).ToList();

                _context.Update(product);
                await _context.SaveChangesAsync();
                return Ok(productDTO);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe una ciudad con el mismo nombre.");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
