export enum UserRole {
  Owner = 1,
  Manager = 2,
  Staff = 3,
  Receptionist = 4,
  Client = 5,
}

export interface UserMembership {
  businessId: string;
  businessName: string;
  role: UserRole;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  memberships: UserMembership[];
}

export interface RegisterBusinessRequest {
  businessName: string;
  businessRuc: string;
  businessAddress: string;
  businessPhoneNumber?: string;
  businessEmail?: string;
}

export interface RegisterBusinessResponse {
  userId: string;
  businessId: string;
  email: string;
}

export interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}
