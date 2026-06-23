using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESports.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTournamentPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrizePool",
                table: "Tournaments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrizePool",
                table: "Tournaments");
        }
    }
}
