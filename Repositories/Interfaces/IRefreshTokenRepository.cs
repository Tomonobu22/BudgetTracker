using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
