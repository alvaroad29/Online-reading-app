import { Component, computed, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FormUtils } from '../../../auth/utils/form-utils';
import { BooksService } from '../../../features/books/services/books.service';
import { rxResource } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-create-book-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-book-page.component.html',
})
export class CreateBookPageComponent {

  private fb = inject(FormBuilder);
  private booksService = inject(BooksService);

  // private genreService = inject(GenreService);
  // private writerService = inject(WriterService);
  private router = inject(Router);
  formUtils = FormUtils;

   // Resource para cargar géneros
  genresResource = rxResource({
      loader: () => {
          return this.booksService.getGenres();
        },
      }
    );

  // Señal computada para los géneros
  genres = computed(() => this.genresResource.value() ?? []);

  file: File | null = null;

  bookForm: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    coverUrl: [''],
    synopsis: ['', [Validators.required, Validators.maxLength(4000)]],
    genres: this.fb.array<number>([], [Validators.required]),

    volumeTitle: ['', [Validators.required]],
    volumeOrder: [1, [Validators.required, Validators.min(1)]],

    chapterTitle: ['', [Validators.required]],
    chapterOrder: [1, [Validators.required, Validators.min(1)]],
    chapterContent: ['', [Validators.required]]
  });

  hasError = () => false; // Implementa si usas manejo de errores global
  errorMessages = () => [];

  isGenreSelected(genreId: number) {
    const selected = this.bookForm.get('genres') as FormArray;
    return selected.value.includes(genreId);
  }

  onGenreChange(event: Event) {
    const checkbox = event.target as HTMLInputElement;
    const genreId = checkbox.value;
    const selected = this.bookForm.get('genres') as FormArray;

    if (checkbox.checked) {
      selected.push(new FormControl(genreId));
    } else {
      const index = selected.value.indexOf(genreId);
      if (index >= 0) {
        selected.removeAt(index);
      }
    }
  }

  // Variables de estado
cover: File | null = null;
chapterFile: File | null = null;
coverPreview = signal<string | null>(null);

// Manejar selección de portada
onCoverSelected(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files[0]) {
    this.cover = input.files[0];

    // Vista previa
    const reader = new FileReader();
    reader.onload = () => this.coverPreview.set(reader.result as string);
    reader.readAsDataURL(this.cover);

    this.bookForm.get('coverImage')?.setValue(this.cover.name);
  }
}

// Manejar selección de contenido del capítulo
onFileSelected(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files[0]) {
    this.chapterFile = input.files[0];
    this.bookForm.get('chapterContent')?.setValue(this.chapterFile.name);
  }
}

  onSubmit() {
    console.log(this.bookForm.value);

    // if (this.bookForm.invalid) return;

    // const formData = new FormData();
    // const formValue = this.bookForm.value;

    // // Datos del libro
    // formData.append('title', formValue.title);
    // if (formValue.coverUrl) formData.append('coverUrl', formValue.coverUrl);
    // if (formValue.synopsis) formData.append('synopsis', formValue.synopsis);
    // formValue.genres.forEach((genreId: string) => formData.append('genres[]', genreId));

    // // Volumen
    // formData.append('volumeTitle', formValue.volumeTitle);
    // formData.append('volumeOrder', formValue.volumeOrder);

    // // Capítulo
    // formData.append('chapterTitle', formValue.chapterTitle);
    // if (this.file) formData.append('chapterFile', this.file);

    // this.writerService.createBookWithVolumeAndChapter(formData).subscribe({
    //   next: () => this.router.navigate(['/writer/books']),
    //   error: (err) => {
    //     // Maneja error
    //     console.error(err);
    //   }
    // });
  }
}
