export interface List {
  id: string;
  title: string;
  image?: string;
  creationDate: string;  // fecha de creación
  genres?: string[];     // array de géneros (strings)
  cant?: number;         // cantidad de series
  views?: number;        // cantidad de vistas
  follows?: number;      // cantidad de seguidores
  comments?: number;     // cantidad de comentarios
  author?: string;       // creador de la lista
  description?: string;  // descripción corta
}
