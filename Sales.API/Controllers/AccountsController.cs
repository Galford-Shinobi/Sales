using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Sales.API.Helpers;
using Sales.API.Helpers.platform;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;
using Sales.Shared.Responses;
using Sales.Shared.ViewsModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Sales.API.Controllers
{
    [ApiController]
    [Route("/api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly IMailHelper _mailHelper;
        private readonly IFireBaseService _fireBaseService;
        private readonly string _container;


        public AccountsController(IUserHelper userHelper, IConfiguration configuration, IFileStorage fileStorage, IMailHelper mailHelper, IFireBaseService fireBaseService)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _mailHelper = mailHelper;
            _fireBaseService = fireBaseService;
            _container = "users";
        }
        [HttpPost("CreateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO model)
        {
            User user = model;
            string nombreImagen = "";
            var StorageCarpeta_Usuario = _configuration["Configuracion:FireBase_StorageCarpeta_Usuario"];

            if (!string.IsNullOrEmpty(model.Photo))
            {
                string nombre_en_codigo = Guid.NewGuid().ToString("N");
                string extension = ".png"; //Path.GetExtension();
                nombreImagen = string.Concat(nombre_en_codigo, extension);

                var photoUser = Convert.FromBase64String(model.Photo);
                var fileFromBase64ToStream = FirebaseStorageService.ConvertBase64ToStream(model.Photo);
                var fileStream = fileFromBase64ToStream.ReadAsStream();
                user.MyFileStorageImage = await _fireBaseService.SubirStorageAsync(fileStream, StorageCarpeta_Usuario, nombreImagen);
                model.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
            }

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());
                return Ok(BuildToken(user));
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }


        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Email);
                return Ok(BuildToken(user));
            }

            return BadRequest("Email o contraseña incorrectos.");
        }

        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("Document", user.Document),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Address", user.Address),
                new Claim("Photo", user.Photo ?? string.Empty),
                 new Claim("MyFileStorageImage", user.MyFileStorageImage ?? string.Empty),
                new Claim("CityId", user.CityId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var UserResp = new UserResponse { Id =  user.Id, Document = user.Document, FirstName = user.FirstName,LastName = user.LastName, Email = user.Email };

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                User = UserResp,
            };
        }

        [HttpPost("ResedToken")]
        public async Task<ActionResult> ResedToken([FromBody] EmailDTO model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            //TODO: Improve code 
            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "accounts", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["UrlWEB"]);

            var response = _mailHelper.SendMail(user.FullName, user.Email!,
                $"Saless- Confirmación de cuenta",
                $"<h1>Sales - Confirmación de cuenta</h1>" +
                $"<p>Para habilitar el usuario, por favor hacer clic 'Confirmar Email':</p>" +
                $"<b><a href ={tokenLink}>Confirmar Email</a></b>");

            if (response.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmailAsync(string userId, string token)
        {
            token = token.Replace(" ", "+");
            var user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault());
            }

            return NoContent();
        }
        private static async Task<Stream> Upload(string FileBase64)
        {
            var fileFromBase64ToStream = FirebaseStorageService.ConvertBase64ToStream(FileBase64);
            var fileStream = fileFromBase64ToStream.ReadAsStream();

            //string fileUrlFirebase = await FirebaseStorageService.UploadFile(fileStream, file);
            return fileStream;
        }
        //private static async Task<string> Upload(FileModel file)
        //{
        //    var fileFromBase64ToStream = FirebaseStorageService.ConvertBase64ToStream(file.FileBase64);
        //    var fileStream = fileFromBase64ToStream.ReadAsStream();

        //    string fileUrlFirebase = await FirebaseStorageService.UploadFile(fileStream, file);
        //    return fileUrlFirebase;
        //}
    }
}
