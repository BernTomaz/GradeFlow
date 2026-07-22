import { AsyncPipe, DecimalPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, forkJoin, map, of, switchMap } from 'rxjs';
import { AssignmentApiService } from '../../core/api/assignment-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';

@Component({
  selector: 'app-dashboard',
  imports: [AsyncPipe, DecimalPipe, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent {
  private readonly assignmentApi = inject(AssignmentApiService);
  private readonly submissionApi = inject(SubmissionApiService);
  protected chartType: 'bar' | 'pie' = 'bar';

  protected readonly vm$ = this.assignmentApi.getAll().pipe(
    switchMap((assignments) => {
      if (!assignments.length) {
        return of({ assignments, submissionCount: 0, averageScore: 0, pendingReviews: 0, chart: [], pie: '' });
      }

      return forkJoin(assignments.map((assignment) =>
        forkJoin({
          report: this.submissionApi.getReport(assignment.id).pipe(catchError(() => of(null))),
          submissions: this.submissionApi.getByAssignmentId(assignment.id).pipe(catchError(() => of([])))
        })
      )).pipe(
        map((items) => {
          const submissionCount = items.reduce((sum, item) => sum + (item.report?.submissionCount ?? item.submissions.length), 0);
          const weightedScore = items.reduce((sum, item) => sum + ((item.report?.averageScore ?? 0) * (item.report?.submissionCount ?? 0)), 0);
          const pendingReviews = items.reduce(
            (sum, item) => sum + item.submissions.flatMap((submission) => submission.answers).filter((answer) => answer.needsReview).length,
            0);
          const chartBase = items
            .map((item, index) => ({
              title: assignments[index].title,
              averageScore: item.report?.averageScore ?? 0
            }))
            .filter((item) => item.averageScore > 0)
            .slice(0, 6);
          const totalScore = chartBase.reduce((sum, item) => sum + item.averageScore, 0);
          let cursor = 0;
          const colors = ['#2563eb', '#16a34a', '#f59e0b', '#dc2626', '#7c3aed', '#0891b2'];
          const chart = chartBase.map((item, index) => {
            const start = cursor;
            cursor += totalScore ? (item.averageScore / totalScore) * 100 : 0;

            return {
              ...item,
              color: colors[index % colors.length],
              percent: Math.min(100, Math.max(0, item.averageScore * 10)),
              slice: `${colors[index % colors.length]} ${start}% ${cursor}%`
            };
          });

          return {
            assignments,
            submissionCount,
            averageScore: submissionCount ? weightedScore / submissionCount : 0,
            pendingReviews,
            chart,
            pie: `conic-gradient(${chart.map((item) => item.slice).join(', ')})`
          };
        })
      );
    })
  );

  protected setChartType(chartType: 'bar' | 'pie') {
    this.chartType = chartType;
  }

  protected exportPdf() {
    window.print();
  }
}
