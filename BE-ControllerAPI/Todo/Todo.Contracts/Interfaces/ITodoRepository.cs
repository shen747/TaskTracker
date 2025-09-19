using Todo.Contracts.Models;

namespace Todo.Contracts.Interfaces
{
    public interface ITodoRepository
    {
        Task<int> AddAsync(TodoItem item);
        Task<TodoItem?> GetByIdAsync(int id);
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(TodoItem item);
    }
}
