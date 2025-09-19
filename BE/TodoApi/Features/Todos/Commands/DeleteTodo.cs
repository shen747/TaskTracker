using MediatR;
using TodoApi.Data;

namespace TodoApi.Features.Todos.Commands
{
    public class DeleteTodo
    {
        public class Command : IRequest<bool>
        {
            public int Id { get; set; }
        }

        public class Handler(InMemoryDataStore store) : IRequestHandler<Command, bool>
        {
            private readonly InMemoryDataStore _store = store;

            public Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var success = _store.Delete(request.Id);
                return Task.FromResult(success);
            }
        }
    }
}
