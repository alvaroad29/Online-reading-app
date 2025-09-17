import { Component, inject, signal } from '@angular/core';
import { LucideAngularModule, SquarePen } from 'lucide-angular';
import { AuthService } from '../../../auth/services/auth.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'profile-page',
  imports: [LucideAngularModule, RouterLink],
  templateUrl: './profile-page.component.html',
})
export class ProfilePageComponent {

  authService = inject(AuthService);
  user = this.authService.user;

  memberSince = new Date(2022, 0, 20); // Jan 20, 2022


  // Iconos
  readonly editIcon = SquarePen;



  // imagenes
  image = signal<string[]>([]);
  onFileChange( event: Event ) {
    const file = (event.target as HTMLInputElement).files;

    const imageUrl = Array.from( file ?? []).map((file) => URL.createObjectURL(file)); //crear url temporal

    this.image.set(imageUrl);
  }

}
