export interface Medicine {
  id: number;
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  imageUrl: string | null;
  requiresPrescription: boolean;
  categoryId: number;
  categoryName: string;
  manufacturer: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateMedicine {
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  imageUrl: string | null;
  requiresPrescription: boolean;
  categoryId: number;
  manufacturer: string;
  isActive: boolean;
}

export interface UpdateMedicine {
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  imageUrl: string | null;
  requiresPrescription: boolean;
  categoryId: number;
  manufacturer: string;
  isActive: boolean;
}
