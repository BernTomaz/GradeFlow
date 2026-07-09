import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthApiService } from '../api/auth-api.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(AuthApiService).token;
  return next(token ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request);
};
