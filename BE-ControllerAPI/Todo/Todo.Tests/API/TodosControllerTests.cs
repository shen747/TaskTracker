using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Todo.API.Controllers;
using Todo.Business.Features.Todos.Commands;
using Todo.Business.Features.Todos.Queries;
using Todo.Contracts.DTOs;
using Todo.Contracts.Models;

namespace Todo.Tests.API
{
    [TestFixture]
    public class TodosControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private TodosController _controller;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new TodosController(_mockMediator.Object);
        }

        [Test]
        public async Task GetTodos_ShouldReturnOkWithTodos()
        {
            // Arrange
            var todos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Todo 1", IsCompleted = false },
                new TodoItem { Id = 2, Title = "Todo 2", IsCompleted = true }
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetTodosQuery.Query>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(todos);

            // Act
            var result = await _controller.GetTodos();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(todos));
        }

        [Test]
        public async Task GetTodo_WithExistingId_ShouldReturnOkWithTodo()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetTodoByIdQuery.Query>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(todo);

            // Act
            var result = await _controller.GetTodo(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(todo));
        }

        [Test]
        public async Task GetTodo_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<GetTodoByIdQuery.Query>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.GetTodo(999);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CreateTodo_WithValidRequest_ShouldReturnCreated()
        {
            // Arrange
            var request = new AddTodoRequest { Title = "New Todo" };
            var createdTodo = new TodoItem { Id = 1, Title = "New Todo", IsCompleted = false };

            _mockMediator.Setup(m => m.Send(It.IsAny<AddTodoCommand.Command>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(createdTodo);

            // Act
            var result = await _controller.CreateTodo(request);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult!.Value, Is.EqualTo(createdTodo));
        }

        [Test]
        public async Task CreateTodo_WithEmptyTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new AddTodoRequest { Title = "" };

            // Act
            var result = await _controller.CreateTodo(request);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateTodo_WithNullTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new AddTodoRequest { Title = null! };

            // Act
            var result = await _controller.CreateTodo(request);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateTodo_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = new UpdateTodoRequest { Title = "Updated Todo", IsCompleted = true };
            var updatedTodo = new TodoItem { Id = 1, Title = "Updated Todo", IsCompleted = true };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateTodoCommand.Command>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(updatedTodo);

            // Act
            var result = await _controller.UpdateTodo(1, request);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(updatedTodo));
        }

        [Test]
        public async Task UpdateTodo_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            var request = new UpdateTodoRequest { Title = "Updated Todo", IsCompleted = true };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateTodoCommand.Command>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.UpdateTodo(999, request);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateTodo_WithEmptyTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new UpdateTodoRequest { Title = "", IsCompleted = true };

            // Act
            var result = await _controller.UpdateTodo(1, request);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteTodo_WithExistingId_ShouldReturnNoContent()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteTodoCommand.Command>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTodo(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteTodo_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteTodoCommand.Command>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTodo(999);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Controller_ShouldUseMediatRCorrectly()
        {
            // Arrange
            var request = new AddTodoRequest { Title = "Test Todo" };
            var createdTodo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };

            _mockMediator.Setup(m => m.Send(It.IsAny<AddTodoCommand.Command>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(createdTodo);

            // Act
            await _controller.CreateTodo(request);

            // Assert
            _mockMediator.Verify(m => m.Send(
                It.Is<AddTodoCommand.Command>(c => c.Title == "Test Todo"), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateTodo_ShouldSetIdFromRoute()
        {
            // Arrange
            var request = new UpdateTodoRequest { Title = "Updated Todo", IsCompleted = true };
            var updatedTodo = new TodoItem { Id = 123, Title = "Updated Todo", IsCompleted = true };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateTodoCommand.Command>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(updatedTodo);

            // Act
            await _controller.UpdateTodo(123, request);

            // Assert
            _mockMediator.Verify(m => m.Send(
                It.Is<UpdateTodoCommand.Command>(c => c.Id == 123), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
