import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';

export function roleGuard(allowedRoles: readonly string[]): CanActivateFn {
  return (_route, state) => {
    const tokenService = inject(TokenService);
    const router = inject(Router);

    if (!tokenService.getToken()) {
      return router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: state.url }
      });
    }

    const role = tokenService.getRole();
    if (role && allowedRoles.some(allowed => allowed.toLowerCase() === role.toLowerCase())) {
      return true;
    }

    if (role?.toLowerCase() === 'admin') {
      return router.parseUrl('/categories');
    }

    if (role?.toLowerCase() === 'customer') {
      return router.parseUrl('/medicines');
    }

    return router.parseUrl('/auth/login');
  };
}
