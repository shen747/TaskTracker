using MediatR;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Business.Features.Todos.Queries
{
    public static class GetTodoByIdQuery
    {
        public class Query : IRequest<TodoItem?>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, TodoItem?>
        {
            private readonly ITodoRepository _repository;

            public Handler(ITodoRepository repository)
            {
                _repository = repository;
            }

            public async Task<TodoItem?> Handle(Query request, CancellationToken cancellationToken)
            {
                var todo = await _repository.GetByIdAsync(request.Id);
                return todo;
            }
        }
    }
}
