import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { AuthResponse, ChangeNameRequest, ChangePasswordRequest, LoginRequest, RegisterRequest, SetupAdminRequest, SetupStatusResponse } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly storageKey = 'gradeflow.auth';
  private readonly baseUrl = '/api/auth';
  private readonly setupUrl = '/api/setup';
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

  setupStatus() {
    return this.http.get<SetupStatusResponse>(`${this.setupUrl}/status`);
  }

  createSetupAdmin(request: SetupAdminRequest) {
    return this.http.post<AuthResponse>(`${this.setupUrl}/admin`, request);
  }

  refreshToken() {
    return this.http.post<AuthResponse>(`${this.baseUrl}/refresh-token`, {});
  }

  changePassword(request: ChangePasswordRequest) {
    return this.http.post<void>(`${this.baseUrl}/change-password`, request);
  }

  changeName(request: ChangeNameRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/change-name`, request);
  }

  save(response: AuthResponse) {
    localStorage.removeItem(this.storageKey);
    sessionStorage.setItem(this.storageKey, JSON.stringify(response));
    this.current.set(response);
  }

  logout() {
    localStorage.removeItem(this.storageKey);
    sessionStorage.removeItem(this.storageKey);
    this.current.set(null);
  }

  private load() {
    const value = sessionStorage.getItem(this.storageKey);
    localStorage.removeItem(this.storageKey);
    try {
      return value ? (JSON.parse(value) as AuthResponse) : null;
    } catch {
      sessionStorage.removeItem(this.storageKey);
      return null;
    }
  }
}
