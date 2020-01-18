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
        // POST: api/User
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserInfo>> PostUser(UserInfo item)
        {
            if(item.Username.Length == 0 || item.Password.Length == 0)
            {
                return BadRequest("Fields can't be empty");
            }
            UserInfo newUser = new UserInfo { Username = item.Username, Role = "User", Password = sha256(item.Password) };
            if (_context.Users.Where(u => u.Username == item.Username).FirstOrDefault() == null)
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok("User registered");
            }
            else
                return BadRequest("User Already Exists");
        }
        // POST: api/User/id
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<UserInfo>> PatchUser(long id, UserInfo item)
        {
            UserInfo user;
            if (( user = _context.Users.Where(u => u.Id ==id).FirstOrDefault()) != null)
            {
                if (item.Role != "User" && item.Role != "Moderator" && item.Role != "Admin")
                    return BadRequest("Bad Role");
                user.Role = item.Role;
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
                return BadRequest("User Does not exist");
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