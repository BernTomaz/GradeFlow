import { Component, inject } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { QuestionApiService } from '../../core/api/question-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';
import { QuestionResponse } from '../../core/models/question.models';

@Component({
  selector: 'app-submission-create',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './submission-create.component.html'
})
export class SubmissionCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly questionApi = inject(QuestionApiService);
  private readonly submissionApi = inject(SubmissionApiService);
  protected assignmentId = this.route.snapshot.paramMap.get('assignmentId');
  protected readonly submissionId = this.route.snapshot.paramMap.get('id');
  protected errorMessage: string | null = null;
  protected saving = false;
  protected questions: QuestionResponse[] = [];
  protected readonly form = this.fb.nonNullable.group({
    studentName: ['', [Validators.required, Validators.maxLength(200)]],
    studentEmail: [''],
    answers: this.fb.array([])
  });

  get answers() {
    return this.form.controls.answers as FormArray;
  }

  constructor() {
    if (this.submissionId) {
      this.submissionApi.getById(this.submissionId).subscribe((submission) => {
        this.assignmentId = submission.assignmentId;
        this.form.patchValue({
          studentName: submission.studentName,
          studentEmail: submission.studentEmail ?? ''
        });
        this.loadQuestions(submission.assignmentId, submission.answers);
      });
      return;
    }

    if (this.assignmentId) this.loadQuestions(this.assignmentId);
  }

  private loadQuestions(
    assignmentId: string,
    answers: { questionId: string; answer: string }[] = []
  ) {
    this.questionApi.getByAssignmentId(assignmentId).subscribe((questions) => {
      this.questions = questions;
      questions.forEach((question) => {
        const existingAnswer = answers.find((answer) => answer.questionId === question.id)?.answer ?? '';
        this.answers.push(
          this.fb.nonNullable.group({
            questionId: [question.id],
            answer: [existingAnswer, Validators.required]
          })
        );
      });
    });
  }

  save() {
    this.errorMessage = null;

    if (this.form.invalid) {
      this.errorMessage = 'Preencha os campos obrigatórios.';
      return;
    }

    if (!this.assignmentId) {
      this.errorMessage = 'Avaliação não encontrada para esta submissão.';
      return;
    }

    const value = this.form.getRawValue();
    const request = {
      studentName: value.studentName,
      studentEmail: value.studentEmail || null,
      answers: value.answers as { questionId: string; answer: string }[]
    };
    this.saving = true;

    if (this.submissionId) {
      this.submissionApi.update(this.submissionId, request).subscribe({
        next: () => this.router.navigate(['/submissions', this.submissionId]),
        error: (error) => this.handleError(error)
      });
      return;
    }

    this.submissionApi.create(this.assignmentId, request).subscribe({
      next: (submission) => this.router.navigate(['/submissions', submission.id]),
      error: (error) => this.handleError(error)
    });
  }

  private handleError(error: { error?: { error?: string }; message?: string }) {
    this.saving = false;
    this.errorMessage = error.error?.error ?? error.message ?? 'Não foi possível salvar a submissão.';
  }
}
