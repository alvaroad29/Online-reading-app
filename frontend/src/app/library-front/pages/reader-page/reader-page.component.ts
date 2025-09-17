import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ReaderFooterComponent } from "../../../features/books/components/reader-footer/reader-footer.component";
import { ReadingSettings } from '../../../features/books/interfaces/reading-setting.interface';
import { rxResource } from '@angular/core/rxjs-interop';
import { BooksService } from '../../../features/books/services/books.service';
@Component({
  selector: 'app-reader-page',
  imports: [CommonModule, RouterModule, ReaderFooterComponent],
  templateUrl: './reader-page.component.html',
  styleUrls: ['./reader-page.component.css'],
})
export class ReaderPageComponent {
  activatedRoute = inject(ActivatedRoute);

  private booksService = inject(BooksService);

  bookId = signal<number>(0);
  chapterId = signal<number>(0);

  chapterResource = rxResource({
    request: () => ({id: this.chapterId()}),
    loader: ({request}) => {
        return this.booksService.getChapter(request.id);
      },
    // equal: (prev, curr) => prev.nextChapterId === curr.id,
    }
  );

  constructor() {
    // Suscribirse a los cambios de los parámetros
    this.activatedRoute.paramMap.subscribe(params => {
      const bookId = Number(params.get('bookId'));
      const chapterId = Number(params.get('chapterId'));

      this.bookId.set(bookId);
      this.chapterId.set(chapterId);
    });
  }


  ngAfterViewInit() {
    window.scrollTo(0, 0);
  }

  // Estado de las configuraciones
  font = 'Open Sans, sans-serif';
  textSize = 16;
  lineHeight = 24;
  highContrast = false;

  // Maneja los cambios de configuración
  onSettingsChanged(settings: ReadingSettings) {
    this.font = settings.font;
    this.textSize = settings.textSize;
    this.lineHeight = settings.lineHeight;
    this.highContrast = settings.highContrast;
  }
}
