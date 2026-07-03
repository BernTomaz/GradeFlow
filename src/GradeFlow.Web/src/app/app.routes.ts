import { Routes } from '@angular/router';
import { AssignmentCreateComponent } from './features/assignments/assignment-create.component';
import { AssignmentDetailComponent } from './features/assignments/assignment-detail.component';
import { AssignmentListComponent } from './features/assignments/assignment-list.component';
import { CorrectionResultComponent } from './features/correction/correction-result.component';
import { QuestionCreateComponent } from './features/questions/question-create.component';
import { SubmissionCreateComponent } from './features/submissions/submission-create.component';
import { SubmissionDetailComponent } from './features/submissions/submission-detail.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'assignments' },
  { path: 'assignments', component: AssignmentListComponent },
  { path: 'assignments/new', component: AssignmentCreateComponent },
  { path: 'assignments/:id/edit', component: AssignmentCreateComponent },
  { path: 'assignments/:id', component: AssignmentDetailComponent },
  { path: 'assignments/:assignmentId/questions/new', component: QuestionCreateComponent },
  { path: 'assignments/:assignmentId/questions/:questionId/edit', component: QuestionCreateComponent },
  { path: 'questions/:questionId', component: QuestionCreateComponent },
  { path: 'assignments/:assignmentId/submissions/new', component: SubmissionCreateComponent },
  { path: 'submissions/:id', component: SubmissionDetailComponent },
  { path: 'submissions/:id/edit', component: SubmissionCreateComponent },
  { path: 'submissions/:id/answers/:answerId/edit', component: SubmissionCreateComponent },
  { path: 'submissions/:id/questions/:questionId/answer', component: SubmissionCreateComponent },
  { path: 'submissions/:id/correction', component: CorrectionResultComponent }
];
