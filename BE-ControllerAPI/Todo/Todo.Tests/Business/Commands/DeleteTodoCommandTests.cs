using Moq;
using Todo.Business.Features.Todos.Commands;
using Todo.Contracts.Interfaces;

namespace Todo.Tests.Business.Commands
{
    [TestFixture]
    public class DeleteTodoCommandTests
    {
        private Mock<ITodoRepository> _mockRepository;
        private DeleteTodoCommand.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _handler = new DeleteTodoCommand.Handler(_mockRepository.Object);
        }

        [Test]
        public async Task Handle_WithExistingTodo_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var command = new DeleteTodoCommand.Command { Id = 1 };

            _mockRepository.Setup(r => r.DeleteAsync(1))
                          .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Test]
        public async Task Handle_WithNonExistentTodo_ShouldReturnFalse()
        {
            // Arrange
            var command = new DeleteTodoCommand.Command { Id = 999 };

            _mockRepository.Setup(r => r.DeleteAsync(999))
                          .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _mockRepository.Verify(r => r.DeleteAsync(999), Times.Once);
        }

        [Test]
        public async Task Handle_WithZeroId_ShouldCallRepository()
        {
            // Arrange
            var command = new DeleteTodoCommand.Command { Id = 0 };

            _mockRepository.Setup(r => r.DeleteAsync(0))
                          .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _mockRepository.Verify(r => r.DeleteAsync(0), Times.Once);
        }

        [Test]
        public async Task Handle_WithNegativeId_ShouldCallRepository()
        {
            // Arrange
            var command = new DeleteTodoCommand.Command { Id = -1 };

            _mockRepository.Setup(r => r.DeleteAsync(-1))
                          .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _mockRepository.Verify(r => r.DeleteAsync(-1), Times.Once);
        }

        [Test]
        public async Task Handle_WithLargeId_ShouldCallRepository()
        {
            // Arrange
            var command = new DeleteTodoCommand.Command { Id = int.MaxValue };

            _mockRepository.Setup(r => r.DeleteAsync(int.MaxValue))
                          .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _mockRepository.Verify(r => r.DeleteAsync(int.MaxValue), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldPassCancellationToken()
        {
            // Arrange
            var command = new DeleteTodoCommand.Command { Id = 1 };
            var cancellationToken = new CancellationToken();

            _mockRepository.Setup(r => r.DeleteAsync(1))
                          .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Test]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new DeleteTodoCommand.Command { Id = 1 };

            _mockRepository.Setup(r => r.DeleteAsync(1))
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task Handle_WithMultipleCalls_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var command1 = new DeleteTodoCommand.Command { Id = 1 };
            var command2 = new DeleteTodoCommand.Command { Id = 2 };

            _mockRepository.Setup(r => r.DeleteAsync(1))
                          .ReturnsAsync(true);
            _mockRepository.Setup(r => r.DeleteAsync(2))
                          .ReturnsAsync(true);

            // Act
            var result1 = await _handler.Handle(command1, CancellationToken.None);
            var result2 = await _handler.Handle(command2, CancellationToken.None);

            // Assert
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(2), Times.Once);
        }
    }
}
