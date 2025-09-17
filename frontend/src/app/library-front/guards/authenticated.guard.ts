import { inject } from '@angular/core';
import { CanMatchFn, Route, Router, UrlSegment } from '@angular/router';

import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../auth/services/auth.service';

export const AuthenticatedGuard: CanMatchFn = async (
  route: Route,
  segments: UrlSegment[]
) => {

  const authService = inject(AuthService);

  const router = inject(Router); // para redireccionar

  const isAuthenticated = await firstValueFrom( authService.checkStatus() ); // true o false

  if ( !isAuthenticated ) {
    router.navigateByUrl('/');
    return false; // para que no vea la ruta
  }

  return true;
}
