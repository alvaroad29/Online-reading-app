// front-navbar.component.ts
import { Component, HostListener, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterModule } from '@angular/router';
import { LucideAngularModule, User } from 'lucide-angular';
import { ToggleThemeComponent } from "../../../shared/components/toggle-theme/toggle-theme.component";
import { AuthService } from '../../../auth/services/auth.service';

@Component({
  selector: 'front-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule, ToggleThemeComponent, RouterLink, RouterLinkActive],
  templateUrl: './front-navbar.component.html',
  // styleUrls: ['./front-navbar.component.css']
})
export class FrontNavbarComponent {
  authService = inject(AuthService);
  authStatus = this.authService.authStatus;
  currentUser = this.authService.user;

  isMenuOpen = false;
  isUserDropdownOpen = false;
  lastScrollPosition = 0;
  isNavbarHidden = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  toggleUserDropdown() {
    this.isUserDropdownOpen = !this.isUserDropdownOpen;
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    const currentScrollPosition = window.pageYOffset;

    if (currentScrollPosition > 100) {
      this.isNavbarHidden = currentScrollPosition > this.lastScrollPosition;
    } else {
      this.isNavbarHidden = false;
    }

    this.lastScrollPosition = currentScrollPosition;
  }


  // Iconos
  readonly user = User;


}
