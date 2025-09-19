using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Todo.Business.Features.Todos.Commands;
using Todo.Business.Features.Todos.Queries;
using Todo.Contracts.DTOs;

namespace Todo.API.Controllers
{
    /// <summary>
    /// Todo API Version 2.0 - Enhanced CRUD operations with metadata and improved validation
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/todos")]
    [Tags("Todos")]
    public class TodosV2Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public TodosV2Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all todo items with enhanced metadata (V2)
        /// </summary>
        /// <returns>A wrapped response containing todo items and metadata</returns>
        /// <response code="200">Returns the list of todo items with metadata</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        /// GET /api/v2/todos
        /// Returns:
        /// {
        ///   "data": [
        ///     {
        ///       "id": 1,
        ///       "title": "Build a REST API",
        ///       "isCompleted": false
        ///     }
        ///   ],
        ///   "metadata": {
        ///     "version": "2.0",
        ///     "totalCount": 3,
        ///     "timestamp": "2024-01-15T10:30:00Z",
        ///     "apiVersion": "v2"
        ///   }
        /// }
        /// </example>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> GetTodos()
        {
            Log.Information("Getting all todos with metadata - API Version 2.0");
            
            try
            {
                var todos = await _mediator.Send(new GetTodosQuery.Query());
                var todoCount = todos.Count();
                
                var response = new
                {
                    data = todos,
                    metadata = new
                    {
                        version = "2.0",
                        totalCount = todoCount,
                        timestamp = DateTime.UtcNow,
                        apiVersion = "v2"
                    }
                };

                Log.Information("Retrieved {TodoCount} todos with metadata successfully", todoCount);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving todos with metadata");
                throw;
            }
        }

        /// <summary>
        /// Get a specific todo item by ID (V2 - with enhanced response)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTodo(int id)
        {
            var todo = await _mediator.Send(new GetTodoByIdQuery.Query { Id = id });
            
            if (todo == null)
            {
                return NotFound(new { Error = "Todo item not found.", Version = "2.0" });
            }

            var response = new
            {
                data = todo,
                metadata = new
                {
                    version = "2.0",
                    retrievedAt = DateTime.UtcNow,
                    apiVersion = "v2"
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Create a new todo item (V2 - with enhanced validation)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> CreateTodo([FromBody] AddTodoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { Error = "Title is required.", Version = "2.0" });
            }

            if (request.Title.Length > 500)
            {
                return BadRequest(new { Error = "Title must be 500 characters or less.", Version = "2.0" });
            }

            var newTodo = await _mediator.Send(new AddTodoCommand.Command
            {
                Title = request.Title
            });

            var response = new
            {
                data = newTodo,
                metadata = new
                {
                    version = "2.0",
                    createdAt = DateTime.UtcNow,
                    apiVersion = "v2"
                }
            };

            return CreatedAtAction(nameof(GetTodo), new { id = newTodo.Id, version = "2.0" }, response);
        }

        /// <summary>
        /// Update an existing todo item (V2 - with enhanced validation)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { Error = "Title is required.", Version = "2.0" });
            }

            if (request.Title.Length > 500)
            {
                return BadRequest(new { Error = "Title must be 500 characters or less.", Version = "2.0" });
            }

            var updatedTodo = await _mediator.Send(new UpdateTodoCommand.Command
            {
                Id = id,
                Title = request.Title,
                IsCompleted = request.IsCompleted
            });

            if (updatedTodo == null)
            {
                return NotFound(new { Error = "Todo item not found.", Version = "2.0" });
            }

            var response = new
            {
                data = updatedTodo,
                metadata = new
                {
                    version = "2.0",
                    updatedAt = DateTime.UtcNow,
                    apiVersion = "v2"
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Delete a todo item (V2 - with enhanced response)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodo(int id)
        {
            var result = await _mediator.Send(new DeleteTodoCommand.Command { Id = id });
            
            if (!result)
            {
                return NotFound(new { Error = "Todo item not found.", Version = "2.0" });
            }

            return Ok(new { Message = "Todo deleted successfully.", Version = "2.0", deletedAt = DateTime.UtcNow });
        }
    }
}
