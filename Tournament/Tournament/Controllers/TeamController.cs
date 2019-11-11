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
    public class TeamController : ControllerBase
    {
        private readonly TournamentContext _context;
        public TeamController(TournamentContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        //GET: api/Team/{id}/Player
        [HttpGet("{teamId}/player")]
        public async Task<ActionResult<IEnumerable<Player>>> FindPlayersByTeam(int teamId)
        {
            var teamItem = await _context.TeamItems
                .Include(i => i.Players)
                .Where(j => j.Id == teamId)
                .Select(i => i.Players)
                .FirstOrDefaultAsync();

            if (teamItem == null)
            {
                return NotFound();
            }

            return teamItem;
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        //GET: api/Team/{id}/Player/{id}
        [HttpGet("{teamId}/player/{playerId}")]
        public async Task<ActionResult<Player>> FindPlayerByIdInTeam(int teamId, int playerId)
        {
            var teamItem = await _context.TeamItems
                .Include(i => i.Players)
                .Where(j => j.Id == teamId)
                .Select(i => i.Players)
                .Select(m => m.FirstOrDefault(z => z.Id == playerId))
                .FirstOrDefaultAsync();

            if (teamItem == null)
            {
                return NotFound();
            }

            return teamItem;
        }
        [Authorize(Roles = "User,Admin")]
        //POST: api/Team/{id}/Player/{id}
        [HttpPost("{teamId}/player/{playerId}")]
        public async Task<ActionResult<Player>> AddPlayerToTeam(int teamId, int playerId)
        {
            var team = _context.TeamItems.Include(c => c.Players).FirstOrDefault(j => j.Id == teamId);
            var player = _context.PlayerItems.FirstOrDefault(j => j.Id == playerId);
            List<Player> players = team.Players;
            players.Add(player);
            team.Players = players;
            _context.SaveChanges();
            return Ok();
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        //DELETE: api/Team/{id}/Player/{id}
        [HttpDelete("{teamId}/player/{playerId}")]
        public async Task<ActionResult<Player>> RemovePlayerFromTeam(int teamId, int playerId)
        {
            var team = _context.TeamItems.Include(c => c.Players).FirstOrDefault(j => j.Id == teamId);
            var player = _context.PlayerItems.FirstOrDefault(j => j.Id == playerId);
            team.Players.Remove(player);
            _context.SaveChanges();
            return Ok();
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Team
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.TeamItems
                .Include(i => i.Players)
                .ToListAsync();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Team/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeams(long id)
        {
            var teamItem = await _context.TeamItems
                .Include(i => i.Players)
                .Where(j => j.Id == id)
                .FirstOrDefaultAsync();

            if (teamItem == null)
            {
                return NotFound();
            }

            return teamItem;
        }

        [Authorize(Roles = "User,Admin")]
        // POST: api/Team
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeamItem(Team item)
        {
            _context.TeamItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeams), new { id = item.Id }, item);
        }
        [Authorize(Roles = "User,Admin")]
        // PUT: api/Team/{id}
        [HttpPut("{id}")]
        public IActionResult PutTeam(long id, Team item)
        {
            var existingTeam = _context.TeamItems.Where(s => s.Id == id).FirstOrDefault<Team>();

            if (existingTeam != null)
            {
                existingTeam.Name = item.Name;
                existingTeam.Region = item.Region;
                existingTeam.Players = item.Players;

                _context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        // DELETE: api/Team/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(long id)
        {
            var teanItem = await _context.TeamItems.FindAsync(id);

            if (teanItem == null)
            {
                return NotFound();
            }

            _context.TeamItems.Remove(teanItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
