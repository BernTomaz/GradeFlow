import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthApiService } from '../../core/api/auth-api.service';
import { UserResponse, UserRole } from '../../core/models/auth.models';
import { apiErrorMessage } from '../../shared/api-error';

@Component({
  selector: 'app-user-reset-password',
  imports: [FormsModule, RouterLink],
  templateUrl: './user-reset-password.component.html'
})
export class UserResetPasswordComponent implements OnInit {
  private readonly auth = inject(AuthApiService);
  protected readonly users = signal<UserResponse[]>([]);
  protected readonly selected = signal<UserResponse | null>(null);
  protected search = '';
  protected temporaryPassword = '';
  protected dialogOpen = false;
  protected error = '';
  protected success = '';

  ngOnInit() {
    this.loadUsers();
  }

  protected roleName(role: UserRole) {
    if (role === UserRole.Admin) {
      return 'Admin';
    }

    return role === UserRole.Teacher ? 'Professor' : 'Aluno';
  }

  protected filteredUsers() {
    const term = this.search.trim().toLowerCase();
    if (term.length < 3) {
      return this.users();
    }

    return this.users().filter((user) =>
      `${user.name} ${user.email} ${this.roleName(user.role)}`.toLowerCase().includes(term)
    );
  }

  protected updateSearch(value: string) {
    this.search = value;
    this.dialogOpen = value.trim().length >= 3;
  }

  protected selectUser(user: UserResponse) {
    this.selected.set(user);
    this.dialogOpen = false;
    this.search = user.name;
    this.error = '';
    this.success = '';
  }

  protected clearSelection() {
    this.selected.set(null);
    this.temporaryPassword = '';
    this.search = '';
    this.dialogOpen = false;
  }

  protected resetPassword() {
    const user = this.selected();
    if (!user) {
      return;
    }

    this.error = '';
    this.success = '';
    this.auth.resetPassword(user.id, { temporaryPassword: this.temporaryPassword }).subscribe({
      next: () => {
        this.success = `Senha temporária definida para ${user.name}.`;
        this.temporaryPassword = '';
        this.loadUsers();
      },
      error: (error) => (this.error = apiErrorMessage(error, 'Nao foi possivel redefinir a senha.'))
    });
  }

  private loadUsers() {
    this.auth.listUsers().subscribe({
      next: (users) => this.users.set(users),
      error: () => this.users.set([])
    });
  }
}
