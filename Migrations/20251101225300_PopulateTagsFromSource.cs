using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class PopulateTagsFromSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert distinct sources from Incomes as Tags with context 'Income'
            migrationBuilder.Sql(@"
                INSERT INTO Tags (Name, Context, UserId)
                SELECT DISTINCT Source, 'Income', UserId FROM Incomes
                WHERE Source IS NOT NULL
            ");

            // Update Incomes to set TagId based on matching Tag Name and Context
            migrationBuilder.Sql(@"
                UPDATE Incomes
                SET TagId = t.Id
                FROM Incomes i
                INNER JOIN Tags t ON i.Source = t.Name AND t.Context = 'Income' AND i.UserId = t.UserId
                WHERE i.Source IS NOT NULL
            ");

            // Insert distinct categories from Expenses as Tags with context 'Expense'
            migrationBuilder.Sql(@"
                INSERT INTO Tags (Name, Context, UserId)
                SELECT DISTINCT Category, 'Expense', UserId FROM Expenses
                WHERE Category IS NOT NULL
           ");
            // Update Expenses to set TagId based on matching Tag Name and Context
            migrationBuilder.Sql(@"
                UPDATE Expenses
                SET TagId = t.Id
                FROM Expenses e
                INNER JOIN Tags t ON e.Category = t.Name AND t.Context = 'Expense' AND e.UserId = t.UserId
                WHERE e.Category IS NOT NULL
            ");

            // Insert distinct types from Investments as Tags with context 'Investment'
            migrationBuilder.Sql(@"
                INSERT INTO Tags (Name, Context, UserId)
                SELECT DISTINCT Type, 'Investment', UserId FROM Investments
                WHERE Type IS NOT NULL
            ");
            // Update Investments to set TagId based on matching Tag Name and Context
            migrationBuilder.Sql(@"
                UPDATE Investments
                SET TagId = t.Id
                FROM Investments inv
                INNER JOIN Tags t ON inv.Type = t.Name AND t.Context = 'Investment' AND inv.UserId = t.UserId
                WHERE inv.Type IS NOT NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
