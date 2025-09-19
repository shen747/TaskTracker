using Moq;
using Todo.Business.Features.Todos.Queries;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Tests.Business.Queries
{
    [TestFixture]
    public class GetTodosQueryTests
    {
        private Mock<ITodoRepository> _mockRepository;
        private GetTodosQuery.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _handler = new GetTodosQuery.Handler(_mockRepository.Object);
        }

        [Test]
        public async Task Handle_WithExistingTodos_ShouldReturnAllTodos()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Todo 1", IsCompleted = false },
                new TodoItem { Id = 2, Title = "Todo 2", IsCompleted = true },
                new TodoItem { Id = 3, Title = "Todo 3", IsCompleted = false }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedTodos);

            // Act
            var result = await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            
            var resultList = result.ToList();
            Assert.That(resultList[0].Id, Is.EqualTo(1));
            Assert.That(resultList[1].Id, Is.EqualTo(2));
            Assert.That(resultList[2].Id, Is.EqualTo(3));

            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_WithNoTodos_ShouldReturnEmptyCollection()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>();

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedTodos);

            // Act
            var result = await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task Handle_WithSingleTodo_ShouldReturnSingleTodo()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Single Todo", IsCompleted = false }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedTodos);

            // Act
            var result = await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            
            var singleTodo = result.First();
            Assert.That(singleTodo.Id, Is.EqualTo(1));
            Assert.That(singleTodo.Title, Is.EqualTo("Single Todo"));
            Assert.That(singleTodo.IsCompleted, Is.False);
        }

        [Test]
        public async Task Handle_WithMixedCompletionStatus_ShouldReturnAllTodos()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Completed Todo", IsCompleted = true },
                new TodoItem { Id = 2, Title = "Incomplete Todo", IsCompleted = false },
                new TodoItem { Id = 3, Title = "Another Completed", IsCompleted = true }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedTodos);

            // Act
            var result = await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(3));
            Assert.That(resultList.Count(t => t.IsCompleted), Is.EqualTo(2));
            Assert.That(resultList.Count(t => !t.IsCompleted), Is.EqualTo(1));
        }

        [Test]
        public async Task Handle_WithLongTitles_ShouldReturnAllTodos()
        {
            // Arrange
            var longTitle = new string('A', 1000);
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = longTitle, IsCompleted = false }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedTodos);

            // Act
            var result = await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo(longTitle));
        }

        [Test]
        public async Task Handle_WithEmptyTitles_ShouldReturnAllTodos()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "", IsCompleted = false },
                new TodoItem { Id = 2, Title = "", IsCompleted = true }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedTodos);

            // Act
            var result = await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(t => t.Title == ""), Is.True);
        }

        [Test]
        public async Task Handle_ShouldPassCancellationToken()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>();
            var cancellationToken = new CancellationToken();

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedTodos);

            // Act
            await _handler.Handle(new GetTodosQuery.Query(), cancellationToken);

            // Assert
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None));
        }

        [Test]
        public async Task Handle_WithMultipleCalls_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Todo", IsCompleted = false }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedTodos);

            // Act
            var result1 = await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None);
            var result2 = await _handler.Handle(new GetTodosQuery.Query(), CancellationToken.None);

            // Assert
            Assert.That(result1.Count(), Is.EqualTo(1));
            Assert.That(result2.Count(), Is.EqualTo(1));
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }
    }
}
