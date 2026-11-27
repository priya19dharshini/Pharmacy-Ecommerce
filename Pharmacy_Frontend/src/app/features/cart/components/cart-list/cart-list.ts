import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Cart } from '../../models/cart.model';
import { CartService } from '../../services/cart.service';
import { CartItemComponent } from '../cart-item/cart-item';

@Component({
  selector: 'app-cart-list',
  standalone: true,
  imports: [CommonModule, RouterLink, CartItemComponent],
  templateUrl: './cart-list.html',
  styleUrl: './cart-list.css'
})
export class CartListComponent implements OnInit {
  private readonly cartService = inject(CartService);

  cart: Cart | null = null;
  loading = true;
  error = '';
  success = '';
  confirmingClear = false;

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading = true;
    this.error = '';
    this.cartService.getCart().subscribe({
      next: cart => this.cart = cart,
      error: error => this.setError(error),
      complete: () => this.loading = false
    });
  }

  updateQuantity(event: { itemId: number; quantity: number }): void {
    this.error = '';
    this.success = '';
    this.cartService.updateCartItem(event.itemId, event.quantity).subscribe({
      next: cart => {
        this.cart = cart;
        this.success = 'Cart quantity updated.';
      },
      error: error => this.setError(error)
    });
  }

  removeItem(itemId: number): void {
    this.cartService.removeCartItem(itemId).subscribe({
      next: () => this.loadCart(),
      error: error => this.setError(error)
    });
  }

  clearCart(): void {
    if (!this.confirmingClear) {
      this.confirmingClear = true;
      return;
    }

    this.cartService.clearCart().subscribe({
      next: () => {
        this.cart = { id: 0, items: [], totalItems: 0, totalAmount: 0 };
        this.confirmingClear = false;
        this.success = 'Cart cleared.';
      },
      error: error => {
        this.confirmingClear = false;
        this.setError(error);
      }
    });
  }

  private setError(error: HttpErrorResponse): void {
    this.loading = false;
    const payload = error.error;
    this.error = typeof payload === 'string'
      ? payload
      : payload?.message
        ?? payload?.title
        ?? (error.status === 404 ? 'The cart or medicine could not be found.'
          : error.status === 400 ? 'Invalid cart request.'
          : 'Unable to update the cart. Please try again.');
  }
}
