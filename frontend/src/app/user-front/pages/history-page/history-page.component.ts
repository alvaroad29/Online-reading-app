import { Component } from '@angular/core';
import { HistoryItem } from '../../../features/user/interfaces/history-item.interface';
import { ListHistoryComponent } from '../../../features/user/components/list-history/list-history.component';

@Component({
  selector: 'app-history-page',
  imports: [ListHistoryComponent],
  templateUrl: './history-page.component.html',
})
export class HistoryPageComponent {

  historyData: HistoryItem[] = [
  {
    idBook: 'novel-001',
    idChapter: 'ch-10',
    nameChapter: 'capítulo 10 - la batalla final',
    title: 'El ascenso del héroe',
    image: 'https://placehold.co/80x80?text=Hero',
    readAt: new Date('2025-07-20T14:30:00')
  },
  {
    idBook: 'novel-002',
    idChapter: 'ch-45',
    nameChapter: 'capítulo 45 - el despertar del dragón',
    title: 'Mundo de Dragones',
    image: 'https://placehold.co/80x80?text=Dragon',
    readAt: new Date('2025-07-18T10:00:00')
  },
  {
    idBook: 'novel-003',
    idChapter: 'ch-5',
    nameChapter: 'capítulo 5 - sombras en la noche',
    title: 'Cazadores Nocturnos',
    image: 'https://placehold.co/80x80?text=Night',
    readAt: new Date('2025-07-17T22:45:00')
  },
  {
    idBook: 'novel-004',
    idChapter: 'ch-21',
    nameChapter: 'capítulo 21 - las ruinas antiguas',
    title: 'Reinos Olvidados',
    image: 'https://placehold.co/80x80?text=Ruins',
    readAt: new Date('2025-07-15T18:00:00')
  },
  {
    idBook: 'novel-005',
    idChapter: 'ch-30',
    nameChapter: 'capítulo 30 - el pacto oscuro',
    title: 'Crónicas del Mago Oscuro',
    image: 'https://placehold.co/80x80?text=Dark+Mage',
    readAt: new Date('2025-07-14T09:15:00')
  }
];
}
