import { isPlatformBrowser } from '@angular/common';
import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private readonly storageKey = 'pharmacy_token';
  private readonly roleStorageKey = 'pharmacy_role';
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));
  readonly role = signal<string | null>(this.readRole());
  readonly authenticated = signal(this.readToken() !== null);

  setToken(token: string): void {
    if (this.isBrowser) {
      localStorage.setItem(this.storageKey, token);
    }
    this.authenticated.set(true);
  }

  setRole(role: string): void {
    if (this.isBrowser) {
      localStorage.setItem(this.roleStorageKey, role);
    }
    this.role.set(role);
  }

  getToken(): string | null {
    return this.readToken();
  }

  getRole(): string | null {
    return this.role();
  }

  removeToken(): void {
    if (this.isBrowser) {
      localStorage.removeItem(this.storageKey);
      localStorage.removeItem(this.roleStorageKey);
    }
    this.authenticated.set(false);
    this.role.set(null);
  }

  private readToken(): string | null {
    return this.isBrowser ? localStorage.getItem(this.storageKey) : null;
  }

  private readRole(): string | null {
    return this.isBrowser ? localStorage.getItem(this.roleStorageKey) : null;
  }
}
