using System.Collections.Concurrent;
using Serilog;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Data.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ConcurrentDictionary<int, TodoItem> _todos = new();
        private int _currentId = 0;

        public TodoRepository()
        {
            Log.Information("Initializing TodoRepository with seed data");
            
            // Seed with some initial data
            var initialTodos = new[]
            {
                new TodoItem { Title = "Build a REST API", IsCompleted = false },
                new TodoItem { Title = "Build the Angular Front End", IsCompleted = false },
                new TodoItem { Title = "Write Unit tests", IsCompleted = false }
            };

            foreach (var todo in initialTodos)
            {
                AddAsync(todo).Wait();
            }
            
            Log.Information("TodoRepository initialized with {TodoCount} seed todos", initialTodos.Length);
        }

        public Task<int> AddAsync(TodoItem item)
        {
            var id = Interlocked.Increment(ref _currentId);
            item.Id = id;
            _todos[id] = item;
            Log.Debug("Added todo {TodoId}: {TodoTitle} to repository", id, item.Title);
            return Task.FromResult(id);
        }

        public Task<TodoItem?> GetByIdAsync(int id)
        {
            var todo = _todos.GetValueOrDefault(id);
            Log.Debug("Retrieved todo {TodoId} from repository: {TodoFound}", id, todo != null ? "Found" : "Not Found");
            return Task.FromResult(todo);
        }

        public Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            var todos = _todos.Values.OrderBy(t => t.Id).AsEnumerable();
            Log.Debug("Retrieved {TodoCount} todos from repository", todos.Count());
            return Task.FromResult(todos);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var result = _todos.TryRemove(id, out _);
            Log.Debug("Deleted todo {TodoId} from repository: {TodoDeleted}", id, result ? "Success" : "Not Found");
            return Task.FromResult(result);
        }

        public Task<bool> UpdateAsync(TodoItem item)
        {
            if (!_todos.ContainsKey(item.Id))
            {
                Log.Debug("Attempted to update non-existent todo {TodoId}", item.Id);
                return Task.FromResult(false);
            }

            _todos[item.Id] = item;
            Log.Debug("Updated todo {TodoId}: {TodoTitle} in repository", item.Id, item.Title);
            return Task.FromResult(true);
        }
    }
}
