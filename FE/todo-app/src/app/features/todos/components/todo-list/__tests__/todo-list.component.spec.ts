import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { TodoListComponent } from '../todo-list.component';
import { TodoStore } from '../../../stores/todo-store';
import { ApiService } from '../../../services/api.service';
import { TodoItem } from '../../../types/todo-item';

describe('TodoListComponent', () => {
  let component: TodoListComponent;
  let fixture: ComponentFixture<TodoListComponent>;
  let mockTodoStore: jest.Mocked<TodoStore>;
  let mockApiService: jest.Mocked<ApiService>;

  const mockTodos: TodoItem[] = [
    { id: 1, title: 'First Todo', isCompleted: false },
    { id: 2, title: 'Second Todo', isCompleted: false },
    { id: 3, title: 'Third Todo', isCompleted: true },
  ];

  beforeEach(async () => {
    const apiServiceSpy = {
      getTodos: jest.fn(),
      addTodo: jest.fn(),
      deleteTodo: jest.fn(),
    };

    const todoStoreSpy = {
      addTodo: jest.fn(),
      clearError: jest.fn(),
      isAdding: jest.fn().mockReturnValue(false),
      error: jest.fn().mockReturnValue(null),
      todos: jest.fn().mockReturnValue(mockTodos),
      isLoading: jest.fn().mockReturnValue(false),
      deletingIds: jest.fn().mockReturnValue(new Set()),
      updatingIds: jest.fn().mockReturnValue(new Set()),
      loadTodos: jest.fn(),
      deleteTodo: jest.fn(),
      updateTodo: jest.fn(),
      setLoading: jest.fn(),
      todosCount: jest.fn().mockReturnValue(3),
      completedTodosCount: jest.fn().mockReturnValue(1),
      hasTodos: jest.fn().mockReturnValue(true),
      isDeleting: jest.fn().mockReturnValue(false),
      isUpdating: jest.fn().mockReturnValue(false),
      hasError: jest.fn().mockReturnValue(false),
    };

    await TestBed.configureTestingModule({
      imports: [TodoListComponent],
      providers: [
        { provide: ApiService, useValue: apiServiceSpy },
        { provide: TodoStore, useValue: todoStoreSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoListComponent);
    component = fixture.componentInstance;
    mockTodoStore = TestBed.inject(TodoStore) as jest.Mocked<TodoStore>;
    mockApiService = TestBed.inject(ApiService) as jest.Mocked<ApiService>;

    // Set up the required input using the component's input signal
    fixture.componentRef.setInput('todoStore', mockTodoStore);
  });

  beforeEach(() => {
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('deleteTodo', () => {
    it('should call store deleteTodo method', () => {
      const todoIdToDelete = 2;

      component.deleteTodo(todoIdToDelete);

      expect(mockTodoStore.deleteTodo).toHaveBeenCalledWith(todoIdToDelete);
    });
  });

  describe('loadTodos', () => {
    it('should call store loadTodos method', () => {
      component.loadTodos();

      expect(mockTodoStore.loadTodos).toHaveBeenCalled();
    });
  });

  describe('clearError', () => {
    it('should call store clearError method', () => {
      component.clearError();

      expect(mockTodoStore.clearError).toHaveBeenCalled();
    });
  });

  describe('isDeleting', () => {
    it('should return true when store indicates todo is being deleted', () => {
      const todoId = 2;
      mockTodoStore.deletingIds.mockReturnValue(new Set([2, 3]));

      const result = component.isDeleting(todoId);

      expect(result).toBe(true);
    });

    it('should return false when store indicates todo is not being deleted', () => {
      const todoId = 1;
      mockTodoStore.deletingIds.mockReturnValue(new Set([2, 3]));

      const result = component.isDeleting(todoId);

      expect(result).toBe(false);
    });
  });

  describe('isUpdating', () => {
    it('should return true when store indicates todo is being updated', () => {
      const todoId = 2;
      mockTodoStore.updatingIds.mockReturnValue(new Set([2, 3]));

      const result = component.isUpdating(todoId);

      expect(result).toBe(true);
    });

    it('should return false when store indicates todo is not being updated', () => {
      const todoId = 1;
      mockTodoStore.updatingIds.mockReturnValue(new Set([2, 3]));

      const result = component.isUpdating(todoId);

      expect(result).toBe(false);
    });
  });

  describe('startEdit', () => {
    it('should set editingId and editTitle when starting edit', () => {
      const todo = mockTodos[0];

      component.startEdit(todo);

      expect(component.editingId()).toBe(todo.id);
      expect(component.editTitle()).toBe(todo.title);
    });

    it('should set editingId and editTitle with event when starting edit', () => {
      const todo = mockTodos[0];
      const mockEvent = { stopPropagation: jest.fn() } as any;

      component.startEdit(todo, mockEvent);

      expect(component.editingId()).toBe(todo.id);
      expect(component.editTitle()).toBe(todo.title);
      expect(mockEvent.stopPropagation).toHaveBeenCalled();
    });
  });

  describe('cancelEdit', () => {
    it('should clear editingId and editTitle when canceling edit', () => {
      component.startEdit(mockTodos[0]);

      component.cancelEdit();

      expect(component.editingId()).toBeNull();
      expect(component.editTitle()).toBe('');
    });
  });

  describe('saveEdit', () => {
    it('should call updateTodo when title has changed', () => {
      const todo = mockTodos[0];
      const newTitle = 'Updated Title';
      component.editTitle.set(newTitle);

      component.saveEdit(todo);

      expect(mockTodoStore.updateTodo).toHaveBeenCalledWith({
        id: todo.id,
        title: newTitle,
        isCompleted: todo.isCompleted,
      });
      expect(component.editingId()).toBeNull();
      expect(component.editTitle()).toBe('');
    });

    it('should not call updateTodo when title is unchanged', () => {
      const todo = mockTodos[0];
      component.editTitle.set(todo.title);

      component.saveEdit(todo);

      expect(mockTodoStore.updateTodo).not.toHaveBeenCalled();
      expect(component.editingId()).toBeNull();
      expect(component.editTitle()).toBe('');
    });

    it('should not call updateTodo when title is empty', () => {
      const todo = mockTodos[0];
      component.editTitle.set('');

      component.saveEdit(todo);

      expect(mockTodoStore.updateTodo).not.toHaveBeenCalled();
      expect(component.editingId()).toBeNull();
      expect(component.editTitle()).toBe('');
    });
  });

  describe('toggleCompletion', () => {
    it('should call updateTodo with toggled completion status', () => {
      const todo = mockTodos[0]; // isCompleted: false

      component.toggleCompletion(todo);

      expect(mockTodoStore.updateTodo).toHaveBeenCalledWith({
        id: todo.id,
        title: todo.title,
        isCompleted: true,
      });
    });

    it('should call updateTodo with toggled completion status for completed todo', () => {
      const todo = mockTodos[2]; // isCompleted: true

      component.toggleCompletion(todo);

      expect(mockTodoStore.updateTodo).toHaveBeenCalledWith({
        id: todo.id,
        title: todo.title,
        isCompleted: false,
      });
    });
  });

  describe('isEditing', () => {
    it('should return true when todo is being edited', () => {
      const todo = mockTodos[0];
      component.startEdit(todo);

      const result = component.isEditing(todo.id);

      expect(result).toBe(true);
    });

    it('should return false when todo is not being edited', () => {
      const todo = mockTodos[0];

      const result = component.isEditing(todo.id);

      expect(result).toBe(false);
    });
  });

  describe('template interactions', () => {
    it('should display loading message with spinner when store is loading', () => {
      mockTodoStore.isLoading.mockReturnValue(true);
      fixture.detectChanges();

      const loadingDiv = fixture.nativeElement.querySelector('.loading-message');
      const loadingText = fixture.nativeElement.querySelector('.loading-message p');
      const spinner = fixture.nativeElement.querySelector('.loading-spinner');

      expect(loadingDiv).toBeTruthy();
      expect(loadingText.textContent.trim()).toBe('Loading todos...');
      expect(spinner).toBeTruthy();
    });

    it('should not display loading message when store is not loading', () => {
      mockTodoStore.isLoading.mockReturnValue(false);
      fixture.detectChanges();

      const loadingDiv = fixture.nativeElement.querySelector('.loading-message');

      expect(loadingDiv).toBeFalsy();
    });

    it('should display error message when store has error', () => {
      mockTodoStore.error.mockReturnValue('Test error message');
      fixture.detectChanges();

      const errorDiv = fixture.nativeElement.querySelector('.error-message');
      const errorText = fixture.nativeElement.querySelector('.error-message p');

      expect(errorDiv).toBeTruthy();
      expect(errorText.textContent.trim()).toBe('Test error message');
    });

    it('should display retry button in error message', () => {
      mockTodoStore.error.mockReturnValue('Test error message');
      fixture.detectChanges();

      const retryButton = fixture.nativeElement.querySelector('.error-message button');

      expect(retryButton).toBeTruthy();
      expect(retryButton.textContent.trim()).toBe('Retry');
    });

    it('should call loadTodos when retry button is clicked', () => {
      mockTodoStore.error.mockReturnValue('Test error message');
      fixture.detectChanges();

      jest.spyOn(component, 'loadTodos');
      const retryButton = fixture.nativeElement.querySelector('.error-message button');

      retryButton.click();

      expect(component.loadTodos).toHaveBeenCalled();
    });

    it('should display all todos from store in the list', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      fixture.detectChanges();

      const todoItems = fixture.nativeElement.querySelectorAll('.todo-item');
      const todoTitles = fixture.nativeElement.querySelectorAll('.todo-title');

      expect(todoItems).toHaveLength(3);
      expect(todoTitles[0].textContent.trim()).toBe('First Todo');
      expect(todoTitles[1].textContent.trim()).toBe('Second Todo');
      expect(todoTitles[2].textContent.trim()).toBe('Third Todo');
    });

    it('should display delete buttons for each todo', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.deletingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      const deleteButtons = fixture.nativeElement.querySelectorAll('.delete-btn');

      expect(deleteButtons).toHaveLength(3);
      deleteButtons.forEach((button: any) => {
        expect(button.textContent.trim()).toBe('✕');
      });
    });

    it('should disable delete button when todo is being deleted', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.deletingIds.mockReturnValue(new Set([2]));
      fixture.detectChanges();

      const deleteButtons = fixture.nativeElement.querySelectorAll('.delete-btn');

      expect(deleteButtons[0].disabled).toBe(false); // ID 1
      expect(deleteButtons[1].disabled).toBe(true); // ID 2 - being deleted
      expect(deleteButtons[2].disabled).toBe(false); // ID 3
    });

    it('should show spinner on delete button when deleting', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.deletingIds.mockReturnValue(new Set([2]));
      fixture.detectChanges();

      const deleteButtons = fixture.nativeElement.querySelectorAll('.delete-btn');
      const spinner = deleteButtons[1].querySelector('.mini-spinner');

      expect(spinner).toBeTruthy(); // ID 2 - showing spinner
    });

    it('should call deleteTodo when delete button is clicked', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.deletingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      jest.spyOn(component, 'deleteTodo');
      const deleteButtons = fixture.nativeElement.querySelectorAll('.delete-btn');

      deleteButtons[1].click(); // Click second todo's delete button

      expect(component.deleteTodo).toHaveBeenCalledWith(2);
    });

    it('should display empty message when no todos and not loading', () => {
      mockTodoStore.todos.mockReturnValue([]);
      mockTodoStore.isLoading.mockReturnValue(false);
      mockTodoStore.error.mockReturnValue(null);
      fixture.detectChanges();

      const emptyMessage = fixture.nativeElement.querySelector('.empty-list-message');

      expect(emptyMessage).toBeTruthy();
      expect(emptyMessage.textContent.trim()).toBe(
        'Your list is empty. Add a new TODO to get started!'
      );
    });

    it('should not display empty message when loading', () => {
      mockTodoStore.todos.mockReturnValue([]);
      mockTodoStore.isLoading.mockReturnValue(true);
      fixture.detectChanges();

      const emptyMessage = fixture.nativeElement.querySelector('.empty-list-message');

      expect(emptyMessage).toBeFalsy();
    });

    it('should not display empty message when there is an error', () => {
      mockTodoStore.todos.mockReturnValue([]);
      mockTodoStore.isLoading.mockReturnValue(false);
      mockTodoStore.error.mockReturnValue('Test error');
      fixture.detectChanges();

      const emptyMessage = fixture.nativeElement.querySelector('.empty-list-message');

      expect(emptyMessage).toBeFalsy();
    });

    it('should display edit buttons for each todo', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      const editButtons = fixture.nativeElement.querySelectorAll('.edit-btn');

      expect(editButtons).toHaveLength(3);
      editButtons.forEach((button: any) => {
        expect(button.textContent.trim()).toBe('✏️');
      });
    });

    it('should disable edit button when todo is being updated', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set([2]));
      fixture.detectChanges();

      const editButtons = fixture.nativeElement.querySelectorAll('.edit-btn');

      expect(editButtons[0].disabled).toBe(false); // ID 1
      expect(editButtons[1].disabled).toBe(true); // ID 2 - being updated
      expect(editButtons[2].disabled).toBe(false); // ID 3
    });

    it('should show spinner in todo title when updating', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set([2]));
      fixture.detectChanges();

      const todoTitles = fixture.nativeElement.querySelectorAll('.todo-title');
      const spinner = todoTitles[1].querySelector('.inline-spinner');

      expect(spinner).toBeTruthy(); // ID 2 - showing inline spinner
    });

    it('should call startEdit when edit button is clicked', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      jest.spyOn(component, 'startEdit');
      const editButtons = fixture.nativeElement.querySelectorAll('.edit-btn');

      editButtons[1].click(); // Click second todo's edit button

      expect(component.startEdit).toHaveBeenCalledWith(mockTodos[1], expect.any(Object));
    });

    it('should call toggleCompletion when todo title is clicked', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      jest.spyOn(component, 'toggleCompletion');
      const todoTitles = fixture.nativeElement.querySelectorAll('.todo-title');

      todoTitles[1].click(); // Click second todo's title

      expect(component.toggleCompletion).toHaveBeenCalledWith(mockTodos[1]);
    });

    it('should show edit mode when todo is being edited', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      component.startEdit(mockTodos[1]);
      fixture.detectChanges();

      const todoItems = fixture.nativeElement.querySelectorAll('.todo-item');
      const editInput = todoItems[1].querySelector('.edit-input');
      const saveButton = todoItems[1].querySelector('.save-btn');
      const cancelButton = todoItems[1].querySelector('.cancel-btn');

      expect(todoItems[1].classList.contains('editing')).toBe(true);
      expect(editInput).toBeTruthy();
      expect(saveButton).toBeTruthy();
      expect(cancelButton).toBeTruthy();
    });

    it('should call saveEdit when save button is clicked in edit mode', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      component.startEdit(mockTodos[1]);
      fixture.detectChanges();

      jest.spyOn(component, 'saveEdit');
      const saveButton = fixture.nativeElement.querySelector('.save-btn');

      saveButton.click();

      expect(component.saveEdit).toHaveBeenCalledWith(mockTodos[1]);
    });

    it('should call cancelEdit when cancel button is clicked in edit mode', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      component.startEdit(mockTodos[1]);
      fixture.detectChanges();

      jest.spyOn(component, 'cancelEdit');
      const cancelButton = fixture.nativeElement.querySelector('.cancel-btn');

      cancelButton.click();

      expect(component.cancelEdit).toHaveBeenCalled();
    });

    it('should show spinner in save button when updating', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set([2]));
      fixture.detectChanges();

      component.startEdit(mockTodos[1]);
      fixture.detectChanges();

      const saveButton = fixture.nativeElement.querySelector('.save-btn');
      const spinner = saveButton.querySelector('.mini-spinner');

      expect(spinner).toBeTruthy();
    });

    it('should apply completed class to completed todos', () => {
      mockTodoStore.todos.mockReturnValue(mockTodos);
      mockTodoStore.updatingIds.mockReturnValue(new Set());
      fixture.detectChanges();

      const todoItems = fixture.nativeElement.querySelectorAll('.todo-item');

      expect(todoItems[0].classList.contains('completed')).toBe(false); // ID 1 - not completed
      expect(todoItems[1].classList.contains('completed')).toBe(false); // ID 2 - not completed
      expect(todoItems[2].classList.contains('completed')).toBe(true); // ID 3 - completed
    });
  });
});
