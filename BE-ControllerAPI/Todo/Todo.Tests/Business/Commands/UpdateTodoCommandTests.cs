using Moq;
using Todo.Business.Features.Todos.Commands;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Tests.Business.Commands
{
    [TestFixture]
    public class UpdateTodoCommandTests
    {
        private Mock<ITodoRepository> _mockRepository;
        private UpdateTodoCommand.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _handler = new UpdateTodoCommand.Handler(_mockRepository.Object);
        }

        [Test]
        public async Task Handle_WithExistingTodo_ShouldUpdateAndReturnUpdatedTodo()
        {
            // Arrange
            var command = new UpdateTodoCommand.Command 
            { 
                Id = 1, 
                Title = "Updated Todo", 
                IsCompleted = true 
            };
            var existingTodo = new TodoItem { Id = 1, Title = "Original Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingTodo);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Updated Todo"));
            Assert.That(result.IsCompleted, Is.True);

            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<TodoItem>(t => 
                t.Id == 1 && t.Title == "Updated Todo" && t.IsCompleted == true)), Times.Once);
        }

        [Test]
        public async Task Handle_WithNonExistentTodo_ShouldReturnNull()
        {
            // Arrange
            var command = new UpdateTodoCommand.Command 
            { 
                Id = 999, 
                Title = "Updated Todo", 
                IsCompleted = true 
            };

            _mockRepository.Setup(r => r.GetByIdAsync(999))
                          .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
            
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
        }

        [Test]
        public async Task Handle_WhenRepositoryUpdateFails_ShouldReturnNull()
        {
            // Arrange
            var command = new UpdateTodoCommand.Command 
            { 
                Id = 1, 
                Title = "Updated Todo", 
                IsCompleted = true 
            };
            var existingTodo = new TodoItem { Id = 1, Title = "Original Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingTodo);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
            
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
        }

        [Test]
        public async Task Handle_WithEmptyTitle_ShouldUpdateWithEmptyTitle()
        {
            // Arrange
            var command = new UpdateTodoCommand.Command 
            { 
                Id = 1, 
                Title = "", 
                IsCompleted = true 
            };
            var existingTodo = new TodoItem { Id = 1, Title = "Original Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingTodo);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result!.Title, Is.EqualTo(""));
        }

        [Test]
        public async Task Handle_WithNullTitle_ShouldUpdateWithNullTitle()
        {
            // Arrange
            var command = new UpdateTodoCommand.Command 
            { 
                Id = 1, 
                Title = null!, 
                IsCompleted = true 
            };
            var existingTodo = new TodoItem { Id = 1, Title = "Original Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingTodo);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.Null);
        }

        [Test]
        public async Task Handle_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var command = new UpdateTodoCommand.Command 
            { 
                Id = 1, 
                Title = "New Title", 
                IsCompleted = false 
            };
            var existingTodo = new TodoItem { Id = 1, Title = "Original Todo", IsCompleted = true };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingTodo);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result!.Title, Is.EqualTo("New Title"));
            Assert.That(result.IsCompleted, Is.False);
        }

        [Test]
        public async Task Handle_WithLongTitle_ShouldHandleCorrectly()
        {
            // Arrange
            var longTitle = new string('A', 1000);
            var command = new UpdateTodoCommand.Command 
            { 
                Id = 1, 
                Title = longTitle, 
                IsCompleted = true 
            };
            var existingTodo = new TodoItem { Id = 1, Title = "Original Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingTodo);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result!.Title, Is.EqualTo(longTitle));
        }

        [Test]
        public async Task Handle_ShouldPassCancellationToken()
        {
            // Arrange
            var command = new UpdateTodoCommand.Command 
            { 
                Id = 1, 
                Title = "Updated Todo", 
                IsCompleted = true 
            };
            var existingTodo = new TodoItem { Id = 1, Title = "Original Todo", IsCompleted = false };
            var cancellationToken = new CancellationToken();

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingTodo);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
        }
    }
}
