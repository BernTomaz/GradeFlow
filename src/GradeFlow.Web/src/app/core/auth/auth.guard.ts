import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { UserRole } from '../models/auth.models';
import { AuthApiService } from '../api/auth-api.service';

export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthApiService);
  const router = inject(Router);
  const current = auth.current();
  if (!current) return router.createUrlTree(['/login']);

  const roles = route.data['roles'] as UserRole[] | undefined;
  return !roles || roles.includes(current.user.role) ? true : router.createUrlTree(['/assignments']);
};
