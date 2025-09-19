using MediatR;
using TodoApi.Domain;
using TodoApi.Data;

namespace TodoApi.Features.Todos.Commands
{
    public class UpdateTodo
    {
        public class Command : IRequest<TodoItem?>
        {
            public int Id { get; set; }
            public required string Title { get; set; }
            public bool IsCompleted { get; set; }
        }

        public class Handler(InMemoryDataStore store) : IRequestHandler<Command, TodoItem?>
        {
            private readonly InMemoryDataStore _store = store;

            public Task<TodoItem?> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingTodo = _store.Get(request.Id);
                if (existingTodo == null)
                {
                    return Task.FromResult<TodoItem?>(null);
                }

                var updatedTodo = new TodoItem
                {
                    Id = request.Id,
                    Title = request.Title,
                    IsCompleted = request.IsCompleted
                };

                var success = _store.Update(updatedTodo);
                if (!success)
                {
                    return Task.FromResult<TodoItem?>(null);
                }

                return Task.FromResult<TodoItem?>(updatedTodo);
            }
        }
    }
}
