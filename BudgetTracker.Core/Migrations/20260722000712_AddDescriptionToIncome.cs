using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToIncome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Incomes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Incomes");
        }
    }
}
