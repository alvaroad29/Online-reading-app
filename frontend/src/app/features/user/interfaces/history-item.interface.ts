export interface HistoryItem {
  idBook: string; // id de la novela
  idChapter: string; 
  nameChapter: string;
  title: string; //
  image?: string;
  readAt: Date;  // fecha de lectura
}