import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.css'
})
export class CategoryFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly form = this.formBuilder.nonNullable.group({
    name: [
      '',
      [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(100),
        control => control.value.trim().length > 0 ? null : { whitespace: true }
      ]
    ],
    description: ['', [Validators.maxLength(500)]],
    isActive: [true]
  });
  readonly editMode = signal(false);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly formReady = signal(false);
  readonly errorMessage = signal('');

  private categoryId: number | null = null;

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');

    if (idParam === null) {
      this.formReady.set(true);
      return;
    }

    const id = Number(idParam);
    if (!Number.isInteger(id) || id < 1) {
      this.errorMessage.set('The category ID is invalid.');
      return;
    }

    this.categoryId = id;
    this.editMode.set(true);
    this.loadCategory(id);
  }

  submit(): void {
    this.form.markAllAsTouched();
    this.errorMessage.set('');

    if (this.form.invalid || this.submitting() || !this.formReady()) {
      return;
    }

    const name = this.form.controls.name.value.trim();
    const description = this.form.controls.description.value.trim();
    this.submitting.set(true);

    const request = this.editMode()
      ? this.categoryService.update(this.categoryId!, {
          name,
          description,
          isActive: this.form.controls.isActive.value
        })
      : this.categoryService.create({ name, description });

    request
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.submitting.set(false))
      )
      .subscribe({
        next: () => this.router.navigate(['/categories']),
        error: error => this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  private loadCategory(id: number): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.categoryService
      .getById(id)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: category => {
          this.form.patchValue({
            name: category.name,
            description: category.description ?? '',
            isActive: category.isActive
          });
          this.formReady.set(true);
        },
        error: error => this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  private getErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      const responseMessage =
        typeof error.error === 'object' && error.error?.message
          ? error.error.message
          : '';

      if (error.status === 0) {
        return 'Cannot connect to the API server. Please ensure the backend is running.';
      }

      if (error.status === 404) {
        return responseMessage || 'Category not found.';
      }

      if (error.status === 409) {
        return responseMessage || 'A category with this name already exists.';
      }

      return responseMessage || `The server returned an error (${error.status}).`;
    }

    return 'An unexpected error occurred.';
  }
}
