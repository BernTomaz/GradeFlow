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
  protected readonly multipleChoiceOptions = ['A', 'B', 'C', 'D'];
  protected assignmentId = this.route.snapshot.paramMap.get('assignmentId');
  protected readonly questionId = this.route.snapshot.paramMap.get('questionId');
  protected loading = !!this.questionId;
  protected readonly form = this.fb.nonNullable.group({
    text: ['', [Validators.required, Validators.maxLength(4000)]],
    optionA: [''],
    optionB: [''],
    optionC: [''],
    optionD: [''],
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
      this.loading = false;
    });
  }

  save() {
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

function normalizeCorrectAnswer(type: QuestionType, value: string) {
  if (type !== QuestionType.TrueFalse) return value;

  const normalized = value.trim().toLowerCase();
  if (normalized === 'verdadeiro' || normalized === 'v') return 'true';
  if (normalized === 'falso' || normalized === 'f') return 'false';
  return normalized;
}

function buildQuestionText(type: QuestionType, text: string, optionA: string, optionB: string, optionC: string, optionD: string) {
  if (type !== QuestionType.MultipleChoice) return text;

  return [
    text,
    `A) ${optionA}`,
    `B) ${optionB}`,
    `C) ${optionC}`,
    `D) ${optionD}`
  ].filter((line) => !line.endsWith(') ')).join('\n');
}

function parseQuestionText(type: QuestionType, text: string) {
  if (type !== QuestionType.MultipleChoice) return { text };

  const lines = text.split('\n');
  return {
    text: lines[0] ?? '',
    optionA: readOption(lines, 'A'),
    optionB: readOption(lines, 'B'),
    optionC: readOption(lines, 'C'),
    optionD: readOption(lines, 'D')
  };
}

function readOption(lines: string[], option: string) {
  return lines.find((line) => line.startsWith(`${option}) `))?.slice(3) ?? '';
}
