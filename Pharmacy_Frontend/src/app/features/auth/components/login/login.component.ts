import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly form = this.formBuilder.group({
    Email: ['', [Validators.required, Validators.email]],
    Password: ['', Validators.required],
  });

  submit(): void {
    this.errorMessage.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.authService
      .login(this.form.getRawValue())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => {
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
          const safeReturnUrl =
            returnUrl?.startsWith('/') && !returnUrl.startsWith('//')
              ? returnUrl
              : null;
          const destination = safeReturnUrl
            ?? (response.role.toLowerCase() === 'admin'
              ? '/categories'
              : '/medicines');

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

    return 'Sign-in failed. Check your email and password, then try again.';
  }
}
