using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TournamentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly TournamentContext _context;


        public PlayerController(TournamentContext context)
        {
            _context = context;
        }
        // GET: api/Player
        [Authorize(Roles = "Moderator,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.PlayerItems.Include(p => p.user).ToListAsync();
        }

        // GET: api/Player/{id}
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayers(long id)
        {
            var identity = HttpContext.User.Claims.ToList();
            long userId;
            if (identity[0].Value != "null")
                userId = long.Parse(identity[0].Value);
            else
                userId = -99;
            var role = identity[1].Value;
            var clientId = long.Parse(identity[2].Value);
            if ((role == "User" && id == clientId))
            {
                var playerItem = await _context.PlayerItems.Include(p => p.user).Where(p => p.Id == id).FirstOrDefaultAsync();
                playerItem.user.Password = null;
                if (playerItem == null)
                {
                    return NotFound();
                }
                return playerItem;
            }
            else
            {
                return Unauthorized();
            }

        }
        // POST: api/Player
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player item)
        {
            var identity = HttpContext.User.Claims.ToList();
            long clientId = long.Parse(identity[2].Value);
            if (_context.PlayerItems.Include(u => u.user).Where(c => c.user.Id == clientId).FirstOrDefault() != null)
            {
                return BadRequest("You already have player profile");
            }
            else if(_context.PlayerItems.Include(u => u.user).Where(c => c.InGameName == item.InGameName).FirstOrDefault() != null)
                {
                return BadRequest("Player with this ingame name already exists");
            }else
            {
                item.user = _context.Users.Where(u => u.Id == clientId).FirstOrDefault();
                _context.PlayerItems.Add(item);
                await _context.SaveChangesAsync();

                return Ok("User created");
            }
        }
        // PUT: api/Player/{id}
        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public IActionResult PutPlayer(long id, Player item)
        {
            
            var existingPlayer = _context.PlayerItems.Where(s => s.Id == id).FirstOrDefault<Player>();
            var identity = HttpContext.User.Claims.ToList();
            long userId;
            if (identity[0].Value != "null")
                userId = long.Parse(identity[0].Value);
            else
                userId = -99;
            var role = identity[1].Value;

            if (existingPlayer != null && userId == id)
            {
                existingPlayer.InGameName = item.InGameName;
                existingPlayer.Region = item.Region;
                existingPlayer.Username = item.Username;
                _context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
            
        }
        // DELETE: api/Player/{id}
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(long id)
        {
            var identity = HttpContext.User.Claims.ToList();
            long userId;
            if (identity[0].Value != "null")
                userId = long.Parse(identity[0].Value);
            else
                userId = -99;
            var role = identity[1].Value;
            var playerItem = await _context.PlayerItems.FindAsync(id);

            if (playerItem == null)
            {
                return NotFound();
            }

            if (id == userId || role == "Admin")
            {
                _context.PlayerItems.Remove(playerItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            else
                return BadRequest();
        }
    }
}
