# TaskTracker - Full Stack Todo Application

A modern full-stack todo application showcasing clean architecture patterns and modern development practices. The application consists of a .NET 8 Minimal API backend and an Angular frontend with reactive state management.

## 🏗️ Architecture Overview

### Backend (TodoApi)

- **Framework**: .NET 8 with Minimal APIs
- **Architecture**: Clean Architecture with CQRS pattern
- **State Management**: MediatR for command/query separation
- **Data Store**: In-memory data store (for simplicity)
- **API Design**: RESTful endpoints with proper HTTP status codes

### Frontend (TodoApp)

- **Framework**: Angular 20.3.2
- **Architecture**: Domain-Driven Design (DDD) with feature-based organization
- **State Management**: NgRx Signal Store for reactive state management
- **Testing**: Jest with comprehensive unit test coverage
- **Styling**: SCSS with responsive design

## 🎯 Clean Architecture Patterns

### Backend Architecture

```
TodoApi/
├── Domain/           # Business entities and rules
├── Features/         # Feature-based organization (CQRS)
│   ├── Commands/     # Write operations
│   └── Queries/      # Read operations
├── Data/            # Data access layer
└── Program.cs       # Dependency injection and API configuration
```

**Key Patterns:**

- **CQRS (Command Query Responsibility Segregation)**: Separate handlers for read and write operations
- **MediatR Pattern**: Decoupled communication between controllers and business logic
- **Minimal APIs**: Clean, lightweight API endpoints without unnecessary boilerplate
- **Dependency Injection**: Proper IoC container usage for testability

### Frontend Architecture

```
src/app/features/todos/
├── components/       # Presentational components
├── stores/          # State management (NgRx Signal Store)
├── services/        # API communication layer
└── types/           # TypeScript interfaces
```

**Key Patterns:**

- **Domain-Driven Design (DDD)**: Feature-based organization with clear boundaries
- **Single Responsibility Principle**: Each component has a single, well-defined purpose
- **Reactive Programming**: NgRx Signal Store for reactive state management
- **Component Composition**: Modular, reusable components
- **Type Safety**: Full TypeScript implementation with proper typing

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK
- Node.js (v18 or higher)
- npm or yarn

### 1. Clone and Setup

```bash
git clone <repository-url>
cd TaskTracker
```

### 2. Backend Setup

```bash
cd BE/TodoApi
dotnet restore
dotnet run
```

The API will be available at (port may vary):

- **HTTP**: `http://localhost:5000` (or `http://localhost:5267` as shown in terminal output)
- **HTTPS**: `https://localhost:5001` (or similar HTTPS port)
- **Swagger UI**: Available on the HTTPS port at `/swagger` (Development only)

**Note**: The exact port numbers will be displayed in the terminal when you run `dotnet run`. Look for output like:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5267
```

### 3. Frontend Setup

```bash
cd FE/todo-app
npm install
npm start
```

The frontend will be available at (port may vary):

- **URL**: `http://localhost:4200` (default Angular port)

**Note**: The exact port will be displayed in the terminal when you run `npm start`. Look for output like:

```
** Angular Live Development Server is listening on localhost:4200, open your browser on http://localhost:4200/ **
```

### 4. Verify Setup

1. Open `http://localhost:4200` in your browser
2. You should see the todo application with pre-loaded sample data
3. Try adding, viewing, and deleting todos to verify full functionality

## 🔧 Development Workflow

### Running Both Projects

**Terminal 1 (Backend):**

```bash
cd TaskTracker/BE/TodoApi
dotnet run
```

**Terminal 2 (Frontend):**

```bash
cd TaskTracker/FE/todo-app
npm start
```

### API Endpoints

| Method | Endpoint          | Description         |
| ------ | ----------------- | ------------------- |
| GET    | `/api/todos`      | Retrieve all todos  |
| POST   | `/api/todos`      | Create a new todo   |
| PUT    | `/api/todos/{id}` | Update a todo by ID |
| DELETE | `/api/todos/{id}` | Delete a todo by ID |

### Testing

**Backend Tests:**

```bash
cd BE/TodoApi
dotnet test
```

**Frontend Tests:**

```bash
cd FE/todo-app
npm test
npm run test:coverage  # With coverage report
```

## 🏛️ Architecture Benefits

### Backend Benefits

- **Testability**: MediatR enables easy unit testing of handlers
- **Maintainability**: Clear separation of concerns with CQRS
- **Scalability**: Minimal API overhead with efficient request handling
- **Flexibility**: Easy to extend with new features

### Frontend Benefits

- **Reactive UI**: Automatic updates when state changes
- **Type Safety**: Compile-time error checking with TypeScript
- **Modularity**: Feature-based organization for easy maintenance
- **Performance**: Optimized change detection with Angular signals
- **Rich UX**: Inline editing, loading animations, and keyboard shortcuts
- **Comprehensive CRUD**: Full Create, Read, Update, Delete operations

## 📁 Project Structure

```
TaskTracker/
├── BE/
│   └── TodoApi/              # .NET 8 Minimal API
│       ├── Domain/           # Business entities
│       ├── Features/         # CQRS handlers
│       ├── Data/            # Data access
│       └── Program.cs       # Application entry point
├── FE/
│   └── todo-app/            # Angular application
│       ├── src/app/features/todos/
│       │   ├── components/   # UI components
│       │   ├── stores/      # State management
│       │   ├── services/    # API layer
│       │   └── types/       # TypeScript interfaces
│       └── README.md        # Frontend documentation
└── README.md               # This file
```

## 🔗 Integration

The frontend and backend communicate through:

- **HTTP REST API**: Standard RESTful communication with full CRUD operations
- **CORS Configuration**: Proper cross-origin setup for development
- **Error Handling**: Consistent error responses and user feedback
- **Loading States**: Real-time UI feedback for all async operations
- **Optimistic Updates**: Immediate UI feedback with proper error rollback
- **Type Safety**: Shared TypeScript interfaces for consistent data models

## 📚 Additional Resources

- [Backend README](./BE/TodoApi/README.md) - Detailed backend documentation
- [Frontend README](./FE/todo-app/README.md) - Detailed frontend documentation
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Angular Documentation](https://angular.dev/)
- [NgRx Signal Store](https://ngrx.io/guide/signals/signal-store)
