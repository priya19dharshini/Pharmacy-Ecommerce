import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreateOrderRequest, Order, OrderItem } from '../models/order.model';

interface OrderResponse {
  orderId: number;
  orderDate: string;
  deliveryAddress: string;
  paymentMethod: string;
  status: Order['status'];
  totalAmount: number;
  items: Array<{
    orderItemId?: number;
    medicineId: number;
    medicineName: string;
    quantity: number;
    unitPrice: number;
    subTotal: number;
  }>;
  prescriptionId?: number | null;
  prescriptionStatus?: string;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/orders`;

  placeOrder(request: CreateOrderRequest): Observable<Order> {
    return this.http.post<OrderResponse>(this.baseUrl, request)
      .pipe(map(order => this.mapOrder(order)));
  }

  getMyOrders(): Observable<Order[]> {
    return this.http.get<OrderResponse[]>(`${this.baseUrl}/my`)
      .pipe(map(orders => orders.map(order => this.mapOrder(order))));
  }

  getOrderById(id: number): Observable<Order> {
    return this.http.get<OrderResponse>(`${this.baseUrl}/${id}`)
      .pipe(map(order => this.mapOrder(order)));
  }

  cancelOrder(id: number): Observable<unknown> {
    return this.http.put(`${this.baseUrl}/${id}/cancel`, {});
  }

  getAllOrders(): Observable<Order[]> {
    return this.http.get<OrderResponse[]>(this.baseUrl)
      .pipe(map(orders => orders.map(order => this.mapOrder(order))));
  }

  updateOrderStatus(id: number, status: string): Observable<unknown> {
    return this.http.put(`${this.baseUrl}/${id}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  private mapOrder(response: OrderResponse): Order {
    const items: OrderItem[] = (response.items ?? []).map(item => ({
      id: item.orderItemId,
      orderItemId: item.orderItemId,
      medicineId: item.medicineId,
      medicineName: item.medicineName,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      subtotal: item.subTotal ?? item.unitPrice * item.quantity,
      subTotal: item.subTotal
    }));

    return {
      id: response.orderId,
      orderId: response.orderId,
      orderDate: response.orderDate,
      deliveryAddress: response.deliveryAddress,
      paymentMethod: response.paymentMethod,
      status: response.status,
      totalAmount: response.totalAmount,
      items,
      prescriptionId: response.prescriptionId,
      prescriptionStatus: response.prescriptionStatus
    };
  }
}
