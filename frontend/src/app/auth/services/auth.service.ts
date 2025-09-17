import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { catchError, map, Observable, of, tap } from 'rxjs';

import { User } from '../interfaces/user.interface';
import { AuthResponse } from '../interfaces/auth-response.interface';

const baseUrl = environment.baseUrl;

export interface RegisterResponse {
  success: boolean;
  data?: any;
  errors?: string[];
}

type AuthStatus = 'checking' | 'authenticated' | 'not-authenticated';

@Injectable({providedIn: 'root'})
export class AuthService {

  private _authStatus = signal<AuthStatus>('checking');
  private _user = signal<User|null>(null);
  private _token = signal<string|null>(localStorage.getItem('token'));

  private http = inject(HttpClient);

  // getter para acceder a la propiedad privada
  authStatus = computed<AuthStatus>(() => { // computed -> solo lectura
    if (this._authStatus() === 'checking' ) {
      return 'checking';
    }

    if (this._user()) {
      return 'authenticated'
    }

    return 'not-authenticated';
  });

  user = computed<User|null>(() => this._user());
  token = computed(() => this._token());
  isAdmin = computed(() => this._user()?.roles.includes('Admin') ?? false);
  isAuthenticated = computed(() => this.authStatus() == 'authenticated' ? true : false );

  login(email: string, password:string):Observable<boolean> {
    return this.http.post<AuthResponse>(`${baseUrl}/Users/Login`, {
      email: email,
      password: password
    }).pipe(
      tap( resp => { //salio bien
        this.handleAuthSuccess(resp);
      }),
      map(() => true),
      catchError((error : any) =>
        this.handleAuthError(error)
      )
    )
  }

  register(email: string, password: string, username: string):Observable<RegisterResponse> {
    return this.http.post<User>(`${baseUrl}/Users`, {
      username: username,
      email: email,
      password: password
    }).pipe(
      map((resp: any) => {
        // Registro exitoso (201)
        // console.log(resp);

        return {
          success: true,
          data: resp
        } as RegisterResponse;
      }),
      catchError((error: HttpErrorResponse) => {
        return of(this.handleRegisterError(error));
      })
    );
  }


  editUser(id: string, username?: string, newPassword?: string, confirmPassword?: string, currentPassword?: string):Observable<RegisterResponse> {
    return this.http.put<AuthResponse>(`${baseUrl}/Users/${id}`, {
      username,
      newPassword,
      confirmPassword,
      currentPassword
    }).pipe(
      tap( resp => {
        this.handleAuthSuccess(resp);
      }),
      map((resp) => {
        return {
          success: true,
          data: resp.user
        } as RegisterResponse;
      }),
      catchError((error: HttpErrorResponse) => {
        return of(this.handleRegisterError(error));
      })
    );
  }

  checkStatus(): Observable<boolean> {
    const token = localStorage.getItem('token');
    if (!token) {
      this.logout();
      return of(false);
    }
    return this.http.get<AuthResponse>(`${baseUrl}/Users/check-status`, {
      // headers: {
      //   Authorization: `Bearer ${token}`
      // }
    }).pipe(
        tap( resp => { //salio bien
          this.handleAuthSuccess(resp);
        }),
        map(() => true),
        catchError((error : any) =>
          this.handleAuthError(error)
        )
    )

  }

  logout() {
    this._user.set(null);
    this._authStatus.set('not-authenticated');
    this._token.set(null);

    localStorage.removeItem('token');
  }

  private handleAuthSuccess( {token, user}: AuthResponse ){
      this._user.set(user);
      this._authStatus.set('authenticated');
      this._token.set(token);

      localStorage.setItem('token', token);
  }

  private handleAuthError( error : any ) {
    this.logout();
    return of(false);
  }


  private handleRegisterError(error: HttpErrorResponse): RegisterResponse {
  const errorMessages: string[] = [];

  switch (error.status) {
    case 400:
      // Caso 1: Mensaje en propiedad 'message'
      if (error.error?.message) {
        errorMessages.push(error.error.message);
      }
      // Caso 2: Objeto con propiedad 'errors' (errores de validación del DTO)
      else if (error.error?.errors) {
        Object.keys(error.error.errors).forEach(field => {
          const fieldErrors = error.error.errors[field];
          errorMessages.push(...fieldErrors);
        });
      }
      // Caso 3: Otros objetos
      else if (typeof error.error === 'object' && error.error !== null) {
        Object.keys(error.error).forEach(key => {
          if (Array.isArray(error.error[key])) {
            errorMessages.push(...error.error[key]);
          } else if (typeof error.error[key] === 'string') {
            errorMessages.push(error.error[key]);
          }
        });
      }
      // Caso 4: Error genérico
      else {
        errorMessages.push('Error de validación en los datos enviados');
      }
      break;

    case 500:
      errorMessages.push('Error interno del servidor. Por favor, intenta más tarde.');
      break;

    default:
      errorMessages.push('Ha ocurrido un error inesperado. Por favor, intenta nuevamente.');
      break;
  }

  return {
    success: false,
    errors: errorMessages
  };
}
}
