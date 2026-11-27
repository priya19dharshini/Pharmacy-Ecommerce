import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { TokenService } from '../../../../core/services/token.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-profile',
  imports: [ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly userService = inject(UserService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly profileId = signal('');
  readonly errorMessage = signal('');
  readonly successMessage = signal('');
  readonly form = this.formBuilder.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: [{ value: '', disabled: true }],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^[0-9+()\s-]{7,20}$/)]],
    address: ['', [Validators.required, Validators.minLength(3)]],
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  saveProfile(): void {
    this.errorMessage.set('');
    this.successMessage.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.getRawValue();
    this.saving.set(true);
    this.userService
      .updateProfile({
        fullName: formValue.fullName,
        phoneNumber: formValue.phoneNumber,
        address: formValue.address,
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => this.successMessage.set('Your profile has been updated.'),
        error: (error: unknown) => {
          this.errorMessage.set(this.describeError(error));
        },
      });
  }

  logout(): void {
    this.tokenService.removeToken();
    void this.router.navigate(['/auth/login']);
  }

  private loadProfile(): void {
    this.errorMessage.set('');
    this.loading.set(true);

    this.userService
      .getProfile()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (profile) => {
          this.profileId.set(profile.id);
          this.form.patchValue({
            fullName: profile.fullName,
            email: profile.email,
            phoneNumber: profile.phoneNumber,
            address: profile.address,
          });
        },
        error: (error: unknown) => {
          this.errorMessage.set(this.describeError(error));
        },
      });
  }

  private describeError(error: unknown): string {
    if (error instanceof HttpErrorResponse && error.status === 0) {
      return 'Unable to reach the pharmacy server. Please try again.';
    }

    if (error instanceof HttpErrorResponse && error.status === 401) {
      return 'Your session has expired. Please sign in again.';
    }

    return 'We could not complete that request. Please try again.';
  }
}
