# TaskTracker (TodoApi)

A minimal .NET 8 REST API for managing todo items. Uses Minimal APIs, MediatR and an in-memory data store. Provides comprehensive endpoints to list, add, update, and delete todos (seeded with sample items).

## Tech

- .NET 8
- MediatR
- Minimal APIs

## Requirements

- .NET 8 SDK installed
- Visual Studio 2022 (or use the dotnet CLI)

## Run (Visual Studio)

1. Open the solution or folder in Visual Studio 2022.
2. Start the app with **Debug > Start Debugging** (F5) or **Debug > Start Without Debugging**.

## Run (CLI)

From the project folder that contains the .csproj:

```bash
dotnet run
```

The app will print the listening URL(s) (typically https://localhost:{port}). Swagger UI is available when running in Development.

## API Endpoints

- GET /api/todos
  - Returns all todos.
  - Example: curl -k https://localhost:{port}/api/todos
- POST /api/todos
  - Body: { "title": "Buy groceries" }
  - Title is required; server returns 400 if missing.
  - Example:
    curl -k -X POST https://localhost:{port}/api/todos \
     -H "Content-Type: application/json" \
     -d '{"title":"New todo"}'
- PUT /api/todos/{id}
  - Body: { "title": "Updated todo", "isCompleted": true }
  - Updates the todo with the given id. Title and isCompleted are required.
  - Returns 404 if todo not found, 400 if validation fails.
  - Example:
    curl -k -X PUT https://localhost:{port}/api/todos/1 \
     -H "Content-Type: application/json" \
     -d '{"title":"Updated todo","isCompleted":true}'
- DELETE /api/todos/{id}
  - Deletes the todo with the given id.
  - Example: curl -k -X DELETE https://localhost:{port}/api/todos/1

## Notes

- The data store is in-memory and seeded with three todos; data is lost when the app stops.
- CORS is configured to allow any origin/headers/methods (convenient for local development). Restrict CORS in production.
