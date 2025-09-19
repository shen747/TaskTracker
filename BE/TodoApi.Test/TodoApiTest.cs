using NUnit.Framework;
using TodoApi.Data;
using TodoApi.Domain;
using TodoApi.Features.Todos.Commands;
using TodoApi.Features.Todos.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace TodoApi.Test
{
    public class Tests
    {
        private InMemoryDataStore _store;

        [SetUp]
        public void Setup()
        {
            _store = new InMemoryDataStore();
        }

        [Test]
        public async Task GetTodos_ReturnsSeededTodos()
        {
            // ARRANGE
            var handler = new GetTodos.Handler(_store);

            // ACT
            var result = await handler.Handle(new GetTodos.Query(), CancellationToken.None);

            // ASSERT
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(t => t.Title == "Build a REST API"), Is.True);
        }

        [Test]
        public async Task AddTodo_AddsNewTodo()
        {
            // ARRANGE
            var handler = new AddTodo.Command.Handler(_store);
            var command = new AddTodo.Command { Title = "Test new todo" };

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Title, Is.EqualTo("Test new todo"));
            Assert.That(_store.GetAll().Any(t => t.Title == "Test new todo"), Is.True);
        }

        [Test]
        public async Task DeleteTodo_RemovesTodo()
        {
            // ARRANGE
            var addHandler = new AddTodo.Command.Handler(_store);
            var newTodo = await addHandler.Handle(new AddTodo.Command { Title = "To be deleted" }, CancellationToken.None);
            var deleteHandler = new DeleteTodo.Handler(_store);

            // ACT
            var deleteResult = await deleteHandler.Handle(new DeleteTodo.Command { Id = newTodo.Id }, CancellationToken.None);

            // ASSERT
            Assert.That(deleteResult, Is.True);
            Assert.That(_store.Get(newTodo.Id), Is.Null);
        }

        [Test]
        public async Task DeleteTodo_ReturnsFalse_WhenNotFound()
        {
            // ARRANGE
            var handler = new DeleteTodo.Handler(_store);

            // ACT
            var result = await handler.Handle(new DeleteTodo.Command { Id = 9999 }, CancellationToken.None);

            // ASSERT
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateTodo_UpdatesExistingTodo()
        {
            // ARRANGE
            var addHandler = new AddTodo.Command.Handler(_store);
            var newTodo = await addHandler.Handle(new AddTodo.Command { Title = "Original Title" }, CancellationToken.None);
            var updateHandler = new UpdateTodo.Handler(_store);
            var updateCommand = new UpdateTodo.Command
            {
                Id = newTodo.Id,
                Title = "Updated Title",
                IsCompleted = true
            };

            // ACT
            var result = await updateHandler.Handle(updateCommand, CancellationToken.None);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(newTodo.Id));
            Assert.That(result.Title, Is.EqualTo("Updated Title"));
            Assert.That(result.IsCompleted, Is.True);

            var storedTodo = _store.Get(newTodo.Id);
            Assert.That(storedTodo.Title, Is.EqualTo("Updated Title"));
            Assert.That(storedTodo.IsCompleted, Is.True);
        }

        [Test]
        public async Task UpdateTodo_ReturnsNull_WhenTodoNotFound()
        {
            // ARRANGE
            var handler = new UpdateTodo.Handler(_store);
            var command = new UpdateTodo.Command
            {
                Id = 9999,
                Title = "Non-existent Todo",
                IsCompleted = false
            };

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateTodo_UpdatesOnlySpecifiedFields()
        {
            // ARRANGE
            var addHandler = new AddTodo.Command.Handler(_store);
            var newTodo = await addHandler.Handle(new AddTodo.Command { Title = "Initial Title" }, CancellationToken.None);
            var updateHandler = new UpdateTodo.Handler(_store);
            var updateCommand = new UpdateTodo.Command
            {
                Id = newTodo.Id,
                Title = "Updated Title Only",
                IsCompleted = false
            };

            // ACT
            var result = await updateHandler.Handle(updateCommand, CancellationToken.None);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Updated Title Only"));
            Assert.That(result.IsCompleted, Is.False);
            Assert.That(result.Id, Is.EqualTo(newTodo.Id));
        }

        [Test]
        public async Task UpdateTodo_CanMarkTodoAsCompleted()
        {
            // ARRANGE
            var addHandler = new AddTodo.Command.Handler(_store);
            var newTodo = await addHandler.Handle(new AddTodo.Command { Title = "Todo to Complete" }, CancellationToken.None);
            var updateHandler = new UpdateTodo.Handler(_store);
            var updateCommand = new UpdateTodo.Command
            {
                Id = newTodo.Id,
                Title = newTodo.Title,
                IsCompleted = true
            };

            // ACT
            var result = await updateHandler.Handle(updateCommand, CancellationToken.None);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsCompleted, Is.True);
            Assert.That(result.Title, Is.EqualTo("Todo to Complete"));
        }
    }
}