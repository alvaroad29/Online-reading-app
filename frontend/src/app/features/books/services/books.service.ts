import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BooksResponse, Genre, State } from '../interfaces/book-response.interface';
import { Observable } from 'rxjs';
import { BookPopularResponse } from '../interfaces/book-popular-response.interface';
import { environment } from '../../../../environments/environment';
import { BookDetails } from '../interfaces/book-details-interface';
import { ReadChapter } from '../interfaces/chapter-interface';

const baseUrl = environment.baseUrl;

export interface BookRatingResponseDto {
  id: number;
  bookId: number;
  userId: string;
  ratingValue: number;
  ratingDate: string;
  username: string;
}

export interface UserRatingDto {
  rating: number | null;
}

export interface RatingRequestDto {
  rating: number;
}

interface BookQueryParameters
{
    // Paginación
    PageNumber?: number;
    PageSize?: number;

    // Filtros
    SearchText?: string;
    SearchIn?: SearchInField; // Campo para buscar
    State?: State;
    GenreIds?: number[]; // IDs de géneros a filtrar

    // Ordenamiento
    SortBy?: SortBy; // "title", "score", "description"
    SortDescending?: boolean; // "asc", "desc"
}

export enum SortBy {
  Title = 'title',
  Score = 'score',
  Views = 'views'
}

export type SortByOption = {
  value: SortBy;
  label: string;
};

export enum SearchInField {
  All = 'all',
  Title = 'title',
  Author = 'author',
  Description = 'description',
}

export type SearchFieldOption = {
  value: SearchInField;
  label: string;
};

@Injectable({
  providedIn: 'root'
})
export class BooksService {

  private http = inject(HttpClient);

  getPopularBooks(): Observable<BookPopularResponse> {
    return this.http.get<BookPopularResponse>(`${baseUrl}/Book/popular/all`)
    // .pipe( tap (resp => console.log(resp)));
  }

  getBooks(options: BookQueryParameters): Observable<BooksResponse> {


    const defaultedOptions = {
      PageNumber: 1,
      PageSize: 8,
      SortDescending: false,
      SearchIn: SearchInField.Title,
      SortBy: SortBy.Title,
      ...options
    };

    // Si State es All, lo eliminamos (no queremos enviarlo)
    if (defaultedOptions.State === State.All) {
      delete defaultedOptions.State;
    }

    // Filtrar undefined antes de enviar
    const params = Object.fromEntries(
      Object.entries(defaultedOptions)
        .filter(([_, value]) => value !== undefined)
    );

    // console.log({params});
    return this.http.get<BooksResponse>(`${baseUrl}/Book`, { params });
      // .pipe( tap (resp => console.log(resp)));
  }

  getBook(id: number): Observable<BookDetails> {
    return this.http.get<BookDetails>(`${baseUrl}/Book/${id}/detailed`)
    // .pipe(
    //   tap( resp=> console.log(resp)),
    // )

  }

  getChapter(id: number): Observable<ReadChapter> {
    return this.http.get<ReadChapter>(`${baseUrl}/chapter/${id}`)
    // .pipe(
    //   tap( resp=> console.log(resp)),
    // )

  }

  getGenres(): Observable<Genre[]> {
    return this.http.get<Genre[]>(`${baseUrl}/Genre`)
    // .pipe(
    //   tap(r => {
    //   console.log(r);
    //   })
    // );
  }

  // uploadImage( imageFile: File): Observable<string> {
  //   if (!imageFile) {
  //     return of();
  //   }

  //   const formData = new FormData();
  //   formData.append('image', imageFile)

  //   return

  // }

  // Calificar un libro
  rateBook(bookId: number, rating: number): Observable<BookRatingResponseDto> {
    return this.http.post<BookRatingResponseDto>(`${baseUrl}/Book/${bookId}/rate`, {
      rating: rating
    });
  }

  // Obtener la calificación del usuario actual
  getUserRating(bookId: number): Observable<UserRatingDto> {
    return this.http.get<UserRatingDto>(`${baseUrl}/Book/${bookId}/user-rating`);
  }

  // Eliminar calificación
   deleteRating(bookId: number): Observable<any> {
    return this.http.delete(`${baseUrl}/Book/${bookId}/rating`);
  }

}
