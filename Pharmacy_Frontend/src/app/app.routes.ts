import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { CheckoutComponent } from './features/orders/components/checkout/checkout';

export const routes: Routes = [
  { path: '', redirectTo: 'cart', pathMatch: 'full' },
  {
    path: 'cart',
    loadChildren: () => import('./features/cart/cart.routes')
      .then(module => module.CART_ROUTES)
  },
  { path: 'checkout', component: CheckoutComponent, canActivate: [authGuard] },
  {
    path: 'orders',
    loadChildren: () => import('./features/orders/order.routes')
      .then(module => module.ORDER_ROUTES)
  },
  { path: '**', redirectTo: 'cart' }
];
