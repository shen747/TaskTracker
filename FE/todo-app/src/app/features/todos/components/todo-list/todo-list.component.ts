import { Component, input, signal, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoStore } from '../../stores/todo-store';
import { TodoItem } from '../../types/todo-item';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.scss'],
})
export class TodoListComponent implements AfterViewInit {
  todoStore = input.required<TodoStore>();
  editingId = signal<number | null>(null);
  editTitle = signal<string>('');
  @ViewChild('editInput') editInput!: ElementRef<HTMLInputElement>;

  deleteTodo(id: number): void {
    this.todoStore().deleteTodo(id);
  }

  loadTodos(): void {
    this.todoStore().loadTodos();
  }

  clearError(): void {
    this.todoStore().clearError();
  }

  isDeleting(id: number): boolean {
    return this.todoStore().deletingIds().has(id);
  }

  isUpdating(id: number): boolean {
    return this.todoStore().updatingIds().has(id);
  }

  startEdit(todo: TodoItem, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    this.editingId.set(todo.id);
    this.editTitle.set(todo.title);
    // Focus the input after the view updates
    setTimeout(() => {
      if (this.editInput) {
        this.editInput.nativeElement.focus();
        this.editInput.nativeElement.select();
      }
    }, 0);
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.editTitle.set('');
  }

  saveEdit(todo: TodoItem): void {
    const newTitle = this.editTitle().trim();
    if (newTitle && newTitle !== todo.title) {
      this.todoStore().updateTodo({
        id: todo.id,
        title: newTitle,
        isCompleted: todo.isCompleted,
      });
    }
    this.cancelEdit();
  }

  toggleCompletion(todo: TodoItem): void {
    this.todoStore().updateTodo({
      id: todo.id,
      title: todo.title,
      isCompleted: !todo.isCompleted,
    });
  }

  isEditing(id: number): boolean {
    return this.editingId() === id;
  }

  ngAfterViewInit(): void {
    // This method is required by the AfterViewInit interface
    // We don't need to do anything here since we handle focus in startEdit
  }
}
