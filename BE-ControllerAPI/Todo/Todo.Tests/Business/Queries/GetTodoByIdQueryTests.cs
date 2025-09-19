using Moq;
using Todo.Business.Features.Todos.Queries;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Tests.Business.Queries
{
    [TestFixture]
    public class GetTodoByIdQueryTests
    {
        private Mock<ITodoRepository> _mockRepository;
        private GetTodoByIdQuery.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _handler = new GetTodoByIdQuery.Handler(_mockRepository.Object);
        }

        [Test]
        public async Task Handle_WithExistingTodo_ShouldReturnTodo()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = 1 };
            var expectedTodo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Test Todo"));
            Assert.That(result.IsCompleted, Is.False);

            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task Handle_WithNonExistentTodo_ShouldReturnNull()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = 999 };

            _mockRepository.Setup(r => r.GetByIdAsync(999))
                          .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Test]
        public async Task Handle_WithZeroId_ShouldCallRepository()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = 0 };

            _mockRepository.Setup(r => r.GetByIdAsync(0))
                          .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
            _mockRepository.Verify(r => r.GetByIdAsync(0), Times.Once);
        }

        [Test]
        public async Task Handle_WithNegativeId_ShouldCallRepository()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = -1 };

            _mockRepository.Setup(r => r.GetByIdAsync(-1))
                          .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
            _mockRepository.Verify(r => r.GetByIdAsync(-1), Times.Once);
        }

        [Test]
        public async Task Handle_WithLargeId_ShouldCallRepository()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = int.MaxValue };

            _mockRepository.Setup(r => r.GetByIdAsync(int.MaxValue))
                          .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
            _mockRepository.Verify(r => r.GetByIdAsync(int.MaxValue), Times.Once);
        }

        [Test]
        public async Task Handle_WithCompletedTodo_ShouldReturnTodo()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = 1 };
            var expectedTodo = new TodoItem { Id = 1, Title = "Completed Todo", IsCompleted = true };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsCompleted, Is.True);
        }

        [Test]
        public async Task Handle_WithEmptyTitle_ShouldReturnTodo()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = 1 };
            var expectedTodo = new TodoItem { Id = 1, Title = "", IsCompleted = false };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo(""));
        }

        [Test]
        public async Task Handle_WithLongTitle_ShouldReturnTodo()
        {
            // Arrange
            var longTitle = new string('A', 1000);
            var query = new GetTodoByIdQuery.Query { Id = 1 };
            var expectedTodo = new TodoItem { Id = 1, Title = longTitle, IsCompleted = false };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo(longTitle));
        }

        [Test]
        public async Task Handle_ShouldPassCancellationToken()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = 1 };
            var expectedTodo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = false };
            var cancellationToken = new CancellationToken();

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo);

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var query = new GetTodoByIdQuery.Query { Id = 1 };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _handler.Handle(query, CancellationToken.None));
        }

        [Test]
        public async Task Handle_WithMultipleCalls_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var query1 = new GetTodoByIdQuery.Query { Id = 1 };
            var query2 = new GetTodoByIdQuery.Query { Id = 2 };
            var expectedTodo1 = new TodoItem { Id = 1, Title = "Todo 1", IsCompleted = false };
            var expectedTodo2 = new TodoItem { Id = 2, Title = "Todo 2", IsCompleted = true };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTodo1);
            _mockRepository.Setup(r => r.GetByIdAsync(2))
                          .ReturnsAsync(expectedTodo2);

            // Act
            var result1 = await _handler.Handle(query1, CancellationToken.None);
            var result2 = await _handler.Handle(query2, CancellationToken.None);

            // Assert
            Assert.That(result1!.Id, Is.EqualTo(1));
            Assert.That(result2!.Id, Is.EqualTo(2));
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.GetByIdAsync(2), Times.Once);
        }
    }
}
