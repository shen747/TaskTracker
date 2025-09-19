using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Todo.Business.Features.Todos.Commands;
using Todo.Business.Features.Todos.Queries;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;
using Todo.Data.Repositories;

namespace Todo.Tests.Integration
{
    [TestFixture]
    public class TodoIntegrationTests
    {
        private ServiceProvider _serviceProvider;
        private IMediator _mediator;
        private ITodoRepository _repository;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetTodosQuery).Assembly));
            services.AddSingleton<ITodoRepository, TodoRepository>();
            
            _serviceProvider = services.BuildServiceProvider();
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
            _repository = _serviceProvider.GetRequiredService<ITodoRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider?.Dispose();
        }

        [Test]
        public async Task FullCrudFlow_ShouldWorkCorrectly()
        {
            // Create
            var createCommand = new AddTodoCommand.Command { Title = "Integration Test Todo" };
            var createdTodo = await _mediator.Send(createCommand);
            
            Assert.That(createdTodo, Is.Not.Null);
            Assert.That(createdTodo.Title, Is.EqualTo("Integration Test Todo"));
            Assert.That(createdTodo.IsCompleted, Is.False);
            Assert.That(createdTodo.Id, Is.GreaterThan(0));

            // Read by ID
            var getByIdQuery = new GetTodoByIdQuery.Query { Id = createdTodo.Id };
            var retrievedTodo = await _mediator.Send(getByIdQuery);
            
            Assert.That(retrievedTodo, Is.Not.Null);
            Assert.That(retrievedTodo!.Id, Is.EqualTo(createdTodo.Id));
            Assert.That(retrievedTodo.Title, Is.EqualTo(createdTodo.Title));

            // Read all
            var getAllQuery = new GetTodosQuery.Query();
            var allTodos = await _mediator.Send(getAllQuery);
            
            Assert.That(allTodos, Is.Not.Null);
            Assert.That(allTodos.Any(t => t.Id == createdTodo.Id), Is.True);

            // Update
            var updateCommand = new UpdateTodoCommand.Command 
            { 
                Id = createdTodo.Id, 
                Title = "Updated Integration Test Todo", 
                IsCompleted = true 
            };
            var updatedTodo = await _mediator.Send(updateCommand);
            
            Assert.That(updatedTodo, Is.Not.Null);
            Assert.That(updatedTodo!.Title, Is.EqualTo("Updated Integration Test Todo"));
            Assert.That(updatedTodo.IsCompleted, Is.True);

            // Verify update
            var verifyQuery = new GetTodoByIdQuery.Query { Id = createdTodo.Id };
            var verifiedTodo = await _mediator.Send(verifyQuery);
            
            Assert.That(verifiedTodo!.Title, Is.EqualTo("Updated Integration Test Todo"));
            Assert.That(verifiedTodo.IsCompleted, Is.True);

            // Delete
            var deleteCommand = new DeleteTodoCommand.Command { Id = createdTodo.Id };
            var deleteResult = await _mediator.Send(deleteCommand);
            
            Assert.That(deleteResult, Is.True);

            // Verify deletion
            var deletedQuery = new GetTodoByIdQuery.Query { Id = createdTodo.Id };
            var deletedTodo = await _mediator.Send(deletedQuery);
            
            Assert.That(deletedTodo, Is.Null);
        }

        [Test]
        public async Task CreateMultipleTodos_ShouldWorkCorrectly()
        {
            // Arrange
            var todoTitles = new[] { "Todo 1", "Todo 2", "Todo 3" };
            var createdTodos = new List<TodoItem>();

            // Act - Create multiple todos
            foreach (var title in todoTitles)
            {
                var command = new AddTodoCommand.Command { Title = title };
                var todo = await _mediator.Send(command);
                createdTodos.Add(todo);
            }

            // Assert - Verify all were created
            Assert.That(createdTodos.Count, Is.EqualTo(3));
            Assert.That(createdTodos.All(t => t.Id > 0), Is.True);
            Assert.That(createdTodos.Select(t => t.Title), Is.EquivalentTo(todoTitles));

            // Verify all can be retrieved
            var getAllQuery = new GetTodosQuery.Query();
            var allTodos = await _mediator.Send(getAllQuery);
            
            foreach (var createdTodo in createdTodos)
            {
                Assert.That(allTodos.Any(t => t.Id == createdTodo.Id), Is.True);
            }
        }

        [Test]
        public async Task UpdateNonExistentTodo_ShouldReturnNull()
        {
            // Arrange
            var updateCommand = new UpdateTodoCommand.Command 
            { 
                Id = 999, 
                Title = "Non-existent Todo", 
                IsCompleted = true 
            };

            // Act
            var result = await _mediator.Send(updateCommand);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteNonExistentTodo_ShouldReturnFalse()
        {
            // Arrange
            var deleteCommand = new DeleteTodoCommand.Command { Id = 999 };

            // Act
            var result = await _mediator.Send(deleteCommand);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetNonExistentTodo_ShouldReturnNull()
        {
            // Arrange
            var getQuery = new GetTodoByIdQuery.Query { Id = 999 };

            // Act
            var result = await _mediator.Send(getQuery);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task RepositoryAndMediatR_ShouldBeProperlyRegistered()
        {
            // Assert
            Assert.That(_mediator, Is.Not.Null);
            Assert.That(_repository, Is.Not.Null);
            Assert.That(_repository, Is.InstanceOf<TodoRepository>());
        }

        [Test]
        public async Task ConcurrentOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var tasks = new List<Task<TodoItem>>();

            // Act - Create todos concurrently
            for (int i = 0; i < 5; i++)
            {
                var command = new AddTodoCommand.Command { Title = $"Concurrent Todo {i}" };
                tasks.Add(_mediator.Send(command));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - All should be created successfully
            Assert.That(results.Length, Is.EqualTo(5));
            Assert.That(results.All(t => t.Id > 0), Is.True);
            Assert.That(results.Select(t => t.Id).Distinct().Count(), Is.EqualTo(5));

            // Verify all can be retrieved
            var getAllQuery = new GetTodosQuery.Query();
            var allTodos = await _mediator.Send(getAllQuery);
            
            foreach (var result in results)
            {
                Assert.That(allTodos.Any(t => t.Id == result.Id), Is.True);
            }
        }
    }
}
