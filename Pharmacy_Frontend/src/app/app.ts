
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';

import { CartService } from './features/cart/services/cart.service';

@Component({
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive
  ],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html'
})
export class App implements OnInit {
  private readonly cartService = inject(CartService);

  cartCount = 0;

  ngOnInit(): void {
    this.cartService.cart$.subscribe(cart => {
      this.cartCount = cart?.totalItems ?? 0;
    });

    this.cartService.refreshCount();
  }

  get isAuthenticated(): boolean {
    if (typeof localStorage === 'undefined') {
      return false;
    }

    return !!(
      localStorage.getItem('authToken') ??
      localStorage.getItem('accessToken') ??
      localStorage.getItem('token')
    );
  }
}
