import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Order } from '../../models/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order-details.html',
  styleUrl: './order-details.css'
})
export class OrderDetailsComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly orderService = inject(OrderService);

  order: Order | null = null;
  loading = true;
  error = '';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isInteger(id) || id <= 0) {
      this.error = 'Invalid order ID.';
      this.loading = false;
      return;
    }

    this.orderService.getOrderById(id).subscribe({
      next: order => {
        this.order = order;
        this.loading = false;
      },
      error: (error: HttpErrorResponse) => {
        const payload = error.error;
        this.error = typeof payload === 'string'
          ? payload
          : payload?.message
            ?? payload?.title
            ?? (error.status === 403 ? 'You are not allowed to view this order.'
              : error.status === 404 ? 'Order not found.'
              : 'Unable to load this order.');
        this.loading = false;
      }
    });
  }
}
