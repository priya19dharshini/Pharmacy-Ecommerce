import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { CheckoutComponent } from './components/checkout/checkout';
import { OrderListComponent } from './components/order-list/order-list';
import { OrderDetailsComponent } from './components/order-details/order-details';

export const ORDER_ROUTES: Routes = [
  { path: '', component: OrderListComponent, canActivate: [authGuard] },
  { path: ':id', component: OrderDetailsComponent, canActivate: [authGuard] }
];
