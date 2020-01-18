using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore;
using TournamentApi.Models;

namespace TournamentApi.Models
{
    public class TournamentContext : DbContext
    {
        public TournamentContext(DbContextOptions<TournamentContext> options) : base(options)
        {

        }

        public DbSet<Tournament> TournamentItems { get; set; }
        public DbSet<Team> TeamItems { get; set; }
        public DbSet<Player> PlayerItems { get; set; }
        public DbSet<UserInfo> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().ToTable("Player");
            modelBuilder.Entity<Team>().ToTable("Team");
            modelBuilder.Entity<Tournament>().ToTable("Tournament");
            modelBuilder.Entity<UserInfo>().ToTable("User");


        }
    }
}
