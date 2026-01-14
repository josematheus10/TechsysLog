export interface User {
  [prop: string]: any;

  id?: number | string | null;
  userName?: string;
  name?: string;
  email?: string;
  fullName?: string;
  avatar?: string;
  roles?: any[];
  permissions?: any[];
}

export interface Token {
  [prop: string]: any;

  token: string;
  access_token?: string;
  email?: string;
  fullName?: string;
  expiration?: string;
  token_type?: string;
  expires_in?: number;
  exp?: number;
  refresh_token?: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  fullName?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}
