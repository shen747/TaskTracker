import { Injectable, inject, computed } from '@angular/core';
import {
  patchState,
  signalStore,
  withHooks,
  withMethods,
  withState,
  withComputed,
} from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { catchError, of, switchMap, tap } from 'rxjs';
import { ApiService } from '../services/api.service';
import { TodoItem } from '../types/todo-item';

export interface TodoState {
  todos: TodoItem[];
  isLoading: boolean;
  isAdding: boolean;
  deletingIds: Set<number>;
  updatingIds: Set<number>;
  error: string | null;
}

const initialState: TodoState = {
  todos: [],
  isLoading: false,
  isAdding: false,
  deletingIds: new Set(),
  updatingIds: new Set(),
  error: null,
};

@Injectable()
export class TodoStore extends signalStore(
  withState(initialState),
  withComputed((store) => ({
    todosCount: computed(() => store.todos().length),
    completedTodosCount: computed(() => store.todos().filter((todo) => todo.isCompleted).length),
    hasTodos: computed(() => store.todos().length > 0),
    isDeleting: computed(() => store.deletingIds().size > 0),
    isUpdating: computed(() => store.updatingIds().size > 0),
    hasError: computed(() => !!store.error()),
  })),
  withMethods((store, apiService = inject(ApiService)) => ({
    // Load all todos
    loadTodos: rxMethod<void>(
      switchMap(() =>
        apiService.getTodos().pipe(
          tap({
            next: (todos) => {
              patchState(store, {
                todos,
                isLoading: false,
                error: null,
              });
            },
            error: (error) => {
              console.error('Error loading todos:', error);
              patchState(store, {
                isLoading: false,
                error: 'Failed to load todos. Please check if the API server is running.',
              });
            },
          }),
          catchError(() => of([]))
        )
      )
    ),

    // Add a new todo
    addTodo: rxMethod<string>(
      switchMap((title) => {
        patchState(store, { isAdding: true, error: null });

        return apiService.addTodo(title).pipe(
          tap({
            next: (newTodo) => {
              patchState(store, {
                todos: [...store.todos(), newTodo],
                isAdding: false,
                error: null,
              });
            },
            error: (error) => {
              console.error('Error adding todo:', error);
              patchState(store, {
                isAdding: false,
                error: 'Failed to add todo. Please check if the API server is running.',
              });
            },
          }),
          catchError(() => of(null))
        );
      })
    ),

    // Delete a todo
    deleteTodo: rxMethod<number>(
      switchMap((id) => {
        const deletingIds = new Set(store.deletingIds());
        deletingIds.add(id);

        patchState(store, {
          deletingIds: new Set(deletingIds),
          error: null,
        });

        return apiService.deleteTodo(id).pipe(
          tap({
            next: () => {
              const newDeletingIds = new Set(store.deletingIds());
              newDeletingIds.delete(id);

              patchState(store, {
                todos: store.todos().filter((todo) => todo.id !== id),
                deletingIds: newDeletingIds,
                error: null,
              });
            },
            error: (error) => {
              console.error('Error deleting todo:', error);
              const newDeletingIds = new Set(store.deletingIds());
              newDeletingIds.delete(id);

              patchState(store, {
                deletingIds: newDeletingIds,
                error: 'Failed to delete todo. Please check if the API server is running.',
              });
            },
          }),
          catchError(() => of(null))
        );
      })
    ),

    // Update a todo
    updateTodo: rxMethod<{ id: number; title: string; isCompleted: boolean }>(
      switchMap(({ id, title, isCompleted }) => {
        const updatingIds = new Set(store.updatingIds());
        updatingIds.add(id);

        patchState(store, {
          updatingIds: new Set(updatingIds),
          error: null,
        });

        return apiService.updateTodo(id, title, isCompleted).pipe(
          tap({
            next: (updatedTodo) => {
              const newUpdatingIds = new Set(store.updatingIds());
              newUpdatingIds.delete(id);

              patchState(store, {
                todos: store.todos().map((todo) => (todo.id === id ? updatedTodo : todo)),
                updatingIds: newUpdatingIds,
                error: null,
              });
            },
            error: (error) => {
              console.error('Error updating todo:', error);
              const newUpdatingIds = new Set(store.updatingIds());
              newUpdatingIds.delete(id);

              patchState(store, {
                updatingIds: newUpdatingIds,
                error: 'Failed to update todo. Please check if the API server is running.',
              });
            },
          }),
          catchError(() => of(null))
        );
      })
    ),

    // Clear error message
    clearError: () => {
      patchState(store, { error: null });
    },

    // Set loading state manually (for initial load)
    setLoading: (loading: boolean) => {
      patchState(store, { isLoading: loading });
    },
  })),
  withHooks({
    onInit(store) {
      // Auto-load todos when store is initialized
      store.setLoading(true);
      store.loadTodos();
    },
  })
) {}
