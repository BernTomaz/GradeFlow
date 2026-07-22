import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { combineLatest, map, switchMap } from 'rxjs';
import { AssignmentApiService } from '../../core/api/assignment-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';
import { apiErrorMessage } from '../../shared/api-error';
import { questionTitle } from '../../shared/question-text';

@Component({
  selector: 'app-assignment-report',
  imports: [AsyncPipe, RouterLink],
  templateUrl: './assignment-report.component.html'
})
export class AssignmentReportComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly assignmentApi = inject(AssignmentApiService);
  private readonly submissionApi = inject(SubmissionApiService);
  protected errorMessage: string | null = null;
  protected readonly questionTitle = questionTitle;

  protected readonly vm$ = this.route.paramMap.pipe(
    map((params) => params.get('id')!),
    switchMap((id) =>
      combineLatest({
        assignment: this.assignmentApi.getById(id),
        report: this.submissionApi.getReport(id)
      })
    )
  );

  exportCsv(assignmentId: string) {
    this.downloadExport(this.submissionApi.exportCsv(assignmentId), `gradeflow-${assignmentId}-notas.csv`, 'Não foi possível exportar o CSV.');
  }

  exportExcel(assignmentId: string) {
    this.downloadExport(this.submissionApi.exportExcel(assignmentId), `gradeflow-${assignmentId}-notas.xlsx`, 'Não foi possível exportar o Excel.');
  }

  exportPdf(assignmentId: string) {
    this.downloadExport(this.submissionApi.exportPdf(assignmentId), `gradeflow-${assignmentId}-relatorio.pdf`, 'Não foi possível exportar o PDF.');
  }

  private downloadExport(request: ReturnType<SubmissionApiService['exportCsv']>, fileName: string, fallback: string) {
    this.errorMessage = null;
    request.subscribe({
      next: (file) => {
        const url = URL.createObjectURL(file);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        link.click();
        URL.revokeObjectURL(url);
      },
      error: (error) => {
        this.errorMessage = apiErrorMessage(error, fallback);
      }
    });
  }
}
