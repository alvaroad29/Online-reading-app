import { Routes } from '@angular/router';
import { UserFrontLayoutComponent } from './layouts/user-front-layout/user-front-layout.component';
import { ProfilePageComponent } from './pages/profile-page/profile-page.component';
import { ListsPageComponent } from './pages/lists-page/lists-page.component';
import { HistoryPageComponent } from './pages/history-page/history-page.component';
import { ProfileEditPageComponent } from './pages/profile-edit-page/profile-edit-page.component';


export const userFrontRoutes: Routes = [
  {
    path: '',
    component: UserFrontLayoutComponent, // tiene el user-top-menu
    children: [
      {
        path: 'profile',
        component: ProfilePageComponent
      },
      {
        path: 'profile/edit',
        component: ProfileEditPageComponent
      },
      {
        path: 'history',
        component: HistoryPageComponent
      },
      {
        path: 'lists',
        component: ListsPageComponent
      },
      {
        path: '**',
        redirectTo: 'profile'
      }
    ],
  },

];

export default userFrontRoutes; // para dsp exportarlo en app.routes.ts
