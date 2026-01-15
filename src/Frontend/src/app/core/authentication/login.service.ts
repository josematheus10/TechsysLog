import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map } from 'rxjs';

import { Menu } from '@core';
import { Token, User, RegisterRequest, LoginRequest } from './interface';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  protected readonly http = inject(HttpClient);

  register(email: string, password: string, confirmPassword: string, fullName?: string) {
    const request: RegisterRequest = { email, password, confirmPassword, fullName };
    return this.http.post<Token>('/api/Auth/register', request);
  }

  login(username: string, password: string, rememberMe = false) {
    const request: LoginRequest = { email: username, password };
    return this.http.post<Token>('/api/Auth/login', request);
  }

  refresh(params: Record<string, any>) {
    return this.http.post<Token>('/auth/refresh', params);
  }

  logout() {
    return this.http.post<any>('/api/Auth/logout', {});
  }

  user() {
    return this.http.get<User>('/api/Auth/profile');
  }

  menu() {
    return this.http.get<{ menu: Menu[] }>('/user/menu').pipe(map(res => res.menu));
  }
}
