import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthApiService } from '../../core/api/auth-api.service';
import { apiErrorMessage } from '../../shared/api-error';

@Component({
  selector: 'app-change-name',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './change-name.component.html'
})
export class ChangeNameComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthApiService);
  protected error = '';
  protected success = '';
  protected form = this.fb.nonNullable.group({
    name: [this.auth.current()?.user.name ?? '', Validators.required]
  });

  protected submit() {
    this.error = '';
    this.success = '';
    this.auth.changeName(this.form.getRawValue()).subscribe({
      next: (response) => {
        this.auth.save(response);
        this.success = 'Nome alterado com sucesso.';
      },
      error: (error) => (this.error = apiErrorMessage(error, 'Nao foi possivel alterar o nome.'))
    });
  }
}
