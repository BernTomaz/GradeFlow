import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { QuestionApiService } from '../../core/api/question-api.service';
import { QuestionType, questionTypeOptions } from '../../core/models/question.models';
import { apiErrorMessage } from '../../shared/api-error';
import { buildQuestionText, parseQuestionText } from '../../shared/question-text';

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
  protected readonly multipleChoiceOptions = ['A', 'B', 'C', 'D'];
  protected assignmentId = this.route.snapshot.paramMap.get('assignmentId');
  protected readonly questionId = this.route.snapshot.paramMap.get('questionId');
  protected readonly readonly = !!this.questionId && !this.route.snapshot.url.some((segment) => segment.path === 'edit');
  protected loading = !!this.questionId;
  protected errorMessage: string | null = null;
  protected readonly form = this.fb.nonNullable.group({
    text: ['', [Validators.required, Validators.maxLength(4000)]],
    optionA: [''],
    optionB: [''],
    optionC: [''],
    optionD: [''],
    type: [QuestionType.MultipleChoice, Validators.required],
    points: [1, [Validators.required, Validators.min(0.01), Validators.max(10)]],
    order: [1, [Validators.required, Validators.min(1)]],
    correctAnswer: ['', [Validators.required, Validators.maxLength(2000)]],
    acceptedAnswers: [''],
    tolerance: [0],
    feedbackCorrect: [''],
    feedbackIncorrect: ['']
  });

  constructor() {
    this.form.controls.type.valueChanges.subscribe((type) => {
      if (type === QuestionType.TrueFalse && !['true', 'false'].includes(this.form.controls.correctAnswer.value)) {
        this.form.controls.correctAnswer.setValue('true');
      }

      if (type === QuestionType.MultipleChoice && !this.multipleChoiceOptions.includes(this.form.controls.correctAnswer.value)) {
        this.form.controls.correctAnswer.setValue('A');
      }
    });

    if (!this.questionId) return;

    this.questionApi.getById(this.questionId).subscribe((question) => {
      this.assignmentId = question.assignmentId;
      this.form.patchValue({
        ...parseQuestionText(question.type, question.text),
        type: question.type,
        points: question.points,
        order: question.order,
        correctAnswer: normalizeCorrectAnswer(question.type, question.answerKey?.correctAnswer ?? ''),
        acceptedAnswers: question.answerKey?.acceptedAnswers?.join(', ') ?? '',
        tolerance: question.answerKey?.tolerance ?? 0,
        feedbackCorrect: question.answerKey?.feedbackCorrect ?? '',
        feedbackIncorrect: question.answerKey?.feedbackIncorrect ?? ''
      });
      if (this.readonly) this.form.disable();
      this.loading = false;
    });
  }

  save() {
    this.errorMessage = null;
    if (this.form.invalid || !this.assignmentId) return;

    const value = this.form.getRawValue();
    const request = {
      text: buildQuestionText(value.type, value.text, value.optionA, value.optionB, value.optionC, value.optionD),
      type: value.type,
      points: Number(value.points),
      order: Number(value.order),
      answerKey: {
        correctAnswer: value.correctAnswer,
        acceptedAnswers: value.type === QuestionType.ShortText ? splitCsv(value.acceptedAnswers) : null,
        tolerance: value.type === QuestionType.Numeric ? Number(value.tolerance || 0) : null,
        feedbackCorrect: value.feedbackCorrect || null,
        feedbackIncorrect: value.feedbackIncorrect || null
      }
    };

    if (this.questionId) {
      this.questionApi.update(this.questionId, request).subscribe({
        next: () => this.router.navigate(['/assignments', this.assignmentId]),
        error: (error) => this.handleError(error)
      });
      return;
    }

    this.questionApi
      .create(this.assignmentId, request)
      .subscribe({
        next: () => this.router.navigate(['/assignments', this.assignmentId]),
        error: (error) => this.handleError(error)
      });
  }

  private handleError(error: { error?: { error?: string }; message?: string }) {
    this.errorMessage = apiErrorMessage(error, 'Não foi possível salvar a questão.');
  }
}

function splitCsv(value: string) {
  const values = value.split(',').map((item) => item.trim()).filter(Boolean);
  return values.length ? values : null;
}

function normalizeCorrectAnswer(type: QuestionType, value: string) {
  if (type !== QuestionType.TrueFalse) return value;

  const normalized = value.trim().toLowerCase();
  if (normalized === 'verdadeiro' || normalized === 'v') return 'true';
  if (normalized === 'falso' || normalized === 'f') return 'false';
  return normalized;
}
