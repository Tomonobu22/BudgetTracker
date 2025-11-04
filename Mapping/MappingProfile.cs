using BudgetTracker.DTOs;
using BudgetTracker.Models;
using AutoMapper;

namespace BudgetTracker.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Income, IncomeDto>().ReverseMap();
            CreateMap<Expense, ExpenseDto>().ReverseMap();
            CreateMap<Investment, InvestmentDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
        }
    }
}
