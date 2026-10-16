using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddTickerToInvestmentRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ticker",
                table: "Investments",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Investments_Ticker",
                table: "Investments",
                column: "Ticker");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_StockPrices_Ticker",
                table: "Investments",
                column: "Ticker",
                principalTable: "StockPrices",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investments_StockPrices_Ticker",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Investments_Ticker",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "Ticker",
                table: "Investments");
        }
    }
}
