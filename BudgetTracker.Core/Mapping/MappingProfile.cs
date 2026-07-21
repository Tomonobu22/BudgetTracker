using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;
using AutoMapper;

namespace BudgetTracker.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Income, IncomeDto>().ReverseMap();
            CreateMap<Expense, ExpenseDto>().ReverseMap();
            CreateMap<Investment, InvestmentDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<Import, ImportDto>().ReverseMap();
        }
    }
}
