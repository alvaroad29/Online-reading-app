import { Routes } from '@angular/router';
import { LibraryFrontLayoutComponent } from './layouts/library-front-layout/library-front-layout.component';
import { HomePageComponent } from './pages/home-page/home-page.component';
import { BookPageComponent } from './pages/book-page/book-page.component';
import { ReaderPageComponent } from './pages/reader-page/reader-page.component';
import { LibraryPageComponent } from './pages/library-page/library-page.component';
import { AuthenticatedGuard } from './guards/authenticated.guard';
import { AnnoucementPageComponent } from './pages/annoucement-page/annoucement-page.component';

export const libraryFrontRoutes: Routes = [
  {
    path: '',
    component: LibraryFrontLayoutComponent,
    children: [
      {
        path: '',
        component: HomePageComponent // ver dsp si lo cargo de manera perezosa
      },
      {
        path: 'library',
        component: LibraryPageComponent,
      },
      {
        path: 'annoucement',
        component: AnnoucementPageComponent
      },
      {
        path: 'library/:bookId',
        component: BookPageComponent,
      },
      {
        path: 'library/:bookId/:chapterId',
        component: ReaderPageComponent // Muestra SOLO el lector
      },
      {
        path: 'user',
        loadChildren: () => import('../user-front/user-front.routes'),
        canMatch: [
          AuthenticatedGuard
        ]
      }

    ],
  },
  {
    path: '**',
    redirectTo: ''
  }
];

export default libraryFrontRoutes; // para dsp exportarlo en app.routes.ts
