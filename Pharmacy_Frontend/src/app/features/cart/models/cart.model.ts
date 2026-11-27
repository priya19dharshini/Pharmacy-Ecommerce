export interface Cart {
  id: number;
  cartId?: number;
  userId?: string;
  items: CartItem[];
  totalItems: number;
  totalAmount: number;
  imageUrl?: string;
}

export interface CartItem {
  id: number;
  cartItemId?: number;
  medicineId: number;
  medicineName: string;
  imageUrl?: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  subTotal?: number;
  stockQuantity?: number;
}

export interface AddToCartRequest {
  medicineId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}
