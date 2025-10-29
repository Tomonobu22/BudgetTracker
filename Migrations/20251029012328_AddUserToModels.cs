using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Investments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Incomes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Expenses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Investments_UserId1",
                table: "Investments",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_UserId1",
                table: "Incomes",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId1",
                table: "Expenses",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_UserId1",
                table: "Expenses",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_AspNetUsers_UserId1",
                table: "Incomes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_AspNetUsers_UserId1",
                table: "Investments",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_UserId1",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_AspNetUsers_UserId1",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Investments_AspNetUsers_UserId1",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Investments_UserId1",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_UserId1",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_UserId1",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Expenses");
        }
    }
}
