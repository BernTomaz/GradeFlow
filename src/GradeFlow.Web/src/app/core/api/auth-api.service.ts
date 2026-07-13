import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { AuthResponse, ChangePasswordRequest, LoginRequest, RegisterRequest } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly storageKey = 'gradeflow.auth';
  private readonly baseUrl = '/api/auth';
  readonly current = signal<AuthResponse | null>(this.load());

  constructor(private readonly http: HttpClient) {}

  get token() {
    return this.current()?.token ?? null;
  }

  login(request: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request);
  }

  register(request: RegisterRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, request);
  }

  refreshToken() {
    return this.http.post<AuthResponse>(`${this.baseUrl}/refresh-token`, {});
  }

  changePassword(request: ChangePasswordRequest) {
    return this.http.post<void>(`${this.baseUrl}/change-password`, request);
  }

  save(response: AuthResponse) {
    localStorage.setItem(this.storageKey, JSON.stringify(response));
    this.current.set(response);
  }

  logout() {
    localStorage.removeItem(this.storageKey);
    this.current.set(null);
  }

  private load() {
    const value = localStorage.getItem(this.storageKey);
    return value ? (JSON.parse(value) as AuthResponse) : null;
  }
}
