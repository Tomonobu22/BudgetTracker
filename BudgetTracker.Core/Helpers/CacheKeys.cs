using BudgetTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Core.Helpers
{
    public static class CacheKeys
    {
        public static string GetMonthlySummaryKey(string userId, int year) => $"MonthlySummary_{userId}_{year}";
        public static string AvailableYearsKey(string userId) => $"AvailableYears_{userId}";
        public static string GetTagsByContextKey(string userId, RecordType context) => $"Tags_{userId}_{context}";
    }
}
