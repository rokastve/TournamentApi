using Microsoft.EntityFrameworkCore.Migrations;

namespace TournamentAPI.Migrations
{
    public partial class LinkPlayerWithUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "Player",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Player_userId",
                table: "Player",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Player_User_userId",
                table: "Player",
                column: "userId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Player_User_userId",
                table: "Player");

            migrationBuilder.DropIndex(
                name: "IX_Player_userId",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Player");
        }
    }
}
