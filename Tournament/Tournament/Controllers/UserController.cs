using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace TournamentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TournamentContext _context;


        public UserController(TournamentContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        // POST: api/Player
        [HttpPost]
        public async Task<ActionResult<UserInfo>> PostUser(UserInfo item)
        {
            UserInfo newUser = new  UserInfo { Username = item.Username, Role = "User", Password = sha256(item.Password) };
            if (_context.Users.Where(u => u.Username == item.Username).FirstOrDefault() == null)
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok("User registered");
            }
            else
                return BadRequest("User Already Exists");
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
    }
}