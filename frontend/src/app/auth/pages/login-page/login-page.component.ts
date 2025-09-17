import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormUtils } from '../../utils/form-utils';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'login-page',
  imports: [ RouterLink, ReactiveFormsModule],
  templateUrl: './login-page.component.html',
})
export class LoginPageComponent {

  authService = inject(AuthService);
  router = inject(Router);

  fb = inject(FormBuilder);

  formUtils = FormUtils;

  loginForm: FormGroup = this.fb.group({ // validaciones los mas parecidas a las del backend
    email: ['', [Validators.required, Validators.pattern(FormUtils.emailPattern)]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  })

  hasError = signal(false);
  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const { email, password } = this.loginForm.value;

    this.authService.login(email, password)
      .subscribe( isAunthenticated => {
        if (isAunthenticated) {
          this.router.navigateByUrl('/'); // cambiar dsp
          return;
        }
        this.hasError.set(true);
      });

    // this.loginForm.reset();
  }



}
