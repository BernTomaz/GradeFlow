import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { combineLatest, map, startWith, Subject, switchMap } from 'rxjs';
import { AssignmentApiService } from '../../core/api/assignment-api.service';
import { QuestionApiService } from '../../core/api/question-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';
import { apiErrorMessage } from '../../shared/api-error';
import { LocalDatePipe } from '../../shared/local-date.pipe';
import { questionTitle } from '../../shared/question-text';

@Component({
  selector: 'app-assignment-detail',
  imports: [AsyncPipe, DatePipe, LocalDatePipe, RouterLink],
  templateUrl: './assignment-detail.component.html'
})
export class AssignmentDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly assignmentApi = inject(AssignmentApiService);
  private readonly questionApi = inject(QuestionApiService);
  private readonly submissionApi = inject(SubmissionApiService);
  private readonly assignmentId$ = this.route.paramMap.pipe(map((params) => params.get('id')!));
  private readonly refresh$ = new Subject<void>();
  protected pendingDelete: { type: 'question' | 'submission'; id: string } | null = null;
  protected errorMessage: string | null = null;
  protected showCsvHelp = false;

  protected readonly vm$ = this.assignmentId$.pipe(
    switchMap((id) =>
      this.refresh$.pipe(
        startWith(undefined),
        switchMap(() =>
          combineLatest({
            assignment: this.assignmentApi.getById(id),
            questions: this.questionApi.getByAssignmentId(id),
            submissions: this.submissionApi.getByAssignmentId(id),
            report: this.submissionApi.getReport(id)
          })
        )
      )
    )
  );

  protected readonly questionTitle = questionTitle;

  askDeleteQuestion(id: string) {
    this.pendingDelete = { type: 'question', id };
  }

  askDeleteSubmission(id: string) {
    this.pendingDelete = { type: 'submission', id };
  }

  deletePending() {
    if (!this.pendingDelete) return;

    const pendingDelete = this.pendingDelete;
    this.pendingDelete = null;
    this.errorMessage = null;
    const request = pendingDelete.type === 'question'
      ? this.questionApi.delete(pendingDelete.id)
      : this.submissionApi.delete(pendingDelete.id);
    request.subscribe({
      next: () => this.refresh$.next(),
      error: (error) => {
        this.errorMessage = apiErrorMessage(error, 'Não foi possível excluir.');
      }
    });
  }

  importCsv(assignmentId: string, event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.errorMessage = null;
    this.submissionApi.importCsv(assignmentId, file).subscribe({
      next: () => {
        input.value = '';
        this.refresh$.next();
      },
      error: (error) => {
        input.value = '';
        this.errorMessage = apiErrorMessage(error, 'Não foi possível importar o CSV.');
      }
    });
  }

  exportCsv(assignmentId: string) {
    this.downloadExport(
      this.submissionApi.exportCsv(assignmentId),
      `gradeflow-${assignmentId}-notas.csv`,
      'Não foi possível exportar o CSV.');
  }

  exportExcel(assignmentId: string) {
    this.downloadExport(
      this.submissionApi.exportExcel(assignmentId),
      `gradeflow-${assignmentId}-notas.xlsx`,
      'Não foi possível exportar o Excel.');
  }

  exportPdf(assignmentId: string) {
    this.downloadExport(
      this.submissionApi.exportPdf(assignmentId),
      `gradeflow-${assignmentId}-relatorio.pdf`,
      'Não foi possível exportar o PDF.');
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
