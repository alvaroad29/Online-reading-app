// interfaces/book.interface.ts

export interface Chapter {
  id: string;
  number: number;
  title?: string;
  updatedAt: Date | string;
  // isNew?: boolean; // Para mostrar badge "NEW"
}

export interface cardUpdate {
  id: string;
  title: string;
  image: string;

  // Metadatos básicos
  // status: 'ongoing' | 'completed' | 'hiatus';
  // type: 'manhwa' | 'manga' | 'manhua';
  // language: 'english' | 'spanish' | 'korean' | 'japanese';

  // Para carousel
  // description?: string;

  // Para update-card (últimos 4 capítulos)
  recentChapters: Chapter[];

  // Badges/etiquetas
  // badges?: string[]; // ['New', 'Pinned', 'Hot', etc.]

  // Fechas
  // createdAt: Date | string;
  // updatedAt: Date | string;
}

// Ejemplo de uso:
export const exampleBook: cardUpdate = {
  id: '1',
  title: 'I Will Buy Divine Power With Money!',
  image: '/assets/books/divine-power.jpg',
  // status: 'ongoing',
  // type: 'manhwa',
  // language: 'english',
  // description: 'A story about buying divine power with money...',
  // badges: ['New', 'Pinned'],
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
  // createdAt: '2024-01-01',
  // updatedAt: '2024-06-01'
};


