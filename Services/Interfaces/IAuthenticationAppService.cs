using BudgetTracker.DTOs;
using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IAuthenticationAppService
    {
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
    }
}
