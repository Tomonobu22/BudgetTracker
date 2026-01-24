using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class AuthenticationAppService : IAuthenticationAppService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IMapper _mapper;

        public AuthenticationAppService(IRefreshTokenRepository refreshTokenRepository, IMapper mapper)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _mapper = mapper;
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _refreshTokenRepository.GetByTokenAsync(token);
        }
    }
}
