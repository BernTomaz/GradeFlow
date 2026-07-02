import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { QuestionApiService } from '../../core/api/question-api.service';
import { QuestionType, questionTypeOptions } from '../../core/models/question.models';

@Component({
  selector: 'app-question-create',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './question-create.component.html'
})
export class QuestionCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly questionApi = inject(QuestionApiService);
  protected readonly QuestionType = QuestionType;
  protected readonly questionTypeOptions = questionTypeOptions;
  protected assignmentId = this.route.snapshot.paramMap.get('assignmentId');
  protected readonly questionId = this.route.snapshot.paramMap.get('questionId');
  protected loading = !!this.questionId;
  protected readonly form = this.fb.nonNullable.group({
    text: ['', [Validators.required, Validators.maxLength(4000)]],
    type: [QuestionType.MultipleChoice, Validators.required],
    points: [1, [Validators.required, Validators.min(0.01)]],
    order: [1, [Validators.required, Validators.min(1)]],
    correctAnswer: ['', [Validators.required, Validators.maxLength(2000)]],
    acceptedAnswers: [''],
    tolerance: [0],
    feedbackCorrect: [''],
    feedbackIncorrect: ['']
  });

  constructor() {
    if (!this.questionId) return;

    this.questionApi.getById(this.questionId).subscribe((question) => {
      this.assignmentId = question.assignmentId;
      this.form.patchValue({
        text: question.text,
        type: question.type,
        points: question.points,
        order: question.order,
        correctAnswer: question.answerKey?.correctAnswer ?? '',
        acceptedAnswers: question.answerKey?.acceptedAnswers?.join(', ') ?? '',
        tolerance: question.answerKey?.tolerance ?? 0,
        feedbackCorrect: question.answerKey?.feedbackCorrect ?? '',
        feedbackIncorrect: question.answerKey?.feedbackIncorrect ?? ''
      });
      this.loading = false;
    });
  }

  save() {
    if (this.form.invalid || !this.assignmentId) return;

    const value = this.form.getRawValue();
    const request = {
      text: value.text,
      type: value.type,
      points: Number(value.points),
      order: Number(value.order),
      answerKey: {
        correctAnswer: value.correctAnswer,
        acceptedAnswers: splitCsv(value.acceptedAnswers),
        tolerance: value.type === QuestionType.Numeric ? Number(value.tolerance || 0) : null,
        feedbackCorrect: value.feedbackCorrect || null,
        feedbackIncorrect: value.feedbackIncorrect || null
      }
    };

    if (this.questionId) {
      this.questionApi.update(this.questionId, request).subscribe(() => {
        this.router.navigate(['/assignments', this.assignmentId]);
      });
      return;
    }

    this.questionApi
      .create(this.assignmentId, request)
      .subscribe(() => this.router.navigate(['/assignments', this.assignmentId]));
  }
}

function splitCsv(value: string) {
  const values = value.split(',').map((item) => item.trim()).filter(Boolean);
  return values.length ? values : null;
}
