import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map, switchMap } from 'rxjs';
import { QuestionApiService } from '../../core/api/question-api.service';
import { QuestionResponse } from '../../core/models/question.models';
import { SubmissionApiService } from '../../core/api/submission-api.service';

@Component({
  selector: 'app-correction-result',
  imports: [AsyncPipe, RouterLink],
  templateUrl: './correction-result.component.html'
})
export class CorrectionResultComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly submissionApi = inject(SubmissionApiService);
  private readonly questionApi = inject(QuestionApiService);
  private readonly submissionId$ = this.route.paramMap.pipe(map((params) => params.get('id')!));
  protected readonly correction$ = this.submissionId$.pipe(
    switchMap((id) => this.submissionApi.getById(id)),
    switchMap((submission) =>
      this.questionApi.getByAssignmentId(submission.assignmentId).pipe(
        map((questions) => ({
          submissionId: submission.id,
          finalScore: submission.finalScore,
          maxScore: questions.reduce((total, question) => total + question.points, 0),
          results: withQuestionOrder(submission.answers, questions)
        }))
      )
    )
  );
}

function withQuestionOrder<T extends { questionId: string }>(results: T[], questions: QuestionResponse[]) {
  const orderByQuestionId = new Map(questions.map((question) => [question.id, question.order]));
  return results.map((result) => ({
    ...result,
    questionOrder: orderByQuestionId.get(result.questionId)
  })).sort((left, right) => (left.questionOrder ?? 0) - (right.questionOrder ?? 0));
}
