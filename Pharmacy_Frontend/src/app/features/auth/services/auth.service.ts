import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { TokenService } from '../../../core/services/token.service';
import { environment } from '../../../../environments/environment';

export interface RegisterRequest {
  FullName: string;
  Email: string;
  Password: string;
  PhoneNumber: string;
  Address: string;
}

export interface LoginRequest {
  Email: string;
  Password: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  fullName: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(TokenService);
  private readonly authUrl = `${environment.apiUrl}/Auth`;

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/register`, request)
      .pipe(tap((response) => this.saveAuth(response)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/login`, request)
      .pipe(tap((response) => this.saveAuth(response)));
  }

  logout(): void {
    this.tokenService.removeToken();
  }

  private saveAuth(response: AuthResponse): void {
    this.tokenService.setToken(response.token);
    this.tokenService.setRole(response.role);
  }
}
