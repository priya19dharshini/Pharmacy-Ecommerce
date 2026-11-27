import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Order } from '../../models/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order-list.html',
  styleUrl: './order-list.css'
})
export class OrderListComponent implements OnInit {
  private readonly orderService = inject(OrderService);

  orders: Order[] = [];
  loading = true;
  error = '';
  success = '';
  cancellingId: number | null = null;
  confirmingId: number | null = null;

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.error = '';
    this.orderService.getMyOrders().subscribe({
      next: orders => {
        this.orders = [...orders].sort((a, b) =>
          new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime());
        this.loading = false;
      },
      error: error => {
        this.setError(error);
        this.loading = false;
      }
    });
  }

  canCancel(order: Order): boolean {
    return order.status === 'Placed';
  }

  requestCancel(order: Order): void {
    if (this.confirmingId === order.id) {
      this.confirmCancel(order.id);
      return;
    }
    this.confirmingId = order.id;
  }

  cancelConfirmation(order: Order): boolean {
    return this.confirmingId === order.id;
  }

  private confirmCancel(id: number): void {
    this.cancellingId = id;
    this.success = '';
    this.error = '';
    this.orderService.cancelOrder(id).subscribe({
      next: () => {
        this.confirmingId = null;
        this.cancellingId = null;
        this.success = 'Order cancelled successfully.';
        this.loadOrders();
      },
      error: error => {
        this.confirmingId = null;
        this.cancellingId = null;
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
        ?? (error.status === 403 ? 'You are not allowed to access these orders.'
          : error.status === 404 ? 'Order not found.'
          : 'Unable to load or cancel the order.');
  }
}
