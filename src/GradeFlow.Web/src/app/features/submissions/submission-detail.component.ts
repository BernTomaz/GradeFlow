import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { map, switchMap } from 'rxjs';
import { CorrectionApiService } from '../../core/api/correction-api.service';
import { QuestionApiService } from '../../core/api/question-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';

@Component({
  selector: 'app-submission-detail',
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './submission-detail.component.html'
})
export class SubmissionDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly submissionApi = inject(SubmissionApiService);
  private readonly questionApi = inject(QuestionApiService);
  private readonly correctionApi = inject(CorrectionApiService);
  private readonly submissionId$ = this.route.paramMap.pipe(map((params) => params.get('id')!));
  protected pendingDelete: { submissionId: string; assignmentId: string } | null = null;
  protected readonly vm$ = this.submissionId$.pipe(
    switchMap((id) => this.submissionApi.getById(id)),
    switchMap((submission) =>
      this.questionApi.getByAssignmentId(submission.assignmentId).pipe(
        map((questions) => ({
          submission,
          questions: questions.map((question) => ({
            question,
            answer: submission.answers.find((answer) => answer.questionId === question.id)
          }))
        }))
      )
    )
  );

  questionTitle(text: string) {
    return text.split('\n')[0];
  }

  correct(submissionId: string) {
    this.correctionApi.correct(submissionId).subscribe(() => {
      this.router.navigate(['/submissions', submissionId, 'correction']);
    });
  }

  askDelete(submissionId: string, assignmentId: string) {
    this.pendingDelete = { submissionId, assignmentId };
  }

  delete() {
    if (!this.pendingDelete) return;

    const pendingDelete = this.pendingDelete;
    this.pendingDelete = null;
    this.submissionApi.delete(pendingDelete.submissionId).subscribe(() => {
      this.router.navigate(['/assignments', pendingDelete.assignmentId]);
    });
  }
}
