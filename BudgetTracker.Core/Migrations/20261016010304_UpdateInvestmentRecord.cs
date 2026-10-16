using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvestmentRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Investments",
                newName: "PurchasePrice");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "Investments",
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
                name: "Quantity",
                table: "Investments");

            migrationBuilder.RenameColumn(
                name: "PurchasePrice",
                table: "Investments",
                newName: "Amount");
        }
    }
}
