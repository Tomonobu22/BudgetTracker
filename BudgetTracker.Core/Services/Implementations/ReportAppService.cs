using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;
using BudgetTracker.Core.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Core.Services.Implementations
{
    public class ReportAppService : IReportAppService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ReportAppService> _logger;

        public ReportAppService(
            IIncomeRepository incomeRepository,
            IExpenseRepository expenseRepository,
            IInvestmentRepository investmentRepository,
            ICacheService cacheService,
            ILogger<ReportAppService> logger
            )
        {
            _incomeRepository = incomeRepository;
            _expenseRepository = expenseRepository;
            _investmentRepository = investmentRepository;
            _cacheService = cacheService;
            _logger = logger;
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
                TotalInvestments = totalInvestments,
                Year = startDate.Year
            };
        }

        public async Task<MonthlySummaryViewModel> GetMonthlySummaryAsync(string userId, int year)
        {
            if (year < 2000 || year > DateTime.Now.Year)
            {
                throw new ArgumentException("Year is out of valid range.");
            }

            var cacheKey = CacheKeys.GetMonthlySummaryKey(userId, year);
            var cachedMonthlySummary = _cacheService.Get<MonthlySummaryViewModel?>(cacheKey);
            if (cachedMonthlySummary != null)
            {
                _logger.LogInformation($"Cache hit {cacheKey}");
                return cachedMonthlySummary;
            }

            var monthlyIncome = await _incomeRepository.GetMonthlyIncomeAsync(userId, year);
            var monthlyExpense = await _expenseRepository.GetMonthlyExpenseAsync(userId, year);
            var monthlyInvestment = await _investmentRepository.GetMonthlyInvestmentAsync(userId, year);
            
            var result = new MonthlySummaryViewModel
            {
                Year = year,
                MonthlyIncome = monthlyIncome,
                MonthlyExpenses = monthlyExpense,
                MonthlyInvestments = monthlyInvestment
            };

            // Cache the result for 10 minutes
            _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            _logger.LogInformation($"Cache set {cacheKey}");
            return result;
        }

        public async Task<List<int>> GetAvailableYearsAsync(string userId)
        {
            var cacheKey = CacheKeys.AvailableYearsKey(userId);
            var cachedYears = _cacheService.Get<List<int>>(cacheKey);
            if (cachedYears != null)
            {
                _logger.LogInformation($"Cache hit {cacheKey}");
                return cachedYears;
            }

            var incomeYears = await _incomeRepository.GetYearsWithDataAsync(userId);
            var expenseYears = await _expenseRepository.GetYearsWithDataAsync(userId);
            var investmentYears = await _investmentRepository.GetYearsWithDataAsync(userId);
            var allYears = incomeYears
                .Union(expenseYears)
                .Union(investmentYears)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
            // Add current year if not present
            var currentYear = DateTime.Now.Year;
            if (!allYears.Contains(currentYear))
            {
                allYears.Insert(0, currentYear);
            }

            // Cache the result for 10 minutes
            _cacheService.Set(cacheKey, allYears, TimeSpan.FromMinutes(10));
            _logger.LogInformation($"Cache set {cacheKey}");
            return allYears;
        }
    }
}
