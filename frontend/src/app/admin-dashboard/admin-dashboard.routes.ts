import { Routes } from '@angular/router';
import { AdminDashboardLayoutComponent } from './layout/admin-dashboard-layout/admin-dashboard-layout.component';
import { isAdminGuard } from '../auth/guards/is-admin.guard';
import { AnnoucementListPageComponent } from './pages/annoucement-list-page/annoucement-list-page.component';
import { CreateAnnouncementPageComponent } from './pages/create-announcement-page/create-announcement-page.component';
import { EditAnnouncementPageComponent } from './pages/edit-announcement-page/edit-announcement-page.component';

export const adminDashboardRoutes: Routes = [
  {
    path: '',
    component: AdminDashboardLayoutComponent,
    canMatch: [isAdminGuard],
    children: [
      {
        path: 'announcements',
        children: [
          {
            path: '',
            component: AnnoucementListPageComponent, // Lista de anuncios
            title: 'Anuncios - Administración'
          },
          {
            path: 'create',
            component: CreateAnnouncementPageComponent, // Crear nuevo anuncio
            title: 'Crear Anuncio - Administración'
          },
          {
            path: 'edit/:id',
            component: EditAnnouncementPageComponent, // Editar anuncio existente
            title: 'Editar Anuncio - Administración'
          }
        ]
      }
    ],
  },

];

export default adminDashboardRoutes;
