import { Component, computed, HostListener, signal } from '@angular/core';
import { MenuItem } from '../../../admin-dashboard/interfaces/menu-item.interface';
import { StatsCard } from '../../../admin-dashboard/interfaces/stats-card.interface';
import { AuthorNavbarComponent } from '../../components/author-navbar/author-navbar.component';
import { AuthorSidebarComponent } from '../../components/author-sidebar/author-sidebar.component';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-author-dashboard-layout',
  imports: [CommonModule, AuthorNavbarComponent, AuthorSidebarComponent, RouterOutlet],
  templateUrl: './author-dashboard-layout.component.html',
})
export class AuthorDashboardLayoutComponent {
  private _isSidebarOpen = signal(true);
  private _screenWidth = signal(window.innerWidth);

  // Computed signals
  isSidebarOpen = computed(() => this._isSidebarOpen());
  isMobile = computed(() => this._screenWidth() < 1024);
  sidebarWidth = computed(() => this.isMobile() ? '280px' : '240px');

  // Menu items (podrían ser inputs si se desea)
  authorMenuItems: MenuItem[] = [
    {
      label: 'dashboard',
      icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z"></path><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v0H8v0z"></path></svg>`,
      route: '/write/home'
    },
    {
      label: 'Publicaciones',
      icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path></svg>`,
      route: '/write/books'
      // children: []
    }
  ];

  // Stats cards (podrían ser inputs si se desea)
  statsCards: StatsCard[] = [
    {
      title: 'Users',
      value: '2',
      link: 'View'
    },
    {
      title: 'Companies',
      value: '100',
      change: '+30%',
      changeType: 'positive',
      link: 'View'
    },
    {
      title: 'Blogs',
      value: '100',
      link: 'View'
    }
  ];

  // Activities (podrían ser inputs si se desea)
  activities = [
    { title: 'Lorem Ipsum', date: '02-02-2024', time: '17:45' },
    { title: 'Lorem Ipsum', date: '02-02-2024', time: '17:45' }
  ];

   constructor() {
    this._isSidebarOpen.set(!this.isMobile());
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this._screenWidth.set(event.target.innerWidth);
    if (this.isMobile() && this.isSidebarOpen()) {
      this._isSidebarOpen.set(false);
    }
  }

  toggleSidebar() {
    this._isSidebarOpen.update(open => !open);
  }

  closeSidebar() {
    if (this.isMobile()) {
      this._isSidebarOpen.set(false);
    }
  }
}
