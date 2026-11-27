
import { Component, DestroyRef, effect, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  RouterLink,
  RouterLinkActive,
  Router,
  RouterOutlet
} from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { CartService } from './features/cart/services/cart.service';
import { TokenService } from './core/services/token.service';

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
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  private readonly refreshCustomerCart = effect(() => {
    if (this.tokenService.getRole()?.toLowerCase() === 'customer') {
      this.cartService.refreshCount();
    }
  });

  cartCount = 0;

  ngOnInit(): void {
    this.cartService.cart$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(cart => {
        this.cartCount = cart?.totalItems ?? 0;
      });
  }

  get isAuthenticated(): boolean {
    return this.tokenService.authenticated();
  }

  get isAdmin(): boolean {
    return this.tokenService.getRole()?.toLowerCase() === 'admin';
  }

  get isCustomer(): boolean {
    return this.tokenService.getRole()?.toLowerCase() === 'customer';
  }

  get isAuthPage(): boolean {
    return this.router.url.startsWith('/auth/');
  }

  logout(): void {
    this.tokenService.removeToken();
    void this.router.navigate(['/auth/login']);
  }
}
