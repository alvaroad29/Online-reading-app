import { Routes } from '@angular/router';
import { NotAuthenticatedGuard } from './auth/guards/not-authenticated.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.routes'),
    canMatch: [
      NotAuthenticatedGuard
    ]
  },
  {
    path: 'admin',
    loadChildren: () => import('./admin-dashboard/admin-dashboard.routes'),
  },
  {
    path: 'write',
    loadChildren: () => import('./author-dashboard/author-dashboard.routes'),
  },
  {
    path: '',
    loadChildren: () => import('./library-front//library-front.routes'),
  },
];
