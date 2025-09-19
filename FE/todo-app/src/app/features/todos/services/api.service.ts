import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TodoItem } from '../types/todo-item';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5279/api/v1/todos';

  getTodos(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.baseUrl);
  }

  addTodo(title: string): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.baseUrl, { title });
  }

  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  updateTodo(id: number, title: string, isCompleted: boolean): Observable<TodoItem> {
    return this.http.put<TodoItem>(`${this.baseUrl}/${id}`, { title, isCompleted });
  }
}
