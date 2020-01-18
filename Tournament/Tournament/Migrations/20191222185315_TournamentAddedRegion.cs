using Microsoft.EntityFrameworkCore.Migrations;

namespace TournamentAPI.Migrations
{
    public partial class TournamentAddedRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Region",
                table: "Tournament",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Region",
                table: "Tournament");
        }
    }
}
