import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  Category,
  CreateCategory,
  UpdateCategory
} from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly categoriesUrl = `${environment.apiUrl}/categories`;

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.categoriesUrl);
  }

  getById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.categoriesUrl}/${id}`);
  }

  create(dto: CreateCategory): Observable<Category> {
    return this.http.post<Category>(this.categoriesUrl, dto);
  }

  update(id: number, dto: UpdateCategory): Observable<Category> {
    return this.http.put<Category>(`${this.categoriesUrl}/${id}`, dto);
  }

  delete(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.categoriesUrl}/${id}`
    );
  }
}
