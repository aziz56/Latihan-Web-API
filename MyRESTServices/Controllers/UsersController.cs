using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRESTServices.BLL.DTOs;
using MyRESTServices.BLL.Interfaces;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using MyRESTServices.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyRESTServices.LoginViewModel;
namespace MyRESTServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserBLL _userBLL;
        private readonly IRoleBLL _roleBLL;
        private readonly AppSettings _appSettings;

        public UsersController(IUserBLL userBLL, IRoleBLL roleBLL, AppSettings appSettings)
        {
            _userBLL = userBLL;
            _roleBLL = roleBLL;
            _appSettings = appSettings;
        }






        //GetAllWithRoles
        [HttpGet]
        public async Task<IEnumerable<UserDTO>> Get()
        {
            var results = await _userBLL.GetAllWithRoles();
            return results;
        }
        //GetByUsername
        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var result = await _userBLL.GetByUsername(username);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        //Login

        //[HttpPost("Login")]
        //public async Task<IActionResult> Login(LoginDTO loginDTO)
        //{
        //    var result = await _userBLL.Login(loginDTO);
        //    if (result == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);
        //}
        //Insert
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddUser(UserCreateDTO userCreateDTO)
        {
            try
            {
                await _userBLL.Insert(userCreateDTO);
                return Ok("User added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to add user: {ex.Message}");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {

            var user = await _userBLL.Login(loginDTO);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            
            var userWithRoles = await _userBLL.GetUserWithRoles(user.Username);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
           
            foreach (var role in userWithRoles.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                 
        
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userWithToken = new
            {
                user = userWithRoles,
                token = tokenHandler.WriteToken(token)
            };
            return Ok(userWithToken);

         
            
            
        }

    }
}
