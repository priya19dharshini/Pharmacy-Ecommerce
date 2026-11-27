import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { CartListComponent } from './components/cart-list/cart-list';

export const CART_ROUTES: Routes = [
  { path: '', component: CartListComponent, canActivate: [authGuard] }
];
