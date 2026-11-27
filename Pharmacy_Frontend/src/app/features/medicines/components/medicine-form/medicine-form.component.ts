import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { CategoryService } from '../../../categories/services/category.service';
import { Category } from '../../../categories/models/category.model';
import { Medicine } from '../../models/medicine.model';
import { MedicineService } from '../../services/medicine.service';

function optionalHttpUrl(control: AbstractControl): ValidationErrors | null {
  const value = String(control.value).trim();
  if (!value) {
    return null;
  }

  try {
    const url = new URL(value);
    return (url.protocol === 'http:' || url.protocol === 'https:') && url.hostname
      ? null
      : { url: true };
  } catch {
    return { url: true };
  }
}

@Component({
  selector: 'app-medicine-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './medicine-form.component.html',
  styleUrl: './medicine-form.component.css'
})
export class MedicineFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly medicineService = inject(MedicineService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly form = this.formBuilder.nonNullable.group({
    name: [
      '',
      [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(150),
        control => String(control.value).trim().length > 0 ? null : { whitespace: true }
      ]
    ],
    description: ['', [Validators.maxLength(1000)]],
    price: [0, [Validators.required, Validators.min(0.01), Validators.max(999999999)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]],
    imageUrl: ['', [Validators.maxLength(500), optionalHttpUrl]],
    requiresPrescription: [false],
    categoryId: [0, [Validators.required, Validators.min(1)]],
    manufacturer: [
      '',
      [
        Validators.required,
        Validators.maxLength(150),
        control => String(control.value).trim().length > 0 ? null : { whitespace: true }
      ]
    ],
    isActive: [true]
  });

  readonly categories = signal<Category[]>([]);
  readonly editMode = signal(false);
  readonly loading = signal(false);
  readonly categoriesLoading = signal(true);
  readonly submitting = signal(false);
  readonly formReady = signal(false);
  readonly errorMessage = signal('');

  private medicineId: number | null = null;
  private allCategories: Category[] = [];
  private loadedMedicine: Medicine | null = null;
  private categoriesLoaded = false;
  private medicineLoaded = false;

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');

    if (idParam === null) {
      this.loadCategories();
      return;
    }

    const id = Number(idParam);
    if (!Number.isInteger(id) || id < 1) {
      this.errorMessage.set('The medicine ID is invalid.');
      return;
    }

    this.medicineId = id;
    this.editMode.set(true);
    this.loadCategories();
    this.loadMedicine(id);
  }

  submit(): void {
    this.form.markAllAsTouched();
    this.errorMessage.set('');

    if (this.form.invalid || this.submitting() || !this.formReady()) {
      return;
    }

    const values = this.form.getRawValue();
    const data = {
      name: values.name.trim(),
      description: values.description.trim() || null,
      price: values.price,
      stockQuantity: values.stockQuantity,
      imageUrl: values.imageUrl.trim() || null,
      requiresPrescription: values.requiresPrescription,
      categoryId: values.categoryId,
      manufacturer: values.manufacturer.trim(),
      isActive: values.isActive
    };

    this.submitting.set(true);
    const request = this.editMode()
      ? this.medicineService.updateMedicine(this.medicineId!, data)
      : this.medicineService.createMedicine(data);

    request
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.submitting.set(false))
      )
      .subscribe({
        next: () => this.router.navigate(['/medicines']),
        error: error => this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  private loadMedicine(id: number): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.medicineService
      .getMedicineById(id)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: medicine => {
          this.loadedMedicine = medicine;
          this.form.patchValue({
            name: medicine.name,
            description: medicine.description ?? '',
            price: medicine.price,
            stockQuantity: medicine.stockQuantity,
            imageUrl: medicine.imageUrl ?? '',
            requiresPrescription: medicine.requiresPrescription,
            categoryId: medicine.categoryId,
            manufacturer: medicine.manufacturer,
            isActive: medicine.isActive
          });
          this.updateCategoryOptions();
          this.updateFormReady();
        },
        error: error => this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  private loadCategories(): void {
    this.categoriesLoading.set(true);
    this.categoryService
      .getAll()
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.categoriesLoading.set(false))
      )
      .subscribe({
        next: categories => {
          this.allCategories = categories;
          this.categoriesLoaded = true;
          this.updateCategoryOptions();
          this.updateFormReady();
          if (!this.editMode() && this.categories().length === 0) {
            this.errorMessage.set('No active categories are available. Add or activate a category first.');
          }
        },
        error: error => this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  private updateCategoryOptions(): void {
    const assignedCategoryId = this.loadedMedicine?.categoryId;
    const selectableCategories = this.allCategories.filter(
      category => category.isActive || category.id === assignedCategoryId
    );

    if (
      this.loadedMedicine &&
      !selectableCategories.some(category => category.id === this.loadedMedicine?.categoryId)
    ) {
      selectableCategories.push({
        id: this.loadedMedicine.categoryId,
        name: this.loadedMedicine.categoryName,
        description: null,
        isActive: false,
        createdAt: this.loadedMedicine.createdAt
      });
    }

    this.categories.set(selectableCategories);
  }

  private updateFormReady(): void {
    this.medicineLoaded = !this.editMode() || this.loadedMedicine !== null;
    this.formReady.set(this.categoriesLoaded && this.medicineLoaded);
  }

  private getErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) {
        return 'Cannot connect to the API server. Check that the backend is running and its HTTPS certificate is trusted.';
      }

      const body: unknown = error.error;
      if (typeof body === 'object' && body !== null) {
        if ('message' in body && typeof body.message === 'string') {
          return body.message;
        }
        if ('title' in body && typeof body.title === 'string') {
          return body.title;
        }
        if ('errors' in body && typeof body.errors === 'object' && body.errors !== null) {
          const validationMessages = Object.values(body.errors)
            .flatMap(messages => Array.isArray(messages) ? messages : [])
            .filter((message): message is string => typeof message === 'string');
          if (validationMessages.length > 0) {
            return validationMessages.join(' ');
          }
        }
      }

      if (error.status === 404) {
        return 'Medicine not found.';
      }

      return `The server returned an error (${error.status}).`;
    }

    return 'An unexpected error occurred.';
  }
}
