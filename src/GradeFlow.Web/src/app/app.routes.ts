import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { AssignmentCreateComponent } from './features/assignments/assignment-create.component';
import { AssignmentDetailComponent } from './features/assignments/assignment-detail.component';
import { AssignmentListComponent } from './features/assignments/assignment-list.component';
import { ChangePasswordComponent } from './features/auth/change-password.component';
import { LoginComponent } from './features/auth/login.component';
import { CorrectionResultComponent } from './features/correction/correction-result.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { UserRole } from './core/models/auth.models';
import { QuestionCreateComponent } from './features/questions/question-create.component';
import { SubmissionCreateComponent } from './features/submissions/submission-create.component';
import { SubmissionDetailComponent } from './features/submissions/submission-detail.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'change-password', component: ChangePasswordComponent, canActivate: [authGuard] },
  { path: 'assignments', component: AssignmentListComponent, canActivate: [authGuard] },
  { path: 'assignments/new', component: AssignmentCreateComponent, canActivate: [authGuard], data: { roles: [UserRole.Admin, UserRole.Teacher] } },
  { path: 'assignments/:id/edit', component: AssignmentCreateComponent, canActivate: [authGuard], data: { roles: [UserRole.Admin, UserRole.Teacher] } },
  {
    path: 'assignments/:id/report',
    loadComponent: () => import('./features/assignments/assignment-report.component').then((m) => m.AssignmentReportComponent),
    canActivate: [authGuard]
  },
  { path: 'assignments/:id', component: AssignmentDetailComponent, canActivate: [authGuard] },
  { path: 'assignments/:assignmentId/questions/new', component: QuestionCreateComponent, canActivate: [authGuard], data: { roles: [UserRole.Admin, UserRole.Teacher] } },
  { path: 'assignments/:assignmentId/questions/:questionId/edit', component: QuestionCreateComponent, canActivate: [authGuard], data: { roles: [UserRole.Admin, UserRole.Teacher] } },
  { path: 'questions/:questionId', component: QuestionCreateComponent, canActivate: [authGuard], data: { roles: [UserRole.Admin, UserRole.Teacher] } },
  { path: 'assignments/:assignmentId/submissions/new', component: SubmissionCreateComponent, canActivate: [authGuard] },
  { path: 'submissions/:id', component: SubmissionDetailComponent, canActivate: [authGuard] },
  { path: 'submissions/:id/edit', component: SubmissionCreateComponent, canActivate: [authGuard], data: { roles: [UserRole.Admin, UserRole.Teacher] } },
  { path: 'submissions/:id/answers/:answerId/edit', component: SubmissionCreateComponent, canActivate: [authGuard] },
  { path: 'submissions/:id/questions/:questionId/answer', component: SubmissionCreateComponent, canActivate: [authGuard] },
  { path: 'submissions/:id/correction', component: CorrectionResultComponent, canActivate: [authGuard], data: { roles: [UserRole.Admin, UserRole.Teacher] } }
];
