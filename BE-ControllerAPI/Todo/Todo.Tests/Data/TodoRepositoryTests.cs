using Todo.Contracts.Models;

namespace Todo.Tests.Data
{
    [TestFixture]
    public class TodoRepositoryTests
    {
        private Todo.Data.Repositories.TodoRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new Todo.Data.Repositories.TodoRepository();
        }

        [Test]
        public async Task AddAsync_WithValidTodo_ShouldAddTodoAndReturnId()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo", IsCompleted = false };

            // Act
            var id = await _repository.AddAsync(todo);

            // Assert
            Assert.That(id, Is.GreaterThan(0));
            Assert.That(todo.Id, Is.EqualTo(id));
        }

        [Test]
        public async Task AddAsync_WithValidTodo_ShouldIncrementIdCorrectly()
        {
            // Arrange
            var todo1 = new TodoItem { Title = "Todo 1", IsCompleted = false };
            var todo2 = new TodoItem { Title = "Todo 2", IsCompleted = false };

            // Act
            var id1 = await _repository.AddAsync(todo1);
            var id2 = await _repository.AddAsync(todo2);

            // Assert
            Assert.That(id2, Is.EqualTo(id1 + 1));
        }

        [Test]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnCorrectTodo()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo", IsCompleted = false };
            var id = await _repository.AddAsync(todo);

            // Act
            var retrievedTodo = await _repository.GetByIdAsync(id);

            // Assert
            Assert.That(retrievedTodo, Is.Not.Null);
            Assert.That(retrievedTodo!.Id, Is.EqualTo(id));
            Assert.That(retrievedTodo.Title, Is.EqualTo("Test Todo"));
            Assert.That(retrievedTodo.IsCompleted, Is.False);
        }

        [Test]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Act
            var retrievedTodo = await _repository.GetByIdAsync(999);

            // Assert
            Assert.That(retrievedTodo, Is.Null);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllTodosInOrder()
        {
            // Arrange
            var todo1 = new TodoItem { Title = "Todo 1", IsCompleted = false };
            var todo2 = new TodoItem { Title = "Todo 2", IsCompleted = true };
            
            await _repository.AddAsync(todo1);
            await _repository.AddAsync(todo2);

            // Act
            var todos = await _repository.GetAllAsync();

            // Assert
            Assert.That(todos, Is.Not.Null);
            Assert.That(todos.Count(), Is.GreaterThanOrEqualTo(2));
            
            var todoList = todos.ToList();
            Assert.That(todoList[0].Id, Is.LessThan(todoList[1].Id));
        }

        [Test]
        public async Task DeleteAsync_WithExistingId_ShouldRemoveTodoAndReturnTrue()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo", IsCompleted = false };
            var id = await _repository.AddAsync(todo);

            // Act
            var result = await _repository.DeleteAsync(id);

            // Assert
            Assert.That(result, Is.True);
            var deletedTodo = await _repository.GetByIdAsync(id);
            Assert.That(deletedTodo, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WithNonExistentId_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateAsync_WithExistingTodo_ShouldUpdateAndReturnTrue()
        {
            // Arrange
            var todo = new TodoItem { Title = "Original Todo", IsCompleted = false };
            var id = await _repository.AddAsync(todo);

            // Act
            var updatedTodo = new TodoItem { Id = id, Title = "Updated Todo", IsCompleted = true };
            var result = await _repository.UpdateAsync(updatedTodo);

            // Assert
            Assert.That(result, Is.True);
            var retrievedTodo = await _repository.GetByIdAsync(id);
            Assert.That(retrievedTodo!.Title, Is.EqualTo("Updated Todo"));
            Assert.That(retrievedTodo.IsCompleted, Is.True);
        }

        [Test]
        public async Task UpdateAsync_WithNonExistentTodo_ShouldReturnFalse()
        {
            // Arrange
            var updatedTodo = new TodoItem { Id = 999, Title = "Updated Todo", IsCompleted = true };

            // Act
            var result = await _repository.UpdateAsync(updatedTodo);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task Repository_ShouldInitializeWithSeedData()
        {
            // Act
            var todos = await _repository.GetAllAsync();

            // Assert
            Assert.That(todos.Count(), Is.GreaterThanOrEqualTo(3));
            
            var todoList = todos.ToList();
            Assert.That(todoList.Any(t => t.Title.Contains("Build a REST API")), Is.True);
            Assert.That(todoList.Any(t => t.Title.Contains("Build the Angular Front End")), Is.True);
            Assert.That(todoList.Any(t => t.Title.Contains("Write Unit tests")), Is.True);
        }

        [Test]
        public async Task Repository_ShouldHandleConcurrentAccess()
        {
            // Arrange
            var tasks = new List<Task<int>>();
            
            // Act - Add multiple todos concurrently
            for (int i = 0; i < 10; i++)
            {
                var todo = new TodoItem { Title = $"Concurrent Todo {i}", IsCompleted = false };
                tasks.Add(_repository.AddAsync(todo));
            }
            
            var results = await Task.WhenAll(tasks);

            // Assert - All IDs should be unique and sequential
            Assert.That(results, Is.Unique);
            Assert.That(results.Max() - results.Min(), Is.EqualTo(9));
        }
    }
}
