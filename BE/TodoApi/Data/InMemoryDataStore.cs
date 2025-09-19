using System.Collections.Concurrent;
using TodoApi.Domain;

namespace TodoApi.Data
{
    public class InMemoryDataStore
    {
        public readonly ConcurrentDictionary<int, TodoItem> _todos = new();
        private int _currentId = 0;

        public InMemoryDataStore()
        {
            // Seed with some initial data            
            Add(new TodoItem { Title = "Build a REST API", IsCompleted = false });
            Add(new TodoItem { Title = "Build the Angular Front End", IsCompleted = false });
            Add(new TodoItem { Title = "Write Unit tests", IsCompleted = false });
        }

        public int Add(TodoItem item)
        {
            var id = Interlocked.Increment(ref _currentId);
            item.Id = id;
            _todos[id] = item;
            return id;
        }

        public TodoItem? Get(int id)
        {
            return _todos.GetValueOrDefault(id);
        }

        public IEnumerable<TodoItem> GetAll()
        {
            return _todos.Values.OrderBy(t=>t.Id);
        }

        public bool Delete(int id)
        {
            Console.WriteLine($"Attempting to delete todo with ID: {id}");
            Console.WriteLine($"Current todos: {string.Join(", ", _todos.Keys)}");
            var result = _todos.TryRemove(id, out _);
            Console.WriteLine($"Delete result for ID {id}: {result}");
            return result;
        }

        public bool Update(TodoItem item)
        {
            Console.WriteLine($"Attempting to update todo with ID: {item.Id}");
            Console.WriteLine($"Current todos: {string.Join(", ", _todos.Keys)}");
            
            if (!_todos.ContainsKey(item.Id))
            {
                Console.WriteLine($"Todo with ID {item.Id} not found for update");
                return false;
            }

            _todos[item.Id] = item;
            Console.WriteLine($"Successfully updated todo with ID {item.Id}");
            return true;
        }
    }
}
