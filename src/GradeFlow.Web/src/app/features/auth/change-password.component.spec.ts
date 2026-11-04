import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AuthApiService } from '../../core/api/auth-api.service';
import { UserRole } from '../../core/models/auth.models';
import { ChangePasswordComponent } from './change-password.component';

describe('ChangePasswordComponent', () => {
  afterEach(() => sessionStorage.removeItem('gradeflow.auth'));

  it('replaces the revoked session with the token returned after changing the password', async () => {
    await TestBed.configureTestingModule({
      imports: [ChangePasswordComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    }).compileComponents();
    const auth = TestBed.inject(AuthApiService);
    const user = { id: 'user', name: 'QA', email: 'qa@example.test', role: UserRole.Teacher, mustChangePassword: true };
    auth.save({ token: 'old-token', expiresAt: '2099-01-01T00:00:00Z', user });
    const fixture = TestBed.createComponent(ChangePasswordComponent);
    fixture.detectChanges();
    for (const [control, value] of Object.entries({ currentPassword: 'Antiga123!', newPassword: 'NovaSenha123!', confirmPassword: 'NovaSenha123!' })) {
      const input = fixture.nativeElement.querySelector(`input[formControlName="${control}"]`) as HTMLInputElement;
      input.value = value;
      input.dispatchEvent(new Event('input'));
    }
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit', { bubbles: true, cancelable: true }));
    const http = TestBed.inject(HttpTestingController);
    const request = http.expectOne('/api/auth/change-password');
    const replacement = { token: 'new-token', expiresAt: '2099-01-02T00:00:00Z', user: { ...user, mustChangePassword: false } };

    request.flush(replacement);

    expect(auth.current()).toEqual(replacement);
    http.verify();
  });
});
