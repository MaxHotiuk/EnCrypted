import { CanActivateFn } from '@angular/router';

export const nonauthGuard: CanActivateFn = (route, state) => {
  return true;
};
