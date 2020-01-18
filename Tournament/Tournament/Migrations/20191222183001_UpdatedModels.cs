using Microsoft.EntityFrameworkCore.Migrations;

namespace TournamentAPI.Migrations
{
    public partial class UpdatedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tournament",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WinnersId",
                table: "Tournament",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Team",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "Team",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Region",
                table: "Player",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_WinnersId",
                table: "Tournament",
                column: "WinnersId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_OwnerId",
                table: "Team",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Player_OwnerId",
                table: "Team",
                column: "OwnerId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournament_Team_WinnersId",
                table: "Tournament",
                column: "WinnersId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Team_Player_OwnerId",
                table: "Team");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournament_Team_WinnersId",
                table: "Tournament");

            migrationBuilder.DropIndex(
                name: "IX_Tournament_WinnersId",
                table: "Tournament");

            migrationBuilder.DropIndex(
                name: "IX_Team_OwnerId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tournament");

            migrationBuilder.DropColumn(
                name: "WinnersId",
                table: "Tournament");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Team");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Player",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Refreshtoken = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                });
        }
    }
}
