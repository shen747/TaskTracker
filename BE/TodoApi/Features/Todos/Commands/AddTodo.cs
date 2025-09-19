using MediatR;
using TodoApi.Domain;

namespace TodoApi.Features.Todos.Commands
{
    public static class AddTodo
    {
        public class Command : IRequest<TodoItem>
        {
            public required string Title { get; set; }

            public class Handler: IRequestHandler<Command, TodoItem>
            {                 private readonly Data.InMemoryDataStore _store;
                public Handler(Data.InMemoryDataStore store)
                {
                    _store = store;
                }
                public Task<TodoItem> Handle(Command request, CancellationToken cancellationToken)
                {
                    var newTodo = new TodoItem
                    {
                        Title = request.Title,
                        IsCompleted = false
                    };
                    var id = _store.Add(newTodo);
                    var addedTodo = _store.Get(id);
                    if (addedTodo == null)
                    {
                        throw new Exception("Failed to add todo item.");
                    }
                    _store.Add(newTodo);
                    return Task.FromResult(addedTodo);
                }
            }
        }


    }
}
