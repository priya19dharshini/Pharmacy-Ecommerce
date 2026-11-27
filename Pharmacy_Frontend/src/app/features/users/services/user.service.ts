import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { UpdateUserRequest, UserProfile } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly profileUrl = 'http://localhost:5002/api/Users/profile';

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(this.profileUrl);
  }

  updateProfile(update: UpdateUserRequest): Observable<unknown> {
    return this.http.put<unknown>(this.profileUrl, update);
  }
}
