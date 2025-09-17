import { Routes } from '@angular/router';
import { AuthorDashboardLayoutComponent } from './layout/author-dashboard-layout/author-dashboard-layout.component';
import { AuthorDashboardHomeComponent } from './pages/authorDashboardHome/authorDashboardHome.component';
import { MyBooksPageComponent } from './pages/my-books-page/my-books-page.component';
import { CreateBookPageComponent } from './pages/create-book-page/create-book-page.component';


export const authorDashboardRoutes: Routes = [
  {
    path: '',
    component: AuthorDashboardLayoutComponent,
    children: [
      {
        path: 'home',
        component: AuthorDashboardHomeComponent
      },
      {
        path: 'books',
        component: MyBooksPageComponent
      },
      {
        path: 'books/new',
        component: CreateBookPageComponent
      },
      // {
      //   path: 'books/:bookId',
      //   component: BookEditorComponent
      // },
      // {
      //   path: 'books/:bookId/chapters',
      //   component: ChapterListComponent
      // },
      // {
      //   path: 'books/:bookId/chapters/new',
      //   component: ChapterEditorComponent
      // },
      // {
      //   path: 'books/:bookId/chapters/:chapterId',
      //   component: ChapterEditorComponent
      // },
      // {
      //   path: 'stats',
      //   component: AuthorStatsComponent
      // },
      // {
      //   path: 'drafts',
      //   component: DraftsComponent
      // }
      {
        path: '**',
        redirectTo: 'home'
      }
    ],
  },

];

export default authorDashboardRoutes;
