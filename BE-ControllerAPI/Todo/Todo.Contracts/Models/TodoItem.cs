namespace Todo.Contracts.Models
{
    /// <summary>
    /// Represents a todo item in the system
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// The unique identifier of the todo item
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The title or description of the todo item
        /// </summary>
        /// <example>Complete the project documentation</example>
        public required string Title { get; set; }

        /// <summary>
        /// Indicates whether the todo item has been completed
        /// </summary>
        /// <example>false</example>
        public bool IsCompleted { get; set; }
    }
}
