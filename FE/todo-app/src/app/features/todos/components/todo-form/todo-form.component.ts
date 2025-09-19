import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoStore } from '../../stores/todo-store';

@Component({
  selector: 'app-todo-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './todo-form.component.html',
  styleUrls: ['./todo-form.component.scss'],
})
export class TodoFormComponent {
  todoStore = input.required<TodoStore>();

  newTodoTitle = '';

  addTodo(): void {
    if (!this.newTodoTitle.trim()) return;

    const store = this.todoStore();
    store.addTodo(this.newTodoTitle);
    this.newTodoTitle = '';
  }

  clearError(): void {
    this.todoStore().clearError();
  }
}
