import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

import { AuthService, LoginService } from '@core/authentication';

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  styleUrl: './register.scss',
  imports: [
    RouterLink,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatInputModule,
    TranslateModule,
  ],
})
export class Register {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly loginService = inject(LoginService);
  private readonly auth = inject(AuthService);

  isSubmitting = false;

  registerForm = this.fb.nonNullable.group(
    {
      email: ['', [Validators.required, Validators.email]],
      fullName: [''],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    },
    {
      validators: [this.matchValidator('password', 'confirmPassword')],
    }
  );

  get email() {
    return this.registerForm.get('email')!;
  }

  get fullName() {
    return this.registerForm.get('fullName')!;
  }

  get password() {
    return this.registerForm.get('password')!;
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword')!;
  }

  matchValidator(source: string, target: string) {
    return (control: AbstractControl) => {
      const sourceControl = control.get(source)!;
      const targetControl = control.get(target)!;
      if (targetControl.errors && !targetControl.errors.mismatch) {
        return null;
      }
      if (sourceControl.value !== targetControl.value) {
        targetControl.setErrors({ mismatch: true });
        return { mismatch: true };
      } else {
        targetControl.setErrors(null);
        return null;
      }
    };
  }

  register() {
    if (this.registerForm.invalid) {
      return;
    }

    this.isSubmitting = true;

    const { email, password, confirmPassword, fullName } = this.registerForm.value;

    this.loginService
      .register(email!, password!, confirmPassword!, fullName || undefined)
      .subscribe({
        next: (response) => {
          // Automatically log in the user after registration
          this.auth.login(email!, password!).subscribe({
            next: () => {
              this.router.navigateByUrl('/');
            },
            error: () => {
              // If auto-login fails, redirect to login page
              this.router.navigateByUrl('/auth/login');
            },
          });
        },
        error: (errorRes: HttpErrorResponse) => {
          if (errorRes.status === 400) {
            const form = this.registerForm;
            const errors = errorRes.error?.errors || errorRes.error;
            
            if (typeof errors === 'object') {
              Object.keys(errors).forEach(key => {
                const formKey = key.charAt(0).toLowerCase() + key.slice(1);
                const errorMessage = Array.isArray(errors[key]) ? errors[key][0] : errors[key];
                form.get(formKey)?.setErrors({
                  remote: errorMessage,
                });
              });
            }
          }
          this.isSubmitting = false;
        },
      });
  }
}
