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
    public class TournamentController : ControllerBase
    {
        private readonly TournamentContext _context;

        public TournamentController(TournamentContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Tournament
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournaments()
        {
            return await _context.TournamentItems
                .Include(i => i.participants)
                .ThenInclude(j => j.Players)
                .ToListAsync();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Tournament/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournament>> GetTournaments(long id)
        {
            var tournamentItems = await _context.TournamentItems
                .Include(i => i.participants)
                .ThenInclude(j => j.Players)
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (tournamentItems == null)
            {
                return NotFound();
            }

            return tournamentItems;
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Tournament/{id}/team
        [HttpGet("{tournamentId}/team")]
        public ActionResult<IEnumerable<Team>> GetTournamentByIdTeams(long tournamentId)
        {
            var tournamentItems =  _context.TournamentItems
                .Include(i => i.participants)
                .ThenInclude(j => j.Players)
                .Where(i => i.Id == tournamentId)
                .FirstOrDefault()
                .participants
                .ToList();

            if (tournamentItems == null)
            {
                return NotFound();
            }

            return tournamentItems;
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Tournament/{id}/team/{id}
        [HttpGet("{tournamentId}/team/{id}")]
        public ActionResult<Team> GetTournamentByIdTeams(long tournamentId, long id)
        {
            var tournamentItems =  _context.TournamentItems
                .Include(i => i.participants)
                .ThenInclude(j => j.Players)
                .Where(i => i.Id == tournamentId)
                .FirstOrDefault()
                .participants
                .Where(i => i.Id == id)
                .FirstOrDefault();

            if (tournamentItems == null)
            {
                return NotFound();
            }

            return tournamentItems;
        }
        [Authorize(Roles = "Admin")]
        // POST: api/Tournament/{id}/team/{id}
        [HttpPost("{tournamentId}/team/{id}")]
        public ActionResult<Team> AddTeamToTournament(long tournamentId, long id)
        {
            var team = _context.TeamItems.Include(i => i.Players).FirstOrDefault(t => t.Id == id);
            var tournament = _context.TournamentItems
                .Include(i => i.participants)
                .ThenInclude(j => j.Players)
                .Where(i => i.Id == tournamentId)
                .FirstOrDefault();
            if (tournament.participants.Count >= tournament.Capacity)
            {
                return BadRequest("No more room in tournament");
            }
            else if(tournament.participants.Contains(team)){
                return BadRequest("Team already in this tournament");
            }
            else
            {
                
                tournament.participants.Add(team);
                _context.SaveChanges();


                return CreatedAtAction(nameof(GetTournaments), tournament);
            }
        }
        [Authorize(Roles = "Admin")]
        // GET: api/Tournament/{id}/team/{id}
        [HttpDelete("{tournamentId}/team/{id}")]
        public ActionResult<Team> GetTeamFromTournament(long tournamentId, long id)
        {
            var tournament = _context.TournamentItems
               .Include(i => i.participants)
               .ThenInclude(j => j.Players)
               .Where(i => i.Id == tournamentId)
               .FirstOrDefault();

            var team = _context.TeamItems.Include(i => i.Players).FirstOrDefault(t => t.Id == id);
            tournament.participants.Remove(team);
            _context.SaveChanges();

            return NoContent();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Tournament/{id}/team/{id}/player
        [HttpGet("{tournamentId}/team/{teamId}/player")]
        public ActionResult<IEnumerable<Player>> GetPlayerByTeamByTournament(long tournamentId, long teamId)
        {
            var tournamentItems = _context.TournamentItems
                .Include(i => i.participants)
                .ThenInclude(j => j.Players)
                .Where(i => i.Id == tournamentId)
                .FirstOrDefault()
                .participants
                .Where(i => i.Id == teamId)
                .FirstOrDefault()
                .Players
                .ToList();

            if (tournamentItems == null)
            {
                return NotFound();
            }

            return tournamentItems;
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        // GET: api/Tournament/{id}/team/{id}
        [HttpGet("{tournamentId}/team/{teamId}/player/{id}")]
        public ActionResult<Player> GetPlayerByIdByTeamByTournament(long tournamentId, long teamId, long id)
        {
            var tournamentItems = _context.TournamentItems
                .Include(i => i.participants)
                .ThenInclude(j => j.Players)
                .Where(i => i.Id == tournamentId)
                .FirstOrDefault()
                .participants.Where(i => i.Id == teamId)
                .FirstOrDefault()
                .Players
                .Where(i => i.Id == id)
                .FirstOrDefault();

            if (tournamentItems == null)
            {
                return NotFound();
            }

            return tournamentItems;
        }


        [Authorize(Roles = "Admin")]
        // POST: api/Tournament
        [HttpPost]
        public async Task<ActionResult<Tournament>> PostTournament(Tournament item)
        {
            _context.TournamentItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTournaments), new { id = item.Id }, item);
        }
        [Authorize(Roles = "Admin")]
        // PUT: api/Tournament/{id}
        [HttpPut("{id}")]
        public IActionResult PutTournament(long id, Tournament item)
        {
            var existingTournament = _context.TournamentItems.Where(s => s.Id == id).FirstOrDefault<Tournament>();

            if (existingTournament != null)
            {
                existingTournament.Capacity = item.Capacity;
                existingTournament.participants = item.participants;

                _context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        // DELETE: api/Tournament/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(long id)
        {
            var tournamentItem = await _context.TournamentItems.Include(t => t.participants).Where(t => t.Id == id).FirstOrDefaultAsync();

            if (tournamentItem == null)
            {
                return NotFound();
            }
            

            _context.TournamentItems.Remove(tournamentItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
