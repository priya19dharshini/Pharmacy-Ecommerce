import { HttpErrorResponse } from '@angular/common/http';
import { DecimalPipe } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { Medicine } from '../../models/medicine.model';
import { MedicineService } from '../../services/medicine.service';

@Component({
  selector: 'app-medicine-list',
  standalone: true,
  imports: [DecimalPipe, RouterLink],
  templateUrl: './medicine-list.component.html',
  styleUrl: './medicine-list.component.css'
})
export class MedicineListComponent implements OnInit {
  private readonly medicineService = inject(MedicineService);
  private readonly destroyRef = inject(DestroyRef);

  readonly medicines = signal<Medicine[]>([]);
  readonly loading = signal(true);
  readonly deletingId = signal<number | null>(null);
  readonly errorMessage = signal('');
  readonly successMessage = signal('');

  ngOnInit(): void {
    this.loadMedicines();
  }

  loadMedicines(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.medicineService
      .getAllMedicines()
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: medicines => this.medicines.set(medicines),
        error: error => this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  deleteMedicine(medicine: Medicine): void {
    if (!window.confirm(`Delete "${medicine.name}"?`)) {
      return;
    }

    this.deletingId.set(medicine.id);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.medicineService
      .deleteMedicine(medicine.id)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.deletingId.set(null))
      )
      .subscribe({
        next: () => {
          this.successMessage.set('Medicine deleted successfully.');
          this.loadMedicines();
        },
        error: error => this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  private getErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) {
        return 'Cannot connect to the API server. Check that the backend is running and its HTTPS certificate is trusted.';
      }

      const body: unknown = error.error;
      if (typeof body === 'object' && body !== null && 'message' in body) {
        const message = body.message;
        if (typeof message === 'string' && message.length > 0) {
          return message;
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
