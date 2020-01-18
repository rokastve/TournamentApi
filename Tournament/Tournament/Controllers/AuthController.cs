using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TournamentApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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
        static string sha256(string randomString)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserInfo request)
        {
            UserInfo curr;
            if ((curr = _context.Users.Where(c => c.Username == request.Username).Where(d => d.Password == sha256(request.Password)).FirstOrDefault()) != null)
            {
                curr.Password = null;
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

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token), curr
                });
            }
            return BadRequest("Could not verify username and password");
        }
    }
}