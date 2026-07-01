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
  protected readonly assignmentId = this.route.snapshot.paramMap.get('id')!;
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
    this.questionApi.getByAssignmentId(this.assignmentId).subscribe((questions) => {
      this.questions = questions;
      questions.forEach((question) => {
        this.answers.push(
          this.fb.nonNullable.group({
            questionId: [question.id],
            answer: ['', Validators.required]
          })
        );
      });
    });
  }

  save() {
    if (this.form.invalid) return;

    const value = this.form.getRawValue();
    this.submissionApi
      .create(this.assignmentId, {
        studentName: value.studentName,
        studentEmail: value.studentEmail || null,
        answers: value.answers as { questionId: string; answer: string }[]
      })
      .subscribe((submission) => this.router.navigate(['/submissions', submission.id]));
  }
}
