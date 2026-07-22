using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddImportIdToRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImportId",
                table: "Investments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImportId",
                table: "Incomes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImportId",
                table: "Expenses",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "Expenses");
        }
    }
}
