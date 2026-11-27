import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  if (typeof localStorage === 'undefined') return next(request);

  const token = localStorage.getItem('authToken')
    ?? localStorage.getItem('accessToken')
    ?? localStorage.getItem('token');

  return token
    ? next(request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }))
    : next(request);
};
