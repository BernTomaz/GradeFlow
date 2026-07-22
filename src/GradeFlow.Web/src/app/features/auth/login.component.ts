import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthApiService } from '../../core/api/auth-api.service';
import { UserRole } from '../../core/models/auth.models';
import { passwordStrength } from '../../shared/password-strength';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthApiService);
  private readonly router = inject(Router);
  protected readonly roles = UserRole;
  protected registering = false;
  protected error = '';
  protected success = '';
  protected form = this.fb.nonNullable.group({
    name: [''],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: [UserRole.Teacher]
  });

  protected submit() {
    this.error = '';
    this.success = '';
    const request = this.form.getRawValue();
    if (this.registering) {
      if (!request.name.trim()) {
        this.error = 'Informe seu nome.';
        return;
      }

      this.auth.register(request).subscribe({
        next: () => {
          this.registering = false;
          this.success = 'Acesso criado. Entre com seu email e senha.';
          this.form.patchValue({ name: '', password: '', role: UserRole.Teacher });
        },
        error: (error) => (this.error = error.error?.error ?? 'Nao foi possivel criar o acesso.')
      });
      return;
    }

    this.auth.login({ email: request.email, password: request.password }).subscribe({
      next: (response) => {
        this.auth.save(response);
        this.router.navigateByUrl('/dashboard');
      },
      error: (error) => (this.error = error.error?.error ?? 'Nao foi possivel entrar.')
    });
  }

  protected toggleRegistering() {
    this.registering = !this.registering;
    this.error = '';
    this.success = '';
  }

  protected passwordStrength() {
    return passwordStrength(this.form.controls.password.value);
  }
}
