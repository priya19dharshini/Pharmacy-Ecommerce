import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { Category } from '../../models/category.model';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css'
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);
  private readonly destroyRef = inject(DestroyRef);

  readonly categories = signal<Category[]>([]);
  readonly loading = signal(true);
  readonly deletingId = signal<number | null>(null);
  readonly errorMessage = signal('');
  readonly successMessage = signal('');

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.categoryService
      .getAll()
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: categories => this.categories.set(categories),
        error: error => {
          this.errorMessage.set(this.getErrorMessage(error));
          this.categories.set([]);
        }
      });
  }

  deleteCategory(category: Category): void {
    if (!window.confirm(`Delete "${category.name}"?`)) {
      return;
    }

    this.deletingId.set(category.id);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.categoryService
      .delete(category.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.deletingId.set(null);
          this.successMessage.set('Category deleted successfully.');
          this.loadCategories();
        },
        error: error => {
          this.deletingId.set(null);
          this.errorMessage.set(this.getErrorMessage(error));
        }
      });
  }

  private getErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) {
        return 'Cannot connect to the API server. Please ensure the backend is running.';
      }

      const responseMessage =
        typeof error.error === 'object' && error.error?.message
          ? error.error.message
          : '';

      if (error.status === 404) {
        return responseMessage || 'Category not found.';
      }

      return responseMessage || `The server returned an error (${error.status}).`;
    }

    return 'An unexpected error occurred.';
  }
}
