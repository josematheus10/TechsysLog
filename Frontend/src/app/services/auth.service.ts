import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginDto, RegisterDto, AuthResponseDto, UserProfile } from '../models/auth.models';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7063/api/Auth';
  private tokenKey = 'auth_token';
  
  // Signal to track login state
  isAuthenticated = signal<boolean>(this.hasToken());
  currentUser = signal<AuthResponseDto | null>(null);

  constructor(private http: HttpClient, private router: Router) {}

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  login(credentials: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        this.setSession(response);
      })
    );
  }

  register(data: RegisterDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, data).pipe(
      tap(response => {
        this.setSession(response);
      })
    );
  }

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/profile`);
  }

  logout(): void {
    // Call backend logout if needed, but primarily client-side cleanup
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
      next: () => console.log('Backend logout called'),
      error: (err) => console.error('Backend logout error', err)
    });

    localStorage.removeItem(this.tokenKey);
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private setSession(authResult: AuthResponseDto): void {
    localStorage.setItem(this.tokenKey, authResult.token);
    this.isAuthenticated.set(true);
    this.currentUser.set(authResult);
    // You might want to decode token or just store the user details
  }
}
