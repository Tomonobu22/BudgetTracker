using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Core.DTOs
{
    public class PagedResultDto<T>
    {
        public decimal TotalAmount { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = new();
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
