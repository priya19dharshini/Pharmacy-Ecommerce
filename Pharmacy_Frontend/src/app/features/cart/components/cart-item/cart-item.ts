import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartItem } from '../../models/cart.model';

@Component({
  selector: 'app-cart-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart-item.html',
  styleUrl: './cart-item.css'
})
export class CartItemComponent {
  @Input({ required: true }) item!: CartItem;
  @Output() quantityChange = new EventEmitter<{ itemId: number; quantity: number }>();
  @Output() remove = new EventEmitter<number>();

  changeQuantity(quantity: number): void {
    if (quantity < 1) return;
    if (this.item.stockQuantity !== undefined && quantity > this.item.stockQuantity) return;
    this.quantityChange.emit({ itemId: this.item.id, quantity });
  }

  isDecrementDisabled(): boolean {
    return this.item.quantity <= 1;
  }

  isIncrementDisabled(): boolean {
    return this.item.stockQuantity !== undefined
      && this.item.quantity >= this.item.stockQuantity;
  }

  removeItem(): void {
    this.remove.emit(this.item.id);
  }
}
