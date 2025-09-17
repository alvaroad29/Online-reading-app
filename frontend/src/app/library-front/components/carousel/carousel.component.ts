import { SlideData } from './../../interfaces/slidedata.interface';
import { AfterViewInit, Component, ElementRef, signal, viewChild, ViewEncapsulation } from '@angular/core';

// import Swiper JS
import Swiper from 'swiper';
// import Swiper styles
import 'swiper/css';
import 'swiper/css/navigation';
import 'swiper/css/pagination';
import { Navigation, Pagination } from 'swiper/modules';
import { CarouselSlideComponent } from "./carousel-slide/carousel-slide.component";

@Component({
  selector: 'app-carousel',
  imports: [CarouselSlideComponent],
  templateUrl: './carousel.component.html',
  styleUrls: ['carousel.component.css'],
  // encapsulation: ViewEncapsulation.None, // para que los estilos que aplico al hijo desde el padre
})
export class CarouselComponent implements AfterViewInit {

  swiperDiv = viewChild.required<ElementRef>('swiperDiv');

  ngAfterViewInit(): void {
    const element = this.swiperDiv().nativeElement;
    if ( !element ) return;

    const swiper = new Swiper( element, {
      //
      // slidesPerView: 3.5,
      // spaceBetween: 10,
      centeredSlides: true, // Slide central activa

      // Optional parameters
      direction: 'horizontal',
      loop: true,

      modules: [
        // Pagination,
        Navigation
      ],


      // preventClicks: false,
      // preventClicksPropagation: false,

      // If we need pagination
      pagination: {
        el: '.swiper-pagination',
        type: 'fraction'
      },

      // Navigation arrows
      navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
      },

      // And if we need scrollbar
      scrollbar: {
        el: '.swiper-scrollbar',
      },

       // Responsive breakpoints
      breakpoints: {
        // Móviles pequeños
        320: {
          slidesPerView: 1.2,
          spaceBetween: 10,
        },
        // Móviles
        480: {
          slidesPerView: 1.5,
          spaceBetween: 15,
        },
        // Tablets
        768: {
          slidesPerView: 2.5,
          spaceBetween: 10,
        },
        // Desktop pequeño
        1024: {
          slidesPerView: 3.5,
          spaceBetween: 15,
        },
        // Desktop grande
        1200: {
          slidesPerView: 3.9, // lo que veo
          spaceBetween: 20,
        }
      }
    });
  }



  slides = signal<SlideData[]>([
    {
      title: "Desolate devouring art",
      description: "As an Immortal Emperor in his previous life, Liu Wuxie was one of the strongest existences in the world. But in a battle over the Sky Devouring Divine Cauldron with the other Immortal Emperors, Liu Wuxie was forced to self-detonate at the end.",
      image: "//jadescrolls.com/_next/image?url=https%3A%2F%2Fapi.jadescrolls.com%2Fupload%2Fimages%2Fbanner%2F2-2025%2Fd0c46094-d428-4a60-bb25-7521552c87ce.webp&w=1920&q=75",
      percentage: 32,
      status: "Ongoing",
      id: '123'
    },
    {
      title: "Dragon Slayer Chronicles",
      description: "A fearless warrior faces mythical creatures in an epic journey...",
      image: "https://imagizer.imageshack.com/img924/7345/EPVbrf.png",
      percentage: 92,
      status: "Completed",
      id: '123'
    },
    {
      title: "Dragon Slayer Chronicles",
      description: "A fearless warrior faces mythical creatures in an epic journey...",
      image: "https://api.panchotranslations.com/uploads/1746669768846-Portada_df.webp",
      percentage: 92,
      status: "Completed",
      id: '123'
    },
    {
      title: "Desolate devouring art",
      description: "As an Immortal Emperor in his previous life, Liu Wuxie was one of the strongest existences in the world. But in a battle over the Sky Devouring Divine Cauldron with the other Immortal Emperors, Liu Wuxie was forced to self-detonate at the end.",
      image: "https://api.panchotranslations.com/uploads/1745989162847-mensajero_sin_alma_df.webp",
      percentage: 32,
      status: "Ongoing",
      id: '123'
    },
    {
      title: "Dragon Slayer Chronicles",
      description: "A fearless warrior faces mythical creatures in an epic journey...",
      image: "https://api.panchotranslations.com/uploads/1745983441777-arwin_df.webp",
      percentage: 92,
      status: "Completed",
      id: '123'
    },
    {
      title: "Desolate devouring art",
      description: "As an Immortal Emperor in his previous life, Liu Wuxie was one of the strongest existences in the world. But in a battle over the Sky Devouring Divine Cauldron with the other Immortal Emperors, Liu Wuxie was forced to self-detonate at the end.",
      image: "https://api.panchotranslations.com/uploads/1745988099676-destinado_df.webp",
      percentage: 32,
      status: "Ongoing",
      id: '123'
    },


  ]);



}
