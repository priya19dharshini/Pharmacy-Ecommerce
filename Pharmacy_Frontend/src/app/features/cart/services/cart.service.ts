import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, map, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Cart, CartItem } from '../models/cart.model';

interface CartResponse {
  cartId: number;
  items: Array<{
    cartItemId: number;
    medicineId: number;
    medicineName: string;
    unitPrice: number;
    quantity: number;
    subTotal: number;
  }>;
  totalAmount: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/cart`;
  private readonly cartSubject = new BehaviorSubject<Cart | null>(null);

  readonly cart$ = this.cartSubject.asObservable();

  getCart(): Observable<Cart> {
    return this.http.get<CartResponse>(this.baseUrl).pipe(
      map(response => this.mapCart(response)),
      tap(cart => this.cartSubject.next(cart))
    );
  }

  addToCart(medicineId: number, quantity = 1): Observable<Cart> {
    return this.http.post<CartResponse>(`${this.baseUrl}/items`, {
      medicineId,
      quantity
    }).pipe(
      map(response => this.mapCart(response)),
      tap(cart => this.cartSubject.next(cart))
    );
  }

  updateCartItem(itemId: number, quantity: number): Observable<Cart> {
    return this.http.put<CartResponse>(`${this.baseUrl}/items/${itemId}`, {
      quantity
    }).pipe(
      map(response => this.mapCart(response)),
      tap(cart => this.cartSubject.next(cart))
    );
  }

  removeCartItem(itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/items/${itemId}`).pipe(
      tap(() => this.refreshAfterMutation())
    );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(this.baseUrl).pipe(
      tap(() => this.cartSubject.next(this.emptyCart()))
    );
  }

  getCartItemCount(): Observable<number> {
    return new BehaviorSubject(this.cartSubject.value?.totalItems ?? 0).asObservable();
  }

  refreshCount(): void {
    this.getCart().subscribe({
      error: () => this.cartSubject.next(this.emptyCart())
    });
  }

  private refreshAfterMutation(): void {
    this.getCart().subscribe();
  }

  private mapCart(response: CartResponse): Cart {
    const items: CartItem[] = (response.items ?? []).map(item => ({
      id: item.cartItemId,
      cartItemId: item.cartItemId,
      medicineId: item.medicineId,
      medicineName: item.medicineName,
      unitPrice: item.unitPrice,
      quantity: item.quantity,
      subtotal: item.subTotal ?? item.unitPrice * item.quantity,
      subTotal: item.subTotal
    }));

    return {
      id: response.cartId,
      cartId: response.cartId,
      items,
      totalItems: items.reduce((sum, item) => sum + item.quantity, 0),
      totalAmount: response.totalAmount
        ?? items.reduce((sum, item) => sum + item.subtotal, 0)
    };
  }

  private emptyCart(): Cart {
    return { id: 0, items: [], totalItems: 0, totalAmount: 0 };
  }
}
