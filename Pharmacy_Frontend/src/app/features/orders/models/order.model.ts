export type OrderStatus =
  | 'Placed'
  | 'Confirmed'
  | 'Processing'
  | 'Shipped'
  | 'Delivered'
  | 'Cancelled';

export interface Order {
  id: number;
  orderId?: number;
  orderDate: string | Date;
  deliveryAddress: string;
  paymentMethod: string;
  status: OrderStatus;
  totalAmount: number;
  items: OrderItem[];
  prescriptionId?: number | null;
  prescriptionStatus?: string;
}

export interface OrderItem {
  id?: number;
  orderItemId?: number;
  medicineId: number;
  medicineName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  subTotal?: number;
}

export interface CreateOrderRequest {
  deliveryAddress: string;
  paymentMethod: 'COD';
  prescriptionId?: number | null;
}
