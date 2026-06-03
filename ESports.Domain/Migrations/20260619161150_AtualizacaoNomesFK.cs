using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESports.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoNomesFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_RegularUsers_NormalFK",
                table: "Favoritos");

            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_Teams_TeamFK",
                table: "Favoritos");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_AwayTeamFK",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_HomeTeamFK",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Tournaments_TournamentFK",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_TorneioEquipa_Teams_TeamFK",
                table: "TorneioEquipa");

            migrationBuilder.DropForeignKey(
                name: "FK_TorneioEquipa_Tournaments_TournamentFK",
                table: "TorneioEquipa");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "RegularUsers");

            migrationBuilder.AddColumn<int>(
                name: "CategoryFK",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "NormalFK",
                table: "Favoritos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissionLevel",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "RegistrationDate",
                table: "AspNetUsers",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CategoryFK",
                table: "Teams",
                column: "CategoryFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_AspNetUsers_NormalFK",
                table: "Favoritos",
                column: "NormalFK",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_Teams_TeamFK",
                table: "Favoritos",
                column: "TeamFK",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_AwayTeamFK",
                table: "Matches",
                column: "AwayTeamFK",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_HomeTeamFK",
                table: "Matches",
                column: "HomeTeamFK",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Tournaments_TournamentFK",
                table: "Matches",
                column: "TournamentFK",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Categories_CategoryFK",
                table: "Teams",
                column: "CategoryFK",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TorneioEquipa_Teams_TeamFK",
                table: "TorneioEquipa",
                column: "TeamFK",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TorneioEquipa_Tournaments_TournamentFK",
                table: "TorneioEquipa",
                column: "TournamentFK",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_AspNetUsers_NormalFK",
                table: "Favoritos");

            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_Teams_TeamFK",
                table: "Favoritos");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_AwayTeamFK",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_HomeTeamFK",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Tournaments_TournamentFK",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Categories_CategoryFK",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_TorneioEquipa_Teams_TeamFK",
                table: "TorneioEquipa");

            migrationBuilder.DropForeignKey(
                name: "FK_TorneioEquipa_Tournaments_TournamentFK",
                table: "TorneioEquipa");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Teams_CategoryFK",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CategoryFK",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "PermissionLevel",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "NormalFK",
                table: "Favoritos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MyUserFK = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_AspNetUsers_MyUserFK",
                        column: x => x.MyUserFK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegularUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MyUserFK = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegistrationDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegularUsers_AspNetUsers_MyUserFK",
                        column: x => x.MyUserFK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_MyUserFK",
                table: "Admins",
                column: "MyUserFK");

            migrationBuilder.CreateIndex(
                name: "IX_RegularUsers_MyUserFK",
                table: "RegularUsers",
                column: "MyUserFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_RegularUsers_NormalFK",
                table: "Favoritos",
                column: "NormalFK",
                principalTable: "RegularUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_Teams_TeamFK",
                table: "Favoritos",
                column: "TeamFK",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_AwayTeamFK",
                table: "Matches",
                column: "AwayTeamFK",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_HomeTeamFK",
                table: "Matches",
                column: "HomeTeamFK",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Tournaments_TournamentFK",
                table: "Matches",
                column: "TournamentFK",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TorneioEquipa_Teams_TeamFK",
                table: "TorneioEquipa",
                column: "TeamFK",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TorneioEquipa_Tournaments_TournamentFK",
                table: "TorneioEquipa",
                column: "TournamentFK",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
