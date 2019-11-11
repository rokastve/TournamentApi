using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TournamentApi.Models
{
    public class Player
    {
        [Key]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Region { get; set; }
        public string InGameName { get; set; }
    }
}
