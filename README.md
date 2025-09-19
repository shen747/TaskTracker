# TaskTracker - Full Stack Todo Application

> A comprehensive todo application built with .NET 8 Web API backend and Angular frontend, demonstrating clean architecture patterns and modern development practices.

This project showcases how to build a scalable todo application using Controller-based Web API with CQRS pattern, paired with an Angular frontend using reactive state management. The backend includes API versioning, comprehensive testing, structured logging with Serilog, and interactive Swagger documentation.

## What's Inside This Project

### Backend (Todo API)

- **Framework**: .NET 8 with Controller-based Web API
- **Architecture**: Clean Architecture with CQRS pattern using MediatR
- **API Versioning**: V1 and V2 with multiple versioning strategies (URL, header, query string)
- **Documentation**: Swagger UI with interactive testing capabilities
- **Logging**: Serilog with structured logging to console and file outputs
- **Testing**: 74 comprehensive unit and integration tests covering all layers
- **Port**: `http://localhost:5279`

### Frontend (Angular Todo App)

- **Framework**: Angular 20.3.2
- **Architecture**: Feature-based organization with clear domain boundaries
- **State Management**: NgRx Signal Store for reactive state management
- **Testing**: Jest with comprehensive unit test coverage
- **Styling**: SCSS with responsive design principles
- **Port**: `http://localhost:4200`

## Why CQRS and MediatR?

### CQRS (Command Query Responsibility Segregation)

CQRS was chosen because it clearly separates read and write operations, making the application easier to scale and maintain. Here's what this pattern brings to the table:

- **Independent Scaling**: Read and write operations can be scaled independently based on actual demand
- **Clear Separation**: Queries and commands have distinct responsibilities and can evolve separately
- **Better Testing**: Business logic can be tested in isolation without HTTP concerns
- **Future Optimization**: Opens doors for read replicas, event sourcing, and performance tuning
- **Maintainability**: Changes to read logic don't affect write logic and vice versa

### MediatR

MediatR provides a clean way to implement CQRS by decoupling business logic from controllers. The benefits are:

- **Decoupling**: Controllers only handle HTTP concerns, not business logic
- **Testability**: Business logic can be tested independently with simple unit tests
- **Maintainability**: Changes to business logic don't require controller modifications
- **Extensibility**: Easy to add cross-cutting concerns like logging, validation, and caching
- **Single Responsibility**: Each handler has one clear purpose and can be focused on a specific task
- **Dependency Injection**: Leverages .NET's built-in IoC container for clean dependency management

## Design Decisions

This prototype uses an in-memory data store for simplicity and demonstration purposes. In a production environment, you would typically use:

- **EF Core with SQL Server**: For relational data with complex relationships and transactions
- **Azure Table Storage**: For high-scale, cost-effective storage with eventual consistency
- **Redis**: For caching frequently accessed data and session management
- **Event Sourcing**: For audit trails and complex business workflows requiring full history
- **Message Queues**: For asynchronous processing and integration with external systems

The current implementation focuses on demonstrating clean architecture principles, comprehensive testing, and modern .NET development practices rather than production-ready data persistence.

## Project Structure

### Backend Architecture

```
BE-ControllerAPI/Todo/
├── Todo.API/         # Web API controllers and endpoints
├── Todo.Business/    # Application logic layer (CQRS with MediatR)
├── Todo.Data/        # Data access layer with repository pattern
├── Todo.Contracts/   # Shared models, DTOs, and interfaces
└── Todo.Tests/       # Comprehensive unit and integration tests
```

**Key Patterns Used:**

- **CQRS**: Separate handlers for read and write operations
- **MediatR Pattern**: Decoupled communication between controllers and business logic
- **Repository Pattern**: Abstracted data access layer for testability and flexibility
- **API Versioning**: Multiple versioning strategies (URL, header, query string)
- **Dependency Injection**: Proper IoC container usage for testability

### Frontend Architecture

```
src/app/features/todos/
├── components/       # Presentational components
├── stores/          # State management (NgRx Signal Store)
├── services/        # API communication layer
└── types/           # TypeScript interfaces
```

**Key Patterns Used:**

- **Domain-Driven Design**: Feature-based organization with clear boundaries
- **Single Responsibility**: Each component has a single, well-defined purpose
- **Reactive Programming**: NgRx Signal Store for reactive state management
- **Component Composition**: Modular, reusable components
- **Type Safety**: Full TypeScript implementation with proper typing

## Getting Started

### Prerequisites

You'll need the following installed on your machine:

- .NET 8 SDK
- Node.js (v18 or higher)
- npm or yarn

### Setup Instructions

**Step 1: Clone the repository**

```bash
git clone <repository-url>
cd TaskTracker
```

**Step 2: Backend Setup**

```bash
cd BE-ControllerAPI/Todo
dotnet restore
dotnet run --project Todo.API
```

The API will be available at:

- **API**: `http://localhost:5279`
- **Swagger UI**: `http://localhost:5279/swagger`
- **API V1**: `http://localhost:5279/api/v1/todos`
- **API V2**: `http://localhost:5279/api/v2/todos`

**Note**: Check the console output and `logs/todo-api-*.txt` files for structured logging output from Serilog.

**Step 3: Frontend Setup**

```bash
cd FE/todo-app
npm install
npm start
```

