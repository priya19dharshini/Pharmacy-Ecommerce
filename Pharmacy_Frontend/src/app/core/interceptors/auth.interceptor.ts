import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';

const API_BASE_URL = 'http://localhost:5002';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  const isApiRequest = req.url.startsWith(`${API_BASE_URL}/`);
  const isAuthRequest = req.url.startsWith(`${API_BASE_URL}/api/Auth/`);
  const token = tokenService.getToken();

  if (!isApiRequest || isAuthRequest || !token) {
    return next(req);
  }

  const authenticatedRequest = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(authenticatedRequest).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        tokenService.removeToken();

        const queryParams = router.url.startsWith('/auth/')
          ? undefined
          : { returnUrl: router.url };

        void router.navigate(['/auth/login'], { queryParams });
      }

      return throwError(() => error);
    }),
  );
};
