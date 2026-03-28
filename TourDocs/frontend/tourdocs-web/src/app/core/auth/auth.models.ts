export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  organizationName: string;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  tokenType: string;
  // User info returned by backend AuthResponse
  userId?: string;
  fullName?: string;
  email?: string;
  role?: string;
  organizationId?: string;
  organizationName?: string;
}

export interface RefreshTokenRequest {
  accessToken: string;
  refreshToken: string;
}

export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  avatarUrl: string | null;
  roles: string[];
  organizationId: string;
  organizationName: string;
  isActive: boolean;
  createdAt: string;
  lastLoginAt: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
  confirmPassword: string;
}

export interface JwtPayload {
  sub: string;
  email: string;
  given_name: string;
  family_name: string;
  roles: string[];
  org_id: string;
  exp: number;
  iat: number;
}
