import { Component, inject, input } from '@angular/core';
import { SlideData } from '../../../interfaces/slidedata.interface';
import { SlicePipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';



@Component({
  selector: 'carousel-slide',
  imports: [SlicePipe, RouterLink],
  templateUrl: './carousel-slide.component.html',
  styleUrls: ['./carousel-slide.component.css']

})
export class CarouselSlideComponent {
  slide = input<SlideData>();

  private router = inject(Router);

  navigateToDetails() {
    if (this.slide()?.id) {
      this.router.navigate(['/title', this.slide()?.id]);
    }
  }
}
