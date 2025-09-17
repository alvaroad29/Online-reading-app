import { Component, signal, computed, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminNavbarComponent } from '../../components/admin-navbar/admin-navbar.component';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { StatsCardsComponent } from '../../components/stats-cards/stats-cards.component';
import { MenuItem } from '../../interfaces/menu-item.interface';
import { StatsCard } from '../../interfaces/stats-card.interface';
import { RouterOutlet } from '@angular/router';


@Component({
  selector: 'app-admin-dashboard-layout',
  standalone: true,
  imports: [CommonModule, AdminNavbarComponent, AdminSidebarComponent, RouterOutlet],
  templateUrl: './admin-dashboard-layout.component.html',
})
export class AdminDashboardLayoutComponent {
  // Signals
  private _isSidebarOpen = signal(true);
  private _screenWidth = signal(window.innerWidth);

  // Computed signals
  isSidebarOpen = computed(() => this._isSidebarOpen());
  isMobile = computed(() => this._screenWidth() < 1024);
  sidebarWidth = computed(() => this.isMobile() ? '280px' : '240px');

  // Menu items (podrían ser inputs si se desea)
  adminMenuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z"></path><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v0H8v0z"></path></svg>`
    },
    {
      label: 'Usuarios',
      icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path></svg>`,
    },
    // {
    //   label: 'Activities',
    //   icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"></path></svg>`
    // }
  ];

  booksMenuItems: MenuItem[] = [
    {
      label: 'libros',
      icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z"></path></svg>`,
    },
    {
      label: 'pendientes',
      icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 8l6 6 6-6"></path></svg>`
    },
    {
      label: 'reportados',
      icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 8l6 6 6-6"></path></svg>`
    }
  ];

  annoucementMenuItems: MenuItem[] = [
    {
      label: 'anuncios',
      icon: `<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-3.6-3.6A8.014 8.014 0 0017 10V9a8 8 0 10-16 0v1c0 1.566-.44 3.081-1.25 4.4L1 17h5m9 0v1a3 3 0 11-6 0v-1m6 0H9"></path></svg>`,
      route: '/admin/announcements'
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
