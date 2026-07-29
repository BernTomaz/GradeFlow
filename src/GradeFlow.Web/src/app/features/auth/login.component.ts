import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApiService } from '../../core/api/auth-api.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthApiService);
  private readonly router = inject(Router);
  protected loading = false;
  protected error = '';
  private loginFailures = 0;
  private readonly maxLoginAttempts = 5;
  protected form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  ngOnInit() {
    this.auth.setupStatus().subscribe({
      next: (status) => {
        if (status.available) this.router.navigateByUrl('/setup');
      }
    });
  }

  protected submit() {
    if (this.loading) return;

    this.error = '';
    const request = this.form.getRawValue();

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
}
