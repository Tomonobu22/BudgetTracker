using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class ReportAppService : IReportAppService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IInvestmentRepository _investmentRepository;

        public ReportAppService(
            IIncomeRepository incomeRepository,
            IExpenseRepository expenseRepository,
            IInvestmentRepository investmentRepository)
        {
            _incomeRepository = incomeRepository;
            _expenseRepository = expenseRepository;
            _investmentRepository = investmentRepository;
        }


        public async Task<ReportSummaryViewModel> GetSummaryReportAsync(string userId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be after end date.");

            var totalIncome = await _incomeRepository.GetTotalIncomeAsync(userId, startDate, endDate);
            var totalExpenses = await _expenseRepository.GetTotalExpenseAsync(userId, startDate, endDate);
            var totalInvestments = await _investmentRepository.GetTotalInvestmentAsync(userId, startDate, endDate);

            return new ReportSummaryViewModel
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                TotalInvestments = totalInvestments
            };
        }
    }
}
