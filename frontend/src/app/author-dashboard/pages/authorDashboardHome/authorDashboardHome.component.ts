import { DecimalPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-author-dashboard-home',
  imports: [DecimalPipe, RouterLink],
  templateUrl: './authorDashboardHome.component.html',
})
export class AuthorDashboardHomeComponent {
   private router = inject(Router);
   // Datos simulados o obtenidos del backend
  stats = {
    fictions: 0,
    totalChapters: 0,
    totalWords: 0,
    reviewsReceived: 0,
    uniqueFollowers: 0
  };

  constructor() {
    this.loadStats();
  }

  loadStats() {
    // Simulación de llamada al backend
    // En producción: this.writerService.getStats().subscribe(...)
    setTimeout(() => {
      this.stats = {
        fictions: 2,
        totalChapters: 15,
        totalWords: 34876,
        reviewsReceived: 8,
        uniqueFollowers: 45
      };
    }, 500);
  }

  addNewBook() {
    this.router.navigate(['/writer/books/new']);
  }
}
