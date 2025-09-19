using MediatR;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Business.Features.Todos.Commands
{
    public static class UpdateTodoCommand
    {
        public class Command : IRequest<TodoItem?>
        {
            public int Id { get; set; }
            public required string Title { get; set; }
            public bool IsCompleted { get; set; }
        }

        public class Handler : IRequestHandler<Command, TodoItem?>
        {
            private readonly ITodoRepository _repository;

            public Handler(ITodoRepository repository)
            {
                _repository = repository;
            }

            public async Task<TodoItem?> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingTodo = await _repository.GetByIdAsync(request.Id);
                if (existingTodo == null)
                {
                    return null;
                }

                var updatedTodo = new TodoItem
                {
                    Id = request.Id,
                    Title = request.Title,
                    IsCompleted = request.IsCompleted
                };

                var success = await _repository.UpdateAsync(updatedTodo);
                if (!success)
                {
                    return null;
                }

                return updatedTodo;
            }
        }
    }
}
