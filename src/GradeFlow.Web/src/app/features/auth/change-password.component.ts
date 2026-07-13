import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthApiService } from '../../core/api/auth-api.service';
import { apiErrorMessage } from '../../shared/api-error';
import { passwordStrength } from '../../shared/password-strength';

@Component({
  selector: 'app-change-password',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './change-password.component.html'
})
export class ChangePasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthApiService);
  protected error = '';
  protected success = '';
  protected form = this.fb.nonNullable.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  });

  protected submit() {
    this.error = '';
    this.success = '';
    const request = this.form.getRawValue();

    if (request.newPassword !== request.confirmPassword) {
      this.error = 'A confirmação da senha não confere.';
      return;
    }

    this.auth.changePassword({
      currentPassword: request.currentPassword,
      newPassword: request.newPassword
    }).subscribe({
      next: () => {
        this.success = 'Senha alterada com sucesso.';
        this.form.reset();
      },
      error: (error) => (this.error = apiErrorMessage(error, 'Nao foi possivel alterar a senha.'))
    });
  }

  protected passwordStrength() {
    return passwordStrength(this.form.controls.newPassword.value);
  }
}
