import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthApiService } from '../../core/api/auth-api.service';
import { UserRole } from '../../core/models/auth.models';
import { apiErrorMessage } from '../../shared/api-error';
import { passwordRules, passwordStrength } from '../../shared/password-strength';

@Component({
  selector: 'app-user-create',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './user-create.component.html'
})
export class UserCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthApiService);
  protected readonly roles = UserRole;
  protected readonly roleOptions = [
    { value: UserRole.Teacher, label: 'Professor' },
    { value: UserRole.Student, label: 'Aluno' }
  ];
  protected error = '';
  protected success = '';
  protected form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/)
    ]],
    role: [UserRole.Teacher]
  });

  protected submit() {
    this.error = '';
    this.success = '';
    this.auth.register(this.form.getRawValue()).subscribe({
      next: () => {
        this.success = 'Usuário criado com sucesso.';
        this.form.reset({ name: '', email: '', password: '', role: UserRole.Teacher });
      },
      error: (error) => (this.error = apiErrorMessage(error, 'Nao foi possivel criar o usuario.'))
    });
  }

  protected passwordStrength() {
    return passwordStrength(this.form.controls.password.value);
  }

  protected passwordRules() {
    return passwordRules(this.form.controls.password.value);
  }

  protected roleLabel() {
    return this.roleOptions.find((role) => role.value === this.form.controls.role.value)?.label ?? 'Professor';
  }

  protected setRole(role: UserRole, menu: HTMLDetailsElement) {
    this.form.controls.role.setValue(role);
    menu.removeAttribute('open');
  }
}
