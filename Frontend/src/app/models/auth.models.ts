export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  confirmPassword: string;
  fullName?: string;
}

export interface AuthResponseDto {
  token: string;
  email: string;
  fullName?: string;
  expiration: string;
}

export interface UserProfile {
  email: string;
  fullName: string;
  userName: string;
}
