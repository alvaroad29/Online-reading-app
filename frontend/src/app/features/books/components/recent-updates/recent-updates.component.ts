import { cardUpdate } from '../../interfaces/card-update.interface';
import { Component, signal } from '@angular/core';
import { BookUpdateCardComponent } from "../book-update-card/book-update-card.component";

@Component({
  selector: 'recent-updates',
  imports: [BookUpdateCardComponent],
  templateUrl: './ recent-updates.component.html',
})
export class RecentUpdatesComponent {

  recentBooks =  signal<cardUpdate[]>([
    {
      title: "Desolate devouring art",
      image: "//jadescrolls.com/_next/image?url=https%3A%2F%2Fapi.jadescrolls.com%2Fupload%2Fimages%2Fbanner%2F2-2025%2Fd0c46094-d428-4a60-bb25-7521552c87ce.webp&w=1920&q=75",
      id: '123',
      recentChapters: [
        {
          id: '20',
          number: 20,
          updatedAt: '2025-05-01',
          // isNew: true
        },
        {
          id: '19',
          number: 19,
          updatedAt: '2024-05-30'
        },
        {
          id: '12',
          number: 12,
          updatedAt: '2025-06-07T11:20:02'
        },
        {
          id: '11',
          number: 11,
          updatedAt: '2025-06-07T09:10:00'
        }
      ],
    },
    {
      title: "Dragon Slayer Chronicles",
      image: "https://imagizer.imageshack.com/img924/7345/EPVbrf.png",
      id: '123',
      recentChapters: [
        {
          id: '20',
          number: 20,
          updatedAt: '2024-06-01',
          // isNew: true
        },
        {
          id: '19',
          number: 19,
          updatedAt: '2024-05-30'
        },
        {
          id: '12',
          number: 12,
          updatedAt: '2024-05-28'
        },
        {
          id: '11',
          number: 11,
          updatedAt: '2024-05-26'
        }
      ],
    },
    {
      title: "Dragon Slayer Chronicles",
      image: "https://api.panchotranslations.com/uploads/1746669768846-Portada_df.webp",
      id: '123',
      recentChapters: [
        {
          id: '20',
          number: 20,
          updatedAt: '2024-06-01',
          // isNew: true
        },
        {
          id: '19',
          number: 19,
          updatedAt: '2024-05-30'
        },
        {
          id: '12',
          number: 12,
          updatedAt: '2024-05-28'
        },
        {
          id: '11',
          number: 11,
          updatedAt: '2024-05-26'
        }
      ],
    },
    {
      title: "Desolate devouring art",
      image: "https://api.panchotranslations.com/uploads/1745989162847-mensajero_sin_alma_df.webp",
      id: '123',
      recentChapters: [
        {
          id: '20',
          number: 20,
          updatedAt: '2024-06-01',
          // isNew: true
        },
        {
          id: '19',
          number: 19,
          updatedAt: '2024-05-30'
        },
        {
          id: '12',
          number: 12,
          updatedAt: '2024-05-28'
        },
        {
          id: '11',
          number: 11,
          updatedAt: '2024-05-26'
        }
      ],
    },
    {
      title: "Dragon Slayer Chronicles",
      image: "https://api.panchotranslations.com/uploads/1745983441777-arwin_df.webp",
      id: '123',
      recentChapters: [
        {
          id: '20',
          number: 20,
          updatedAt: '2024-06-01',
          // isNew: true
        },
        {
          id: '19',
          number: 19,
          updatedAt: '2024-05-30'
        },
        {
          id: '12',
          number: 12,
          updatedAt: '2024-05-28'
        },
        {
          id: '11',
          number: 11,
          updatedAt: '2024-05-26'
        }
      ],
    },
    {
      title: "Desolate devouring art",
      image: "https://api.panchotranslations.com/uploads/1745988099676-destinado_df.webp",
      id: '123',
      recentChapters: [
        {
          id: '20',
          number: 20,
          updatedAt: '2024-06-01',
          // isNew: true
        },
        {
          id: '19',
          number: 19,
          updatedAt: '2024-05-30'
        },
        {
          id: '12',
          number: 12,
          updatedAt: '2024-05-28'
        },
        {
          id: '11',
          number: 11,
          updatedAt: '2024-05-26'
        }
      ],
    },


  ]);
}
