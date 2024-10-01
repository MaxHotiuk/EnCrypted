import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

export const RoleGuard: CanActivateFn = (route, state) => {
  const roles = route.data['roles'] as string[];
  const authService = inject(AuthService);
  const matSnackBar = inject(MatSnackBar);
  const router = inject(Router);

  const userRoles = authService.getUserRoles() || [];

  if (!authService.isLoggedIn()) {
    matSnackBar.open('You are not logged in', 'Close',{
        duration: 3000,
      }
    );
    router.navigate(['/login']);
    return false;
  }

  if (roles.some((role) => userRoles?.includes(role))) {
    return true;
  }

  router.navigate(['/']);
  matSnackBar.open('You are not authorized to view this page', 'Close',{
      duration: 3000,
    }
  );

  return false;
};
