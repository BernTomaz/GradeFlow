import { AsyncPipe, DecimalPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, forkJoin, map, of, switchMap } from 'rxjs';
import { AssignmentApiService } from '../../core/api/assignment-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';
import { SubmissionStatus } from '../../core/models/submission.models';

@Component({
  selector: 'app-dashboard',
  imports: [AsyncPipe, DecimalPipe, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent {
  private readonly assignmentApi = inject(AssignmentApiService);
  private readonly submissionApi = inject(SubmissionApiService);
  protected chartType: 'bar' | 'column' | 'pie' = 'bar';
  protected selectedAssignmentId = '';

  protected readonly vm$ = this.assignmentApi.getAll().pipe(
    switchMap((assignments) => {
      if (!assignments.length) {
        return of({ assignments, items: [] });
      }

      return forkJoin(assignments.map((assignment) =>
        forkJoin({
          report: this.submissionApi.getReport(assignment.id).pipe(catchError(() => of(null))),
          submissions: this.submissionApi.getByAssignmentId(assignment.id).pipe(catchError(() => of([])))
        })
      )).pipe(
        map((items) => {
          const colors = ['#2563eb', '#16a34a', '#f59e0b', '#dc2626', '#7c3aed', '#0891b2'];
          const dashboardItems = items.map((item, index) => {
            const chartBase = (item.report?.questions ?? [])
              .map((question) => {
                const answers = item.submissions.map((submission) => ({
                  answer: submission.answers.find((answer) => answer.questionId === question.questionId),
                  isSubmissionPending: submission.status === SubmissionStatus.Pending
                }));
                const pendingCount = answers.filter((item) => !item.answer || item.isSubmissionPending || item.answer.needsReview).length;
                const correctedAnswers = answers.filter((item) => item.answer && !item.isSubmissionPending && !item.answer.needsReview);
                const correctCount = correctedAnswers.filter((item) => item.answer?.isCorrect).length;
                return {
                  title: `Q${question.order}`,
                  averageScore: correctedAnswers.length ? (correctCount / correctedAnswers.length) * 100 : 0,
                  pending: pendingCount > 0 && correctedAnswers.length === 0
                };
              });
            const totalScore = chartBase.reduce((sum, question) => sum + question.averageScore, 0);
            let cursor = 0;
            const chart = chartBase.map((question, chartIndex) => {
              const start = cursor;
              cursor += totalScore ? (question.averageScore / totalScore) * 100 : 0;
              const color = colors[chartIndex % colors.length];

              return {
                ...question,
                color,
                label: question.pending ? 'Pendente' : `${question.averageScore.toFixed(0)}%`,
                percent: Math.min(100, Math.max(0, question.averageScore)),
                slice: `${color} ${start}% ${cursor}%`
              };
            });

            return {
              assignment: assignments[index],
              submissionCount: item.report?.submissionCount ?? item.submissions.length,
              averageScore: item.report?.averageScore ?? 0,
              questionCount: item.report?.questions.length ?? 0,
              pendingReviews: item.submissions.flatMap((submission) => submission.answers).filter((answer) => answer.needsReview).length,
              chart,
              pie: `conic-gradient(${chart.map((question) => question.slice).join(', ')})`
            };
          });
          this.selectedAssignmentId ||= assignments[0].id;

          return {
            assignments,
            items: dashboardItems
          };
        })
      );
    })
  );

  protected setChartType(chartType: 'bar' | 'column' | 'pie') {
    this.chartType = chartType;
  }

  protected setAssignment(id: string) {
    this.selectedAssignmentId = id;
  }

  protected selectedItem(vm: { items: any[] }) {
    return vm.items.find((item) => item.assignment.id === this.selectedAssignmentId) ?? vm.items[0];
  }

  protected exportPdf() {
    window.print();
  }
}
