using BudgetTracker.Core.DTOs;

namespace BudgetTracker.Web.ViewModels
{
    public class ImportDetailsViewModel
    {
        public ImportDto Import { get; set; }
        public List<ExpenseDto>? Expenses { get; set; }
        public List<IncomeDto>? Incomes { get; set; }
        public List<InvestmentDto>? Investments { get; set; }
    }
}
