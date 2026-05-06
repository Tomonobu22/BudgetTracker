using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class ProductionSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Investments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Incomes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
