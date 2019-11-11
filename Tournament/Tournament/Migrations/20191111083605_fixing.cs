using Microsoft.EntityFrameworkCore.Migrations;

namespace TournamentAPI.Migrations
{
    public partial class fixing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "RefreshToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Revoked",
                table: "RefreshToken",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
