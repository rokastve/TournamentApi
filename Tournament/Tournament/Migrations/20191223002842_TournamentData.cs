using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TournamentAPI.Migrations
{
    public partial class TournamentData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tournament",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "Tournament",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tournament");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Tournament");
        }
    }
}
