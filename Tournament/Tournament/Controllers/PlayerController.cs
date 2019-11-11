using System;
using System.Collections.Generic;
using System.Linq;
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
        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Player
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.PlayerItems.ToListAsync();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Player/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayers(long id)
        {
            var playerItem = await _context.PlayerItems.FindAsync(id);

            if (playerItem == null)
            {
                return NotFound();
            }

            return playerItem;
        }
        [Authorize(Roles = "User,Admin")]
        // POST: api/Player
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player item)
        {
            _context.PlayerItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayers), new { id = item.Id }, item);
        }
        [Authorize(Roles = "User,Admin")]
        // PUT: api/Player/{id}
        [HttpPut("{id}")]
        public IActionResult PutPlayer(long id, Player item)
        {
            var existingPlayer = _context.PlayerItems.Where(s => s.Id == id).FirstOrDefault<Player>();

            if (existingPlayer != null)
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
        [Authorize(Roles = "User,Moderator,Admin")]
        // DELETE: api/Player/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(long id)
        {
            var playerItem = await _context.PlayerItems.FindAsync(id);

            if (playerItem == null)
            {
                return NotFound();
            }

            _context.PlayerItems.Remove(playerItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
