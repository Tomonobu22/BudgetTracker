using BudgetTracker.DTOs;
using BudgetTracker.Models;
using AutoMapper;

namespace BudgetTracker.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Income, IncomeDto>();
            CreateMap<IncomeDto, Income>();
        }
    }
}
