import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../services/auth.service';

const passwordsMatch: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('Password')?.value;
  const confirmation = control.get('ConfirmPassword')?.value;

  if (!password || !confirmation || password === confirmation) {
    return null;
  }

  return { passwordMismatch: true };
};

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly form = this.formBuilder.group(
    {
      FullName: ['', [Validators.required, Validators.minLength(2)]],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(8)]],
      ConfirmPassword: ['', Validators.required],
      PhoneNumber: ['', [Validators.required, Validators.pattern(/^[0-9+()\s-]{7,20}$/)]],
      Address: ['', [Validators.required, Validators.minLength(3)]],
    },
    { validators: passwordsMatch },
  );

  submit(): void {
    this.errorMessage.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.getRawValue();
    this.loading.set(true);
    this.authService
      .register({
        FullName: formValue.FullName,
        Email: formValue.Email,
        Password: formValue.Password,
        PhoneNumber: formValue.PhoneNumber,
        Address: formValue.Address,
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => {
          const destination = response.role.toLowerCase() === 'admin'
            ? '/categories'
            : '/medicines';
          void this.router.navigateByUrl(destination);
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

    if (error instanceof HttpErrorResponse && error.status === 409) {
      return 'An account with this email may already exist.';
    }

    return 'Registration failed. Check your details and try again.';
  }
}
