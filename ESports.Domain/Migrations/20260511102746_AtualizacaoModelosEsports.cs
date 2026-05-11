using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESports.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoModelosEsports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavoriteTeams");

            migrationBuilder.AddColumn<int>(
                name: "TournamentFK",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PermissionLevel",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    NormalUserFK = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TeamFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.NormalUserFK, x.TeamFK });
                    table.ForeignKey(
                        name: "FK_Favorites_AspNetUsers_NormalUserFK",
                        column: x => x.NormalUserFK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Teams_TeamFK",
                        column: x => x.TeamFK,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalSourceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsManualOverride = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TournamentTeams",
                columns: table => new
                {
                    TournamentFK = table.Column<int>(type: "int", nullable: false),
                    TeamFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentTeams", x => new { x.TournamentFK, x.TeamFK });
                    table.ForeignKey(
                        name: "FK_TournamentTeams_Teams_TeamFK",
                        column: x => x.TeamFK,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentTeams_Tournaments_TournamentFK",
                        column: x => x.TournamentFK,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TournamentFK",
                table: "Matches",
                column: "TournamentFK");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_TeamFK",
                table: "Favorites",
                column: "TeamFK");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentTeams_TeamFK",
                table: "TournamentTeams",
                column: "TeamFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Tournaments_TournamentFK",
                table: "Matches",
                column: "TournamentFK",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Tournaments_TournamentFK",
                table: "Matches");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "TournamentTeams");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Matches_TournamentFK",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "TournamentFK",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PermissionLevel",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UserFavoriteTeams",
                columns: table => new
                {
                    MyUserFK = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TeamFK = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteTeams", x => new { x.MyUserFK, x.TeamFK });
                    table.ForeignKey(
                        name: "FK_UserFavoriteTeams_AspNetUsers_MyUserFK",
                        column: x => x.MyUserFK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteTeams_Teams_TeamFK",
                        column: x => x.TeamFK,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteTeams_TeamFK",
                table: "UserFavoriteTeams",
                column: "TeamFK");
        }
    }
}
