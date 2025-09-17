import { Component, input } from '@angular/core';
import { HistoryItem } from '../../interfaces/history-item.interface';
import { RouterLink } from '@angular/router';
import { RelativeTimePipe } from '../../../../shared/pipes/relative-time.pipe';
import { TitleCasePipe } from '@angular/common';
import { History, LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'list-history',
  imports: [RouterLink,  RelativeTimePipe, TitleCasePipe, LucideAngularModule],
  templateUrl: './list-history.component.html',
})
export class ListHistoryComponent {
  userHistory = input<HistoryItem[]>([]);

   readonly history = History;
}
