# TodoApp - Frontend

A modern Angular-based Todo application featuring reactive state management with NgRx Signal Store. This frontend application provides a clean, responsive interface for managing todo items with real-time updates and comprehensive error handling.

## Tech Stack

- **Framework**: Angular 20.3.2
- **State Management**: NgRx Signal Store
- **Testing**: Jest with Angular Testing Utilities
- **Styling**: SCSS
- **Architecture**: Domain-Driven Design (DDD) with feature-based organization
- **Build Tool**: Angular CLI

## Key Features

- ✅ Add, edit, delete, and view todo items
- ✅ Inline editing with keyboard shortcuts (Enter to save, Escape to cancel)
- ✅ Click-to-toggle completion status
- ✅ Real-time UI updates with reactive state management
- ✅ Loading animations for all operations (add/edit/delete)
- ✅ Comprehensive error handling with retry functionality
- ✅ Clean, responsive design with modern UI
- ✅ Type-safe implementation with TypeScript
- ✅ Comprehensive unit test coverage with Jest

## Project Structure

```
src/app/features/todos/
├── components/
│   ├── todo-form/          # Todo input form component
│   └── todo-list/          # Todo list display component
├── stores/
│   └── todo-store.ts       # NgRx Signal Store for state management
├── services/
│   └── api.service.ts      # API communication service
└── types/
    └── todo-item.ts        # TypeScript interfaces
```

## Getting Started

### Prerequisites

- Node.js (v18 or higher)
- npm or yarn package manager

### Installation

1. Navigate to the project directory:

```bash
cd TaskTracker/FE/todo-app
```

2. Install dependencies:

```bash
npm install
```

### Development Server

To start a local development server, run:

```bash
npm start
# or
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

### API Backend

**Important**: This frontend requires the TodoApi backend to be running. Make sure to start the backend server first:

```bash
cd TaskTracker/BE/TodoApi
dotnet run
```

The backend should be running on `http://localhost:5000` (or `https://localhost:5001` for HTTPS).

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running Tests

This project uses Jest for unit testing. To execute unit tests, use the following commands:

```bash
# Run all tests once
npm test

# Run tests in watch mode
npm run test:watch

# Run tests with coverage report
npm run test:coverage
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## State Management

This application uses **NgRx Signal Store** for state management, providing:

- **Reactive Updates**: UI automatically updates when state changes
- **Type Safety**: Full TypeScript support with proper typing
- **Centralized State**: Single source of truth for all todo-related data
- **Computed Values**: Derived state like todo counts and loading indicators
- **Error Handling**: Centralized error management with user-friendly messages
- **Loading States**: Individual loading indicators for add, edit, and delete operations
- **Optimistic Updates**: Immediate UI feedback with proper error rollback

## Architecture

The application follows **Domain-Driven Design (DDD)** principles:

- **Feature-based organization**: All todo-related code is grouped under `features/todos/`
- **Separation of concerns**: Components, services, stores, and types are clearly separated
- **Reusable components**: Modular design allows for easy extension and maintenance
- **Clean interfaces**: Well-defined contracts between layers

## Available Scripts

- `npm start` - Start development server
- `npm run build` - Build for production
- `npm test` - Run unit tests
- `npm run test:watch` - Run tests in watch mode
- `npm run test:coverage` - Run tests with coverage report

## Additional Resources

- [Angular Documentation](https://angular.dev/)
- [NgRx Signal Store](https://ngrx.io/guide/signals/signal-store)
- [Angular CLI Overview](https://angular.dev/tools/cli)
