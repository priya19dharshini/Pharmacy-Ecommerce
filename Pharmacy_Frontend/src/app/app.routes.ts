
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { CheckoutComponent } from './features/orders/components/checkout/checkout';

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
  },
  {
    path: 'categories',
    canActivate: [roleGuard(['Admin'])],
    loadChildren: () =>
      import('./features/categories/categories.routes').then(
        route => route.CATEGORY_ROUTES
      )
  },
  {
    path: 'medicines',
    canActivate: [roleGuard(['Admin', 'Customer'])],
    loadChildren: () =>
      import('./features/medicines/medicines.routes').then(
        route => route.MEDICINE_ROUTES
      )
  },
  {
    path: 'cart',
    loadChildren: () =>
      import('./features/cart/cart.routes').then(
        module => module.CART_ROUTES
      )
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
    canActivate: [authGuard]
  },
  {
    path: 'orders',
    loadChildren: () =>
      import('./features/orders/order.routes').then(
        module => module.ORDER_ROUTES
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
];
