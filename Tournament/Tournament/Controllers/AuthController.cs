using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TournamentApi.Models;
using TournamentAPI.DTOs;
using TournamentAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace TournamentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TournamentContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration config, TournamentContext context)
        {
            _configuration = config;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserInfo request)
        {
            UserInfo curr;
            if ((curr = _context.Users.Where(c => c.Username == request.Username).Where(d => d.Password == request.Password).FirstOrDefault()) != null)
            {
                Player player = _context.PlayerItems.Include(p => p.user).Where(p => p.Id == curr.Id).FirstOrDefault();
                string playerId;
                if (player == null)
                     playerId= "null";
                else
                    playerId = player.Id.ToString();
                var userclaim = new[] { new Claim("playerId", playerId.ToString()),
                        new Claim("role", curr.Role),
                        new Claim("userId", curr.Id.ToString())};
                var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Jwt:SigningSecret"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "TournamentAPI",
                    audience: "TournamentUsers",
                    claims: userclaim,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: creds);

                var identity = new ClaimsIdentity(userclaim, DefaultAuthenticationTypes.ApplicationCookie);
                var claimsPrincipal = new ClaimsPrincipal(identity);
                // Set current principal
                Thread.CurrentPrincipal = claimsPrincipal;
                RefreshToken _refreshTokenObj = new RefreshToken
                {
                    Username = request.Username,
                    Refreshtoken = Guid.NewGuid().ToString(),
                };

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