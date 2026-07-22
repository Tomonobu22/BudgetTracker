namespace BudgetTracker.Core.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllFromImportIdAsync(int importId, string userId); 
        Task DeleteByImportIdAsync(int importId, string userId);
    }
}
