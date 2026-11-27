import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { Medicine } from '../../models/medicine.model';
import { MedicineService } from '../../services/medicine.service';

@Component({
  selector: 'app-medicine-details',
  standalone: true,
  imports: [DatePipe, DecimalPipe, RouterLink],
  templateUrl: './medicine-details.component.html',
  styleUrl: './medicine-details.component.css'
})
export class MedicineDetailsComponent implements OnInit {
  private readonly medicineService = inject(MedicineService);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);

  readonly medicine = signal<Medicine | null>(null);
  readonly loading = signal(true);
  readonly imageUnavailable = signal(false);
  readonly errorMessage = signal('');
  private medicineId: number | null = null;

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = Number(idParam);

    if (idParam === null || !Number.isInteger(id) || id < 1) {
      this.loading.set(false);
      this.errorMessage.set('The medicine ID is invalid.');
      return;
    }

    this.medicineId = id;
    this.loadMedicine(id);
  }

  retry(): void {
    if (this.medicineId !== null) {
      this.loadMedicine(this.medicineId);
    }
  }

  loadMedicine(id: number): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.imageUnavailable.set(false);

    this.medicineService
      .getMedicineById(id)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: medicine => this.medicine.set(medicine),
        error: error => {
          this.medicine.set(null);
          this.errorMessage.set(this.getErrorMessage(error));
        }
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
