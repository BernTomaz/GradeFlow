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
  { path: 'assignments/:id', component: AssignmentDetailComponent },
  { path: 'assignments/:id/questions/new', component: QuestionCreateComponent },
  { path: 'assignments/:id/submissions/new', component: SubmissionCreateComponent },
  { path: 'submissions/:id', component: SubmissionDetailComponent },
  { path: 'submissions/:id/correction', component: CorrectionResultComponent }
];
