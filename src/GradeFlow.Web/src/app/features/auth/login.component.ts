import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApiService } from '../../core/api/auth-api.service';
import { UserRole } from '../../core/models/auth.models';
import { passwordRules, passwordStrength } from '../../shared/password-strength';

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
  protected loading = false;
  protected error = '';
  protected success = '';
  private loginFailures = 0;
  private readonly maxLoginAttempts = 5;
  protected form = this.fb.nonNullable.group({
    name: [''],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/)
    ]],
    role: [UserRole.Teacher]
  });

  protected submit() {
    if (this.loading) return;

    this.error = '';
    this.success = '';
    const request = this.form.getRawValue();
    if (this.registering) {
      if (!request.name.trim()) {
        this.error = 'Informe seu nome.';
        return;
      }

      this.loading = true;
      this.auth.register(request).pipe(finalize(() => (this.loading = false))).subscribe({
        next: () => {
          this.registering = false;
          this.success = 'Acesso criado. Entre com seu email e senha.';
          this.form.patchValue({ name: '', password: '', role: UserRole.Teacher });
        },
        error: (error) => (this.error = error.error?.error ?? 'Nao foi possivel criar o acesso.')
      });
      return;
    }

    this.loading = true;
    this.auth.login({ email: request.email, password: request.password }).pipe(finalize(() => (this.loading = false))).subscribe({
      next: (response) => {
        this.loginFailures = 0;
        this.auth.save(response);
        this.router.navigateByUrl('/dashboard');
      },
      error: (error) => {
        const message = error.error?.error ?? 'Nao foi possivel entrar.';
        if (error.status !== 401 || message.includes('Restam')) {
          this.error = message;
          return;
        }

        this.loginFailures++;
        const remaining = Math.max(0, this.maxLoginAttempts - this.loginFailures);
        this.error = `${message} Restam ${remaining} tentativa(s).`;
      }
    });
  }

  protected toggleRegistering() {
    if (this.loading) return;

    this.registering = !this.registering;
    this.error = '';
    this.success = '';
  }

  protected passwordStrength() {
    return passwordStrength(this.form.controls.password.value);
  }

  protected passwordRules() {
    return passwordRules(this.form.controls.password.value);
  }
}
