import { Component, computed, inject, signal } from '@angular/core';
import { BookDetailsCoverComponent } from "../../../features/books/components/book-details-cover/book-details-cover.component";
import { ChapterAccordionComponent } from "../../../features/books/components/book-details-content/book-details-content.component";
import { Book } from '../../../features/books/interfaces/book-response.interface';
import { rxResource } from '@angular/core/rxjs-interop';
import { BooksService } from '../../../features/books/services/books.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-book-page',
  imports: [BookDetailsCoverComponent, ChapterAccordionComponent],
  templateUrl: './book-page.component.html',
})
export class BookPageComponent {

  activatedRoute = inject(ActivatedRoute);

  private booksService = inject(BooksService);

  bookId: number = this.activatedRoute.snapshot.params['bookId'];

  bookResource = rxResource({
    request: () => ({id: this.bookId}),
    loader: ({request}) => {
        return this.booksService.getBook(request.id);
      },
    }
  );

  // Señal computada para los géneros
  // genres = computed(() => this.genresResource.value() ?? []);

}
