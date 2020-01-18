using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TournamentApi.Models
{
    public class Team
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string Description { get; set; }
        public virtual List<Player> Players { get; set; }
        public Player Owner { get; set; }
    }
}
