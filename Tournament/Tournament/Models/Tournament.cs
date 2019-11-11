using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TournamentApi.Models
{
    public class Tournament
    {
        [Key]
        public long Id { get; set; }
        public int Capacity { get; set; }
        public virtual List<Team> participants { get; set; }
    }
}
