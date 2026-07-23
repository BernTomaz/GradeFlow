import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { UserRole } from './core/models/auth.models';

describe('App', () => {
  const storageKey = 'gradeflow.auth';

  beforeEach(async () => {
    localStorage.removeItem(storageKey);
    sessionStorage.removeItem(storageKey);
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideRouter([])]
    }).compileComponents();
  });

  afterEach(() => {
    localStorage.removeItem(storageKey);
    sessionStorage.removeItem(storageKey);
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the app navigation', () => {
    sessionStorage.setItem(storageKey, JSON.stringify({
      token: 'test-token',
      expiresAt: new Date(Date.now() + 60000).toISOString(),
      user: {
        id: 'user-id',
        name: 'Test User',
        email: 'test@example.com',
        role: UserRole.Teacher
      }
    }));

    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.brand')?.textContent).toContain('GradeFlow');
  });
});
