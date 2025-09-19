using MediatR;
using Todo.Contracts.Interfaces;

namespace Todo.Business.Features.Todos.Commands
{
    public static class DeleteTodoCommand
    {
        public class Command : IRequest<bool>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly ITodoRepository _repository;

            public Handler(ITodoRepository repository)
            {
                _repository = repository;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var success = await _repository.DeleteAsync(request.Id);
                return success;
            }
        }
    }
}
