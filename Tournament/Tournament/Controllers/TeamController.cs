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
        //GET: api/Team/{id}/Player
        [Authorize(Roles = "User,Moderator,Admin")]
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

        //GET: api/Team/{id}/Player/{id}
        [Authorize(Roles = "User,Moderator,Admin")]
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
        //POST: api/Team/{id}/Player/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost("{teamId}/player/{playerId}")]
        public async Task<ActionResult<Player>> AddPlayerToTeam(int teamId, int playerId)
        {
            var team = _context.TeamItems.Include(c => c.Players).FirstOrDefault(j => j.Id == teamId);
            var player = _context.PlayerItems.FirstOrDefault(j => j.Id == playerId);
            List<Player> players = team.Players;
            if (players.Contains(player))
            {
                return BadRequest("This player already exists in team");
            }
            //else if(_context.TeamItems.Include(c => c.Players).Where(t => t.Players.Contains(player)).FirstOrDefaultAsync() != null)
            //{
            //    return BadRequest("Player is in another team");
            //}
            else
            {
                players.Add(player);
                team.Players = players;
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetTeams), team);
            }
        }
        //DELETE: api/Team/{id}/Player/{id}
        [Authorize(Roles = "Moderator,Admin")]
        [HttpDelete("{teamId}/player/{playerId}")]
        public async Task<ActionResult<Player>> RemovePlayerFromTeam(int teamId, int playerId)
        {
            var team = _context.TeamItems.Include(c => c.Players).FirstOrDefault(j => j.Id == teamId);
            var player = _context.PlayerItems.FirstOrDefault(j => j.Id == playerId);
            team.Players.Remove(player);
            _context.SaveChanges();
            return NoContent();
        }
        // GET: api/Team
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.TeamItems
                .Include(i => i.Players)
                .ToListAsync();
        }

        // GET: api/Team/{id}
        [Authorize(Roles = "User,Moderator,Admin")]
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

        // POST: api/Team
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeamItem(Team item)
        {
            _context.TeamItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeams), new { id = item.Id }, item);
        }
        // PUT: api/Team/{id}
        [Authorize(Roles = "Admin")]
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
        // DELETE: api/Team/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(long id)
        {
            var teamItem = await _context.TeamItems.Include(t => t.Players).Where(i => i.Id == id).FirstOrDefaultAsync();
            List<Player> players = teamItem.Players;
            foreach(Player player in players)
            {
                teamItem.Players.Remove(player);
            }

            if (teamItem == null)
            {
                return NotFound();
            }

            _context.TeamItems.Remove(teamItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
