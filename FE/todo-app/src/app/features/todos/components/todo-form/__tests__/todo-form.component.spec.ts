import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { TodoFormComponent } from '../todo-form.component';
import { TodoStore } from '../../../stores/todo-store';
import { ApiService } from '../../../services/api.service';
import { TodoItem } from '../../../types/todo-item';

describe('TodoFormComponent', () => {
  let component: TodoFormComponent;
  let fixture: ComponentFixture<TodoFormComponent>;
  let mockTodoStore: jest.Mocked<TodoStore>;
  let mockApiService: jest.Mocked<ApiService>;

  const mockTodoItem: TodoItem = {
    id: 1,
    title: 'Test Todo',
    isCompleted: false,
  };

  beforeEach(async () => {
    const apiServiceSpy = {
      addTodo: jest.fn(),
      getTodos: jest.fn(),
      deleteTodo: jest.fn(),
    };

    const todoStoreSpy = {
      addTodo: jest.fn(),
      clearError: jest.fn(),
      isAdding: jest.fn().mockReturnValue(false),
      error: jest.fn().mockReturnValue(null),
      todos: jest.fn().mockReturnValue([]),
      isLoading: jest.fn().mockReturnValue(false),
      deletingIds: jest.fn().mockReturnValue(new Set()),
      updatingIds: jest.fn().mockReturnValue(new Set()),
      loadTodos: jest.fn(),
      deleteTodo: jest.fn(),
      updateTodo: jest.fn(),
      setLoading: jest.fn(),
      todosCount: jest.fn().mockReturnValue(0),
      completedTodosCount: jest.fn().mockReturnValue(0),
      hasTodos: jest.fn().mockReturnValue(false),
      isDeleting: jest.fn().mockReturnValue(false),
      isUpdating: jest.fn().mockReturnValue(false),
      hasError: jest.fn().mockReturnValue(false),
    };

    await TestBed.configureTestingModule({
      imports: [TodoFormComponent, FormsModule],
      providers: [
        { provide: ApiService, useValue: apiServiceSpy },
        { provide: TodoStore, useValue: todoStoreSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoFormComponent);
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

  it('should initialize with empty title', () => {
    expect(component.newTodoTitle).toBe('');
  });

  describe('addTodo', () => {
    it('should not call store when title is empty', () => {
      component.newTodoTitle = '';

      component.addTodo();

      expect(mockTodoStore.addTodo).not.toHaveBeenCalled();
    });

    it('should not call store when title is only whitespace', () => {
      component.newTodoTitle = '   ';

      component.addTodo();

      expect(mockTodoStore.addTodo).not.toHaveBeenCalled();
    });

    it('should call store addTodo and clear title when valid title provided', () => {
      const testTitle = 'New Todo';
      component.newTodoTitle = testTitle;

      component.addTodo();

      expect(mockTodoStore.addTodo).toHaveBeenCalledWith(testTitle);
      expect(component.newTodoTitle).toBe('');
    });
  });

  describe('clearError', () => {
    it('should call store clearError method', () => {
      component.clearError();

      expect(mockTodoStore.clearError).toHaveBeenCalled();
    });
  });

  describe('template interactions', () => {
    it('should disable button when store is adding', () => {
      // Reset the mock to ensure it's properly configured
      mockTodoStore.isAdding.mockClear();
      mockTodoStore.isAdding.mockReturnValue(true);

      fixture.detectChanges();

      const button = fixture.nativeElement.querySelector('button');

      expect(button.disabled).toBe(true);
      expect(mockTodoStore.isAdding).toHaveBeenCalled();
    });

    it('should show "Adding..." text with spinner on button when store is adding', () => {
      mockTodoStore.isAdding.mockReturnValue(true);
      fixture.detectChanges();

      const button = fixture.nativeElement.querySelector('button');
      const spinner = fixture.nativeElement.querySelector('.mini-spinner');

      expect(button.textContent.trim()).toContain('Adding...');
      expect(spinner).toBeTruthy();
    });

    it('should show "Add" text without spinner on button when store is not adding', () => {
      mockTodoStore.isAdding.mockReturnValue(false);
      fixture.detectChanges();

      const button = fixture.nativeElement.querySelector('button');
      const spinner = fixture.nativeElement.querySelector('.mini-spinner');

      expect(button.textContent.trim()).toBe('Add');
      expect(spinner).toBeFalsy();
    });

    it('should call addTodo when Enter key is pressed', () => {
      jest.spyOn(component, 'addTodo');
      const input = fixture.nativeElement.querySelector('input');

      input.value = 'Test Todo';
      input.dispatchEvent(new KeyboardEvent('keyup', { key: 'Enter' }));

      expect(component.addTodo).toHaveBeenCalled();
    });

    it('should call addTodo when Add button is clicked', () => {
      jest.spyOn(component, 'addTodo');
      const button = fixture.nativeElement.querySelector('button');

      button.click();

      expect(component.addTodo).toHaveBeenCalled();
    });

    it('should display error message when store has error', () => {
      mockTodoStore.error.mockReturnValue('Test error message');
      fixture.detectChanges();

      const errorDiv = fixture.nativeElement.querySelector('.error-message');
      const errorText = fixture.nativeElement.querySelector('.error-message p');

      expect(errorDiv).toBeTruthy();
      expect(errorText.textContent.trim()).toBe('Test error message');
    });

    it('should not display error message when store has no error', () => {
      mockTodoStore.error.mockReturnValue(null);
      fixture.detectChanges();

      const errorDiv = fixture.nativeElement.querySelector('.error-message');

      expect(errorDiv).toBeFalsy();
    });

    it('should call clearError when dismiss button is clicked', () => {
      mockTodoStore.error.mockReturnValue('Test error message');
      fixture.detectChanges();

      jest.spyOn(component, 'clearError');
      const dismissButton = fixture.nativeElement.querySelector('.error-message button');

      dismissButton.click();

      expect(component.clearError).toHaveBeenCalled();
    });
  });
});
