using MediatR;
using Serilog;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Business.Features.Todos.Queries
{
    public static class GetTodosQuery
    {
        public class Query : IRequest<IEnumerable<TodoItem>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<TodoItem>>
        {
            private readonly ITodoRepository _repository;

            public Handler(ITodoRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<TodoItem>> Handle(Query request, CancellationToken cancellationToken)
            {
                Log.Information("Getting all todos from repository");
                
                try
                {
                    var todos = await _repository.GetAllAsync();
                    Log.Information("Retrieved {TodoCount} todos from repository", todos.Count());
                    return todos;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occurred while retrieving todos from repository");
                    throw;
                }
            }
        }
    }
}
