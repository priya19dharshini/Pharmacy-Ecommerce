import { isPlatformBrowser } from '@angular/common';
import { Injectable, PLATFORM_ID, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private readonly storageKey = 'pharmacy_token';
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  setToken(token: string): void {
    if (this.isBrowser) {
      localStorage.setItem(this.storageKey, token);
    }
  }

  getToken(): string | null {
    return this.isBrowser ? localStorage.getItem(this.storageKey) : null;
  }

  removeToken(): void {
    if (this.isBrowser) {
      localStorage.removeItem(this.storageKey);
    }
  }
}
