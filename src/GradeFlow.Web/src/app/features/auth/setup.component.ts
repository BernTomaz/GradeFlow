import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApiService } from '../../core/api/auth-api.service';
import { apiErrorMessage } from '../../shared/api-error';
import { passwordRules, passwordStrength } from '../../shared/password-strength';

@Component({
  selector: 'app-setup',
  imports: [ReactiveFormsModule],
  templateUrl: './setup.component.html'
})
export class SetupComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthApiService);
  private readonly router = inject(Router);
  protected loading = false;
  protected error = '';
  protected form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/)
    ]],
    confirmPassword: ['', Validators.required]
  });

  ngOnInit() {
    this.auth.setupStatus().subscribe({
      next: (status) => {
        if (!status.available) this.router.navigateByUrl('/login');
      },
      error: (error) => (this.error = apiErrorMessage(error, 'Nao foi possivel verificar a configuracao inicial.'))
    });
  }

  protected submit() {
    if (this.loading) return;

    this.error = '';
    const request = this.form.getRawValue();
    if (request.password !== request.confirmPassword) {
      this.error = 'A confirmacao da senha nao confere.';
      return;
    }

    this.loading = true;
    this.auth.createSetupAdmin({
      name: request.name,
      email: request.email,
      password: request.password
    }).pipe(finalize(() => (this.loading = false))).subscribe({
      next: (response) => {
        this.auth.save(response);
        this.router.navigateByUrl('/dashboard');
      },
      error: (error) => (this.error = apiErrorMessage(error, 'Nao foi possivel criar o administrador.'))
    });
  }

  protected passwordStrength() {
    return passwordStrength(this.form.controls.password.value);
  }

  protected passwordRules() {
    return passwordRules(this.form.controls.password.value);
  }
}
