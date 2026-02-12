export enum UserRole {
  Owner = 1,
  Manager = 2,
  Staff = 3,
  Receptionist = 4,
  Client = 5,
}

export interface Membership {
  businessId: string;
  role: UserRole;
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

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    memberships: Membership[];
  };
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  businessName: string;
  businessRuc: string;
  businessAddress: string;
  businessPhoneNumber?: string;
  businessEmail?: string;
}

export interface RegisterResponse {
  userId: string;
  businessId: string;
  email: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export interface AuthState {
  user: User | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}
