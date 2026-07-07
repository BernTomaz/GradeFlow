import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BehaviorSubject, forkJoin, map, switchMap } from 'rxjs';
import { QuestionApiService } from '../../core/api/question-api.service';
import { QuestionResponse } from '../../core/models/question.models';
import { SubmissionApiService } from '../../core/api/submission-api.service';
import { StudentAnswerResponse } from '../../core/models/submission.models';
import { LocalDatePipe } from '../../shared/local-date.pipe';

@Component({
  selector: 'app-correction-result',
  imports: [AsyncPipe, DatePipe, FormsModule, LocalDatePipe, RouterLink],
  templateUrl: './correction-result.component.html'
})
export class CorrectionResultComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly submissionApi = inject(SubmissionApiService);
  private readonly questionApi = inject(QuestionApiService);
  private readonly refresh$ = new BehaviorSubject<void>(undefined);
  protected savingAnswerId: string | null = null;
  private readonly submissionId$ = this.route.paramMap.pipe(map((params) => params.get('id')!));
  protected readonly correction$ = this.refresh$.pipe(
    switchMap(() => this.submissionId$),
    switchMap((id) => this.submissionApi.getById(id)),
    switchMap((submission) =>
      forkJoin({
        questions: this.questionApi.getByAssignmentId(submission.assignmentId),
        auditLogs: this.submissionApi.getCorrectionLogs(submission.id)
      }).pipe(
        map(({ questions, auditLogs }) => ({
          submissionId: submission.id,
          finalScore: submission.finalScore,
          maxScore: questions.reduce((total, question) => total + question.points, 0),
          results: withQuestionOrder(submission.answers, questions).map((result) => ({
            ...result,
            reviewScoreAwarded: result.scoreAwarded,
            reviewFeedback: result.feedback,
            reviewIsCorrect: result.isCorrect,
            auditLogs: withQuestionOrder(auditLogs, questions)
              .filter((log) => log.questionId === result.questionId)
              .sort((left, right) => Date.parse(right.createdAt) - Date.parse(left.createdAt))
          }))
        }))
      )
    )
  );

  protected review(result: ReviewResult) {
    this.savingAnswerId = result.id;
    this.submissionApi.reviewAnswer(result.id, {
      scoreAwarded: Number(result.reviewScoreAwarded),
      feedback: result.reviewFeedback,
      isCorrect: result.reviewIsCorrect
    }).subscribe({
      next: () => {
        this.savingAnswerId = null;
        this.refresh$.next();
      },
      error: () => {
        this.savingAnswerId = null;
      }
    });
  }
}

function withQuestionOrder<T extends { questionId: string }>(results: T[], questions: QuestionResponse[]) {
  const questionById = new Map(questions.map((question) => [question.id, question]));
  return results.map((result) => ({
    ...result,
    questionOrder: questionById.get(result.questionId)?.order,
    questionText: questionById.get(result.questionId)?.text,
    questionPoints: questionById.get(result.questionId)?.points,
    expectedAnswer: questionById.get(result.questionId)?.answerKey?.correctAnswer
  })).sort((left, right) => (left.questionOrder ?? 0) - (right.questionOrder ?? 0));
}

type ReviewResult = StudentAnswerResponse & {
  questionOrder?: number;
  questionText?: string;
  questionPoints?: number;
  expectedAnswer?: string;
  reviewScoreAwarded: number;
  reviewFeedback?: string | null;
  reviewIsCorrect: boolean;
  auditLogs: Array<{
    id: string;
    questionId: string;
    questionOrder?: number;
    originalAnswer: string;
    expectedAnswer?: string | null;
    score: number;
    message?: string | null;
    createdAt: string;
  }>;
};
