import { Component, computed, inject, signal } from '@angular/core';
import { CarouselComponent } from "../../components/carousel/carousel.component";
import { BookCarouselComponent } from "../../../features/books/components/book-carousel/book-carousel.component";
import { CardData } from '../../../features/books/interfaces/card-data.interface';
import { RecentUpdatesComponent } from '../../../features/books/components/recent-updates/ recent-updates.component';
import { BooksService } from '../../../features/books/services/books.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { Book, BooksResponse, State } from '../../../features/books/interfaces/book-response.interface';
import { BookPopularResponse } from '../../../features/books/interfaces/book-popular-response.interface';
import { AnnouncementComponent } from '../../components/announcement/announcement.component';

type Period = 'daily' | 'weekly' | 'monthly';
@Component({
  selector: 'app-home-page',
  imports: [CarouselComponent, BookCarouselComponent, RecentUpdatesComponent, AnnouncementComponent],
  templateUrl: './home-page.component.html',
})
export class HomePageComponent {

  private booksService = inject(BooksService);

  booksResource = rxResource({
    request: () => ({}),
    loader: ({request}) => {
      return this.booksService.getPopularBooks();
    }
  }
  );

  selectedPeriod = signal<Period>('daily');

  periods = [
    { label: 'Diario', value: 'daily' },
    { label: 'Semanal', value: 'weekly' },
    { label: 'Mensual', value: 'monthly' }
  ];


   cardsByPeriod = computed<Record<Period, CardData[]>>(() => {
    const data = this.booksResource.value();

    if (!data) return { daily: [], weekly: [], monthly: [] };

    const mapBooks = (books: Book[]): CardData[] =>
      books.map(book => ({
        title: book.title,
        image: book.imageUrl,
        status: State[book.state],
        score: book.score,
        id: book.id.toString(),
      }));

    return {
      daily: mapBooks(data.daily),
      weekly: mapBooks(data.weekly),
      monthly: mapBooks(data.monthly)
    };
  });

  changePeriod(period: Period) {
    this.selectedPeriod.set(period);
  }

  getPopularBooks = computed(() => this.cardsByPeriod()[this.selectedPeriod()]);

}
