import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TodoFormComponent } from './components/todo-form/todo-form.component';
import { TodoListComponent } from './components/todo-list/todo-list.component';
import { TodoStore } from './stores/todo-store';

@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [CommonModule, TodoFormComponent, TodoListComponent],
  providers: [TodoStore], // Provide the store at component level
  templateUrl: './todos.component.html',
  styleUrls: ['./todos.component.scss'],
})
export class TodosComponent {
  constructor(public todoStore: TodoStore) {}
}
