import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { TokenService } from '../../../core/services/token.service';

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
  Token: string;
  UserId: string;
  FullName: string;
  Email: string;
  Role: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(TokenService);
  private readonly authUrl = 'http://localhost:5002/api/Auth';

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/register`, request)
      .pipe(tap((response) => this.tokenService.setToken(response.Token)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/login`, request)
      .pipe(tap((response) => this.tokenService.setToken(response.Token)));
  }
}
