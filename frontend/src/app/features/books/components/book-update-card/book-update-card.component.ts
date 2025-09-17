import { Component, input } from '@angular/core';
import { cardUpdate } from '../../interfaces/card-update.interface';
import { SlicePipe, TitleCasePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RelativeTimePipe } from '../../../../shared/pipes/relative-time.pipe';

@Component({
  selector: 'book-update-card',
  imports: [RouterLink, RelativeTimePipe, TitleCasePipe],
  templateUrl: './book-update-card.component.html',
})
export class BookUpdateCardComponent {
  book = input<cardUpdate>();
}
