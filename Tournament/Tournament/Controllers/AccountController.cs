using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TournamentApi.Models;
using TournamentAPI.DTOs;
using TournamentAPI.Models;
using TournamentAPI.Services;

namespace TournamentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TournamentContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration config, TournamentContext context)
        {
            _configuration = config;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginDto request)
        {
            User curr;
            if ((curr = _context.Users.Where(c => c.Username == request.Username).Where(d => d.Password == request.Password).FirstOrDefault()) != null)
            {

                var userclaim = new[] { new Claim("userid", curr.Id.ToString()),
                        new Claim("role", curr.Role) };
                var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Jwt:SigningSecret"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: userclaim,
                    expires: DateTime.Now.AddMinutes(2),
                    signingCredentials: creds);

                RefreshToken _refreshTokenObj = new RefreshToken
                {
                    Username = request.Username,
                    Refreshtoken = Guid.NewGuid().ToString(),
                };
                _context.RefreshTokens.Add(_refreshTokenObj);
                _context.SaveChanges();

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = _refreshTokenObj.Refreshtoken
                });
            }

            return BadRequest("Could not verify username and password");
        }
    }
}