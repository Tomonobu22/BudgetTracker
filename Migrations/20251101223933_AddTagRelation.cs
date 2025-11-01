using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddTagRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Investments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Incomes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Investments_TagId",
                table: "Investments",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_TagId",
                table: "Incomes",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TagId",
                table: "Expenses",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Tags_TagId",
                table: "Expenses",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Tags_TagId",
                table: "Incomes",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_Tags_TagId",
                table: "Investments",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Tags_TagId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Tags_TagId",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Investments_Tags_TagId",
                table: "Investments");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Investments_TagId",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_TagId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_TagId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Expenses");
        }
    }
}