The frontend will be available at:

- **URL**: `http://localhost:4200`

**Step 4: Verify Everything Works**

1. Start the backend (Controller API)
2. Start the frontend
3. Open `http://localhost:4200` in your browser
4. You should see the todo application running
5. Try adding, viewing, and deleting todos to verify full functionality
6. Test the Swagger UI at `http://localhost:5279/swagger`
7. Check the console output and log files to see Serilog structured logging in action

## Development Workflow

### Running Both Projects

You'll need two terminals running simultaneously:

**Terminal 1 (Backend):**

```bash
cd TaskTracker/BE-ControllerAPI/Todo
dotnet run --project Todo.API
```

**Terminal 2 (Frontend):**

```bash
cd TaskTracker/FE/todo-app
npm start
```

### API Endpoints

**Version 1.0 (Basic):**
| Method | Endpoint | Description |
| ------ | --------------------------- | ------------------- |
| GET | `/api/v1/todos` | Retrieve all todos |
| GET | `/api/v1/todos/{id}` | Get todo by ID |
| POST | `/api/v1/todos` | Create a new todo |
| PUT | `/api/v1/todos/{id}` | Update a todo by ID |
| DELETE | `/api/v1/todos/{id}` | Delete a todo by ID |

**Version 2.0 (Enhanced):**
| Method | Endpoint | Description |
| ------ | --------------------------- | ------------------- |
| GET | `/api/v2/todos` | Retrieve all todos with metadata |
| GET | `/api/v2/todos/{id}` | Get todo by ID with metadata |
| POST | `/api/v2/todos` | Create a new todo with enhanced validation |
| PUT | `/api/v2/todos/{id}` | Update a todo by ID with enhanced validation |
| DELETE | `/api/v2/todos/{id}` | Delete a todo by ID with enhanced response |

### API Versioning Options

The API supports multiple versioning strategies:

- **URL Versioning**: `/api/v1/todos` or `/api/v2/todos`
- **Query String**: `/api/todos?version=1.0`
- **Header**: `X-Version: 1.0`
- **Media Type**: `application/json; ver=1.0`

### Running Tests

**Backend Tests (74 comprehensive tests):**

```bash
cd BE-ControllerAPI/Todo
dotnet test
dotnet test --collect:"XPlat Code Coverage"  # With coverage report
```

**Frontend Tests:**

```bash
cd FE/todo-app
npm test
npm run test:coverage  # With coverage report
```

## What Makes This Architecture Special

### Backend Advantages

- **API Versioning**: Support for multiple API versions with backward compatibility
- **Comprehensive Testing**: 74 unit and integration tests with full coverage
- **Interactive Documentation**: Swagger UI with request/response examples
- **Structured Logging**: Serilog with meaningful logs throughout all layers (API, Business, Data)
- **Clean Architecture**: Clear separation of concerns with CQRS and MediatR
- **Repository Pattern**: Abstracted data access for easy testing and maintenance
- **Configurable Versioning**: Easy to update API versions via configuration
- **Testability**: MediatR enables easy unit testing of handlers
- **Maintainability**: Clear separation of concerns with CQRS
- **Flexibility**: Easy to extend with new features
- **CQRS Pattern**: Independent scaling of read and write operations

### Frontend Advantages

- **Reactive UI**: Automatic updates when state changes
- **Type Safety**: Compile-time error checking with TypeScript
- **Modularity**: Feature-based organization for easy maintenance
- **Performance**: Optimized change detection with Angular signals
- **Rich UX**: Inline editing, loading animations, and keyboard shortcuts
- **Comprehensive CRUD**: Full Create, Read, Update, Delete operations

## Project Structure

```
TaskTracker/
├── BE-ControllerAPI/         # Controller-based Web API
│   └── Todo/
│       ├── Todo.API/         # Web API controllers and endpoints
│       ├── Todo.Business/    # Application logic layer (CQRS with MediatR)
│       ├── Todo.Data/        # Data access layer with repository pattern
│       ├── Todo.Contracts/   # Shared models, DTOs, and interfaces
│       ├── Todo.Tests/       # Comprehensive unit and integration tests
│

├── FE/
│   └── todo-app/            # Angular application
│       ├── src/app/features/todos/
│           ├── components/   # UI components
│           ├── stores/      # State management
│           ├── services/    # API layer
│           └── types/       # TypeScript interfaces
│
└── README.md
```

## How Frontend and Backend Work Together

The frontend and backend communicate through:

- **HTTP REST API**: Standard RESTful communication with full CRUD operations
- **API Versioning**: Support for multiple API versions (Controller API only)
- **CORS Configuration**: Proper cross-origin setup for development
- **Error Handling**: Consistent error responses and user feedback
- **Loading States**: Real-time UI feedback for all async operations
- **Optimistic Updates**: Immediate UI feedback with proper error rollback
- **Type Safety**: Shared TypeScript interfaces for consistent data models
- **Interactive Documentation**: Swagger UI for API testing and exploration

### Backend Port Configuration

The frontend is configured to call the backend at `http://localhost:5279` by default. If you need to change the backend port, simply update the `baseUrl` in the `api.service.ts` file:

```typescript
// In FE/todo-app/src/app/features/todos/services/api.service.ts
private readonly baseUrl = 'http://localhost:YOUR_PORT/api/v1/todos';
```
