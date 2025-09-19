namespace Todo.Contracts.DTOs
{
    /// <summary>
    /// Request model for updating an existing todo item
    /// </summary>
    public class UpdateTodoRequest
    {
        /// <summary>
        /// The updated title or description of the todo item
        /// </summary>
        /// <example>Updated todo title</example>
        public required string Title { get; set; }

        /// <summary>
        /// The updated completion status of the todo item
        /// </summary>
        /// <example>true</example>
        public bool IsCompleted { get; set; }
    }
}
