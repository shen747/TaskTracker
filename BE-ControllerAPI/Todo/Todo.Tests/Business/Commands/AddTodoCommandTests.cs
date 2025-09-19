using Moq;
using Todo.Business.Features.Todos.Commands;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Tests.Business.Commands
{
    [TestFixture]
    public class AddTodoCommandTests
    {
        private Mock<ITodoRepository> _mockRepository;
        private AddTodoCommand.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _handler = new AddTodoCommand.Handler(_mockRepository.Object);
        }

        [Test]
        public async Task Handle_WithValidCommand_ShouldAddTodoAndReturnIt()
        {
            // Arrange
            var command = new AddTodoCommand.Command { Title = "Test Todo" };
            var expectedTodo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(1);
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Test Todo"));
            Assert.That(result.IsCompleted, Is.False);

            _mockRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(t => 
                t.Title == "Test Todo" && t.IsCompleted == false)), Times.Once);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task Handle_WithEmptyTitle_ShouldStillAddTodo()
        {
            // Arrange
            var command = new AddTodoCommand.Command { Title = "" };
            var expectedTodo = new TodoItem { Id = 1, Title = "", IsCompleted = false };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(1);
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(""));
        }

        [Test]
        public async Task Handle_WithNullTitle_ShouldThrowException()
        {
            // Arrange
            var command = new AddTodoCommand.Command { Title = null! };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => 
                await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task Handle_WhenRepositoryFailsToAdd_ShouldThrowException()
        {
            // Arrange
            var command = new AddTodoCommand.Command { Title = "Test Todo" };
            var expectedTodo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(1);
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync((TodoItem?)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => 
                await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task Handle_ShouldSetIsCompletedToFalseByDefault()
        {
            // Arrange
            var command = new AddTodoCommand.Command { Title = "Test Todo" };
            var expectedTodo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(1);
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsCompleted, Is.False);
            
            _mockRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(t => 
                t.IsCompleted == false)), Times.Once);
        }

        [Test]
        public async Task Handle_WithLongTitle_ShouldHandleCorrectly()
        {
            // Arrange
            var longTitle = new string('A', 1000);
            var command = new AddTodoCommand.Command { Title = longTitle };
            var expectedTodo = new TodoItem { Id = 1, Title = longTitle, IsCompleted = false };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(1);
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Title, Is.EqualTo(longTitle));
        }

        [Test]
        public async Task Handle_ShouldPassCancellationToken()
        {
            // Arrange
            var command = new AddTodoCommand.Command { Title = "Test Todo" };
            var expectedTodo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };
            var cancellationToken = new CancellationToken();

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                          .ReturnsAsync(1);
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Once);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }
    }
}
