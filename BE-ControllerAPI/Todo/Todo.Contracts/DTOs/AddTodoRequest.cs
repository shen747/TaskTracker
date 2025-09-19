namespace Todo.Contracts.DTOs
{
    /// <summary>
    /// Request model for creating a new todo item
    /// </summary>
    public class AddTodoRequest
    {
        /// <summary>
        /// The title or description of the todo item to create
        /// </summary>
        /// <example>Complete the project documentation</example>
        public required string Title { get; set; }
    }
}
