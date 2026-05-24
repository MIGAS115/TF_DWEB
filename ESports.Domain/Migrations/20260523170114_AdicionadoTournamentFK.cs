using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESports.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AdicionadoTournamentFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_NormalUserFK",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "NormalUserFK",
                table: "Favorites",
                newName: "UserFK");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "RegistrationDate",
                table: "AspNetUsers",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_UserFK",
                table: "Favorites",
                column: "UserFK",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_UserFK",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "UserFK",
                table: "Favorites",
                newName: "NormalUserFK");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_NormalUserFK",
                table: "Favorites",
                column: "NormalUserFK",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
