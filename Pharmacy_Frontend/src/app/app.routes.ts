import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.routes').then((route) => route.authRoutes),
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/users/components/profile/profile.component').then(
        (component) => component.ProfileComponent,
      ),
    path: 'categories',
    loadChildren: () =>
      import('./features/categories/categories.routes').then(
        route => route.CATEGORY_ROUTES
      )
  },
  {
    path: 'medicines',
    loadChildren: () =>
      import('./features/medicines/medicines.routes').then(
        route => route.MEDICINE_ROUTES
      )
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'auth/login',
  },
  {
    path: '**',
    redirectTo: 'auth/login',
  },
    redirectTo: 'categories'
  },
  {
    path: '**',
    redirectTo: 'categories'
  }
];
