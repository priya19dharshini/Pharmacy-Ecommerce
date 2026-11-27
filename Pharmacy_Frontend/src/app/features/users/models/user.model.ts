export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  address: string;
}

export interface UpdateUserRequest {
  fullName: string;
  phoneNumber: string;
  address: string;
}
