using MediatR;
using TodoApi.Data;
using TodoApi.Domain;

namespace TodoApi.Features.Todos.Queries
{
    public static class GetTodos
    {
        public class Query : IRequest<IEnumerable<TodoItem>> { }
        public class Handler : IRequestHandler<Query, IEnumerable<TodoItem>>
        {
            private readonly InMemoryDataStore _store;
            public Handler(InMemoryDataStore store)
            {
                _store = store;
            }
            public Task<IEnumerable<TodoItem>> Handle(Query request, CancellationToken cancellationToken)
            {
                var todos = _store.GetAll();
                return Task.FromResult(todos);
            }

        }
    }
}
