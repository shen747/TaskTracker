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
    /// Todo API Version 1.0 - Basic CRUD operations for todo items
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/todos")]
    [Tags("Todos")]
    public class TodosController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the TodosController
        /// </summary>
        /// <param name="mediator">MediatR mediator for handling commands and queries</param>
        public TodosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all todo items
        /// </summary>
        /// <returns>A list of all todo items</returns>
        /// <response code="200">Returns the list of todo items</response>
        /// <response code="500">Internal server error</response>
        /// <remarks>
        /// **Production Considerations:**
        /// 
        /// In a production scenario with large datasets, this endpoint would implement pagination to:
        /// - Improve performance by limiting result sets
        /// - Reduce memory usage and network payload
        /// - Provide better user experience with manageable data chunks
        /// 
        /// **Proposed Pagination Implementation:**
        /// 
        /// ```csharp
        /// /// <summary>
        /// /// Retrieves paginated todo items
        /// /// </summary>
        /// /// <param name="pageNumber">Page number (1-based)</param>
        /// /// <param name="pageSize">Number of items per page (default: 20, max: 100)</param>
        /// /// <param name="filter">Optional filter by completion status</param>
        /// /// <param name="sortBy">Sort field (id, title, isCompleted)</param>
        /// /// <param name="sortOrder">Sort direction (asc, desc)</param>
        /// /// <returns>Paginated result with metadata</returns>
        /// [HttpGet]
        /// public async Task<ActionResult<PagedResult&lt;TodoItem&gt;&gt;&gt; GetTodos(
        ///     [FromQuery] int pageNumber = 1,
        ///     [FromQuery] int pageSize = 20,
        ///     [FromQuery] bool? isCompleted = null,
        ///     [FromQuery] string sortBy = "id",
        ///     [FromQuery] string sortOrder = "asc")
        /// {
        ///     // Validate pagination parameters
        ///     if (pageNumber < 1) pageNumber = 1;
        ///     if (pageSize < 1 || pageSize > 100) pageSize = 20;
        ///     
        ///     var query = new GetTodosQuery.Query
        ///     {
        ///         PageNumber = pageNumber,
        ///         PageSize = pageSize,
        ///         IsCompleted = isCompleted,
        ///         SortBy = sortBy,
        ///         SortOrder = sortOrder
        ///     };
        ///     
        ///     var result = await _mediator.Send(query);
        ///     return Ok(result);
        /// }
        /// ```
        /// 
        /// **Response Format:**
        /// ```json
        /// {
        ///   "data": [
        ///     { "id": 1, "title": "Task 1", "isCompleted": false },
        ///     { "id": 2, "title": "Task 2", "isCompleted": true }
        ///   ],
        ///   "pagination": {
        ///     "pageNumber": 1,
        ///     "pageSize": 20,
        ///     "totalCount": 150,
        ///     "totalPages": 8,
        ///     "hasPrevious": false,
        ///     "hasNext": true
        ///   }
        /// }
        /// ```
        /// 
        /// **Benefits:**
        /// - **Performance**: Only loads required data, reducing database load
        /// - **Scalability**: Handles large datasets without memory issues
        /// - **User Experience**: Faster response times and manageable data chunks
        /// - **Flexibility**: Supports filtering, sorting, and custom page sizes
        /// - **API Design**: Follows REST best practices for paginated endpoints
        /// </remarks>
        /// <example>
        /// GET /api/v1/todos
        /// Returns:
        /// [
        ///   {
        ///     "id": 1,
        ///     "title": "Build a REST API",
        ///     "isCompleted": false
        ///   }
        /// ]
        /// 
        /// GET /api/v1/todos?pageNumber=1&pageSize=10&isCompleted=false
        /// Returns paginated result with metadata
        /// </example>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Contracts.Models.TodoItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Contracts.Models.TodoItem>>> GetTodos()
        {
            Log.Information("Getting all todos - API Version 1.0");
            
            try
            {
                var todos = await _mediator.Send(new GetTodosQuery.Query());
                Log.Information("Retrieved {TodoCount} todos successfully", todos.Count());
                return Ok(todos);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving todos");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific todo item by its ID
        /// </summary>
        /// <param name="id">The unique identifier of the todo item</param>
        /// <returns>The requested todo item</returns>
        /// <response code="200">Returns the requested todo item</response>
        /// <response code="404">Todo item not found</response>
        /// <response code="400">Invalid ID format</response>
        /// <example>
        /// GET /api/v1/todos/1
        /// Returns:
        /// {
        ///   "id": 1,
        ///   "title": "Build a REST API",
        ///   "isCompleted": false
        /// }
        /// </example>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Contracts.Models.TodoItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Contracts.Models.TodoItem>> GetTodo(int id)
        {
            Log.Information("Getting todo by ID: {TodoId} - API Version 1.0", id);
            
            try
            {
                var todo = await _mediator.Send(new GetTodoByIdQuery.Query { Id = id });
                
                if (todo == null)
                {
                    Log.Warning("Todo with ID {TodoId} not found", id);
                    return NotFound(new { Error = "Todo item not found." });
                }

                Log.Information("Successfully retrieved todo {TodoId}: {TodoTitle}", todo.Id, todo.Title);
                return Ok(todo);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving todo with ID {TodoId}", id);
                throw;
            }
        }

        /// <summary>
        /// Creates a new todo item
        /// </summary>
        /// <param name="request">The todo item data</param>
        /// <returns>The created todo item</returns>
        /// <response code="201">Todo item created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        /// POST /api/v1/todos
        /// Request Body:
        /// {
        ///   "title": "Complete the project documentation"
        /// }
        /// Returns:
        /// {
        ///   "id": 4,
        ///   "title": "Complete the project documentation",
        ///   "isCompleted": false
        /// }
        /// </example>
        [HttpPost]
        [ProducesResponseType(typeof(Contracts.Models.TodoItem), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Contracts.Models.TodoItem>> CreateTodo([FromBody] AddTodoRequest request)
        {
            Log.Information("Creating new todo with title: {TodoTitle} - API Version 1.0", request.Title);
            
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                Log.Warning("Attempted to create todo with empty or null title");
                return BadRequest(new { Error = "Title is required." });
            }

            try
            {
                var newTodo = await _mediator.Send(new AddTodoCommand.Command
                {
                    Title = request.Title
                });

                Log.Information("Successfully created todo {TodoId}: {TodoTitle}", newTodo.Id, newTodo.Title);
                return CreatedAtAction(nameof(GetTodo), new { id = newTodo.Id }, newTodo);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating todo with title: {TodoTitle}", request.Title);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing todo item
        /// </summary>
        /// <param name="id">The unique identifier of the todo item to update</param>
        /// <param name="request">The updated todo item data</param>
        /// <returns>The updated todo item</returns>
        /// <response code="200">Todo item updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Todo item not found</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        /// PUT /api/v1/todos/1
        /// Request Body:
        /// {
        ///   "title": "Updated todo title",
        ///   "isCompleted": true
        /// }
        /// Returns:
        /// {
        ///   "id": 1,
        ///   "title": "Updated todo title",
        ///   "isCompleted": true
        /// }
        /// </example>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Contracts.Models.TodoItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Contracts.Models.TodoItem>> UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
        {
            Log.Information("Updating todo {TodoId} with title: {TodoTitle}, completed: {IsCompleted} - API Version 1.0", 
                id, request.Title, request.IsCompleted);
            
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                Log.Warning("Attempted to update todo {TodoId} with empty or null title", id);
                return BadRequest(new { Error = "Title is required." });
            }

            try
            {
                var updatedTodo = await _mediator.Send(new UpdateTodoCommand.Command
                {
                    Id = id,
                    Title = request.Title,
                    IsCompleted = request.IsCompleted
                });

                if (updatedTodo == null)
                {
                    Log.Warning("Attempted to update non-existent todo {TodoId}", id);
                    return NotFound(new { Error = "Todo item not found." });
                }

                Log.Information("Successfully updated todo {TodoId}: {TodoTitle}", updatedTodo.Id, updatedTodo.Title);
                return Ok(updatedTodo);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating todo {TodoId}", id);
                throw;
            }
        }

        /// <summary>
        /// Deletes a todo item
        /// </summary>
        /// <param name="id">The unique identifier of the todo item to delete</param>
        /// <returns>No content on successful deletion</returns>
        /// <response code="204">Todo item deleted successfully</response>
        /// <response code="404">Todo item not found</response>
        /// <response code="400">Invalid ID format</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        /// DELETE /api/v1/todos/1
        /// Returns: 204 No Content
        /// </example>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteTodo(int id)
        {
            Log.Information("Deleting todo {TodoId} - API Version 1.0", id);
            
            try
            {
                var result = await _mediator.Send(new DeleteTodoCommand.Command { Id = id });
                
                if (!result)
                {
                    Log.Warning("Attempted to delete non-existent todo {TodoId}", id);
                    return NotFound(new { Error = "Todo item not found." });
                }

                Log.Information("Successfully deleted todo {TodoId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting todo {TodoId}", id);
                throw;
            }
        }
    }
}
