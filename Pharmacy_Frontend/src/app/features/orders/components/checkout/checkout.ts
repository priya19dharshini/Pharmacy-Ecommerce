import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Cart } from '../../../cart/models/cart.model';
import { CartService } from '../../../cart/services/cart.service';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css'
})
export class CheckoutComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly cartService = inject(CartService);
  private readonly orderService = inject(OrderService);
  private readonly router = inject(Router);

  cart: Cart | null = null;
  loading = true;
  submitting = false;
  error = '';

  readonly form = this.fb.nonNullable.group({
    deliveryAddress: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
    paymentMethod: ['COD' as 'COD', Validators.required],
    prescriptionId: [null as number | null]
  });

  ngOnInit(): void {
    this.cartService.getCart().subscribe({
      next: cart => {
        this.cart = cart;
        this.loading = false;
      },
      error: error => {
        this.loading = false;
        this.setError(error);
      }
    });
  }

  placeOrder(): void {
    if (this.submitting) return;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    if (!this.cart?.items.length) {
      this.error = 'Your cart is empty.';
      return;
    }

    this.submitting = true;
    this.error = '';
    const value = this.form.getRawValue();
    this.orderService.placeOrder({
      deliveryAddress: value.deliveryAddress.trim(),
      paymentMethod: 'COD',
      prescriptionId: value.prescriptionId
    }).subscribe({
      next: order => {
        this.submitting = false;
        this.cartService.clearCart().subscribe({ complete: () => undefined });
        this.router.navigate(['/orders', order.id]);
      },
      error: error => {
        this.submitting = false;
        this.setError(error);
      }
    });
  }

  private setError(error: HttpErrorResponse): void {
    const payload = error.error;
    this.error = typeof payload === 'string'
      ? payload
      : payload?.message
        ?? payload?.title
        ?? (error.status === 400 ? 'Please check the cart and delivery details.'
          : error.status === 404 ? 'The order or medicine could not be found.'
          : 'Unable to place the order. Please try again.');
  }
}
