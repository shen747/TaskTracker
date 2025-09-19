using MediatR;
using Serilog;
using Todo.Contracts.Interfaces;
using Todo.Contracts.Models;

namespace Todo.Business.Features.Todos.Commands
{
    public static class AddTodoCommand
    {
        public class Command : IRequest<TodoItem>
        {
            public required string Title { get; set; }
        }

        public class Handler : IRequestHandler<Command, TodoItem>
        {
            private readonly ITodoRepository _repository;

            public Handler(ITodoRepository repository)
            {
                _repository = repository;
            }

            public async Task<TodoItem> Handle(Command request, CancellationToken cancellationToken)
            {
                Log.Information("Adding new todo with title: {TodoTitle}", request.Title);
                
                try
                {
                    var newTodo = new TodoItem
                    {
                        Title = request.Title,
                        IsCompleted = false
                    };

                    var id = await _repository.AddAsync(newTodo);
                    Log.Information("Todo added to repository with ID: {TodoId}", id);
                    
                    var addedTodo = await _repository.GetByIdAsync(id);
                    
                    if (addedTodo == null)
                    {
                        Log.Error("Failed to retrieve added todo with ID: {TodoId}", id);
                        throw new Exception("Failed to add todo item.");
                    }

                    Log.Information("Successfully added todo {TodoId}: {TodoTitle}", addedTodo.Id, addedTodo.Title);
                    return addedTodo;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occurred while adding todo with title: {TodoTitle}", request.Title);
                    throw;
                }
            }
        }
    }
}
