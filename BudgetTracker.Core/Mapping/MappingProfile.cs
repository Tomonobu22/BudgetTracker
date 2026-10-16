using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;
using AutoMapper;

namespace BudgetTracker.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Income, IncomeDto>();
            CreateMap<IncomeDto, Income>()
                .ForMember(dest => dest.Tag, opt => opt.Ignore());
            CreateMap<Expense, ExpenseDto>();
            CreateMap<ExpenseDto, Expense>()
                .ForMember(dest => dest.Tag, opt => opt.Ignore());
            CreateMap<Investment, InvestmentDto>();
            CreateMap<InvestmentDto, Investment>()
                .ForMember(dest => dest.Tag, opt => opt.Ignore())
                .ForMember(dest => dest.StockPrice, opt => opt.Ignore());
            CreateMap<StockPrice, StockPriceDto>();
            CreateMap<StockPriceDto, StockPrice>();
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<Import, ImportDto>().ReverseMap();
        }
    }
}
