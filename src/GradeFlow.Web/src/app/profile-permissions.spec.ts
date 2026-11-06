import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { App } from './app';
import { AuthApiService } from './core/api/auth-api.service';
import { UserRole } from './core/models/auth.models';
import { AssignmentListComponent } from './features/assignments/assignment-list.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { AssignmentDetailComponent } from './features/assignments/assignment-detail.component';
import { AssignmentReportComponent } from './features/assignments/assignment-report.component';
import { SubmissionDetailComponent } from './features/submissions/submission-detail.component';

describe('Profile permissions', () => {
  afterEach(() => sessionStorage.removeItem('gradeflow.auth'));

  for (const component of [App, AssignmentListComponent, DashboardComponent]) {
    it(`hides assignment creation from students in ${component.name}`, async () => {
      await TestBed.configureTestingModule({
        imports: [component],
        providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
      }).compileComponents();
      const auth = TestBed.inject(AuthApiService);
      auth.save({ token: 'test', expiresAt: '2099-01-01', user: {
        id: 'student', name: 'QA', email: 'qa@example.test', role: UserRole.Student, mustChangePassword: false
      } });
      const fixture = TestBed.createComponent(component as typeof App);
      fixture.detectChanges();
      const http = TestBed.inject(HttpTestingController);
      http.match('/api/assignments').forEach(request => request.flush([]));
      fixture.detectChanges();
      expect(fixture.nativeElement.querySelector('a[href="/assignments/new"]')).toBeNull();
      auth.save({ ...auth.current()!, user: { ...auth.current()!.user, role: UserRole.Teacher } });
      fixture.detectChanges();
      expect(fixture.nativeElement.querySelector('a[href="/assignments/new"]')).not.toBeNull();
      http.verify();
    });
  }

  it('lets students open questions and submit without administrative actions', async () => {
    await TestBed.configureTestingModule({
      imports: [AssignmentDetailComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([]), {
        provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ id: 'assignment' })) }
      }]
    }).compileComponents();
    const auth = TestBed.inject(AuthApiService);
    auth.save({ token: 'test', expiresAt: '2099-01-01', user: {
      id: 'student', name: 'QA', email: 'qa@example.test', role: UserRole.Student, mustChangePassword: false
    } });
    const fixture = TestBed.createComponent(AssignmentDetailComponent);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/assignments/assignment').flush({ id: 'assignment', title: 'Prova', subject: 'QA' });
    http.expectOne('/api/assignments/assignment/questions').flush([
      { id: 'question', text: 'Enunciado', order: 1, answerKey: null }
    ]);
    http.expectOne('/api/assignments/assignment/submissions').flush([]);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Enunciado');
    expect(fixture.nativeElement.querySelector('a[href="/assignments/assignment/submissions/new"]')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/assignments/assignment/edit"]')).toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/assignments/assignment/report"]')).toBeNull();
    expect(fixture.nativeElement.querySelector('input[type="file"]')).toBeNull();
    expect(fixture.nativeElement.textContent).not.toContain('Gabarito:');
    auth.save({ ...auth.current()!, user: { ...auth.current()!.user, role: UserRole.Teacher } });
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('a[href="/assignments/assignment/edit"]')).not.toBeNull();
    http.verify();
  });

  for (const component of [AssignmentDetailComponent, AssignmentReportComponent, SubmissionDetailComponent]) {
    it(`shows an unavailable message instead of an unhandled error in ${component.name}`, async () => {
      await TestBed.configureTestingModule({
        imports: [component],
        providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([]), {
          provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ id: 'missing' })) }
        }]
      }).compileComponents();
      const fixture = TestBed.createComponent(component as typeof AssignmentDetailComponent);
      fixture.detectChanges();
      const http = TestBed.inject(HttpTestingController);
      const requests = http.match(() => true);
      requests[0].flush(null, { status: 404, statusText: 'Not Found' });
      requests.slice(1).filter(request => !request.cancelled).forEach(request => request.flush([]));
      fixture.detectChanges();
      expect(fixture.nativeElement.querySelector('[role="alert"]')?.textContent)
        .toContain('não encontrado ou você não tem permissão');
      expect(fixture.nativeElement.querySelector('a[href="/assignments"]')).not.toBeNull();
      http.verify();
    });
  }
});
