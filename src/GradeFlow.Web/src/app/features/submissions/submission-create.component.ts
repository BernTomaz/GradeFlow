import { Component, inject } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { QuestionApiService } from '../../core/api/question-api.service';
import { CorrectionApiService } from '../../core/api/correction-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';
import { QuestionResponse, QuestionType } from '../../core/models/question.models';
import { CreateStudentAnswerRequest } from '../../core/models/submission.models';

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
  private readonly correctionApi = inject(CorrectionApiService);
  private readonly submissionApi = inject(SubmissionApiService);
  protected assignmentId = this.route.snapshot.paramMap.get('assignmentId');
  protected readonly submissionId = this.route.snapshot.paramMap.get('id');
  protected readonly answerId = this.route.snapshot.paramMap.get('answerId');
  protected readonly questionId = this.route.snapshot.paramMap.get('questionId');
  protected errorMessage: string | null = null;
  protected readonly QuestionType = QuestionType;
  protected readonly trueFalseOptions = [
    { value: 'true', text: 'Verdadeiro' },
    { value: 'false', text: 'Falso' }
  ];
  protected loading = !!this.submissionId || !!this.assignmentId;
  protected saving = false;
  protected questions: QuestionResponse[] = [];
  private originalAnswers: CreateStudentAnswerRequest[] = [];
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
        this.originalAnswers = submission.answers.map((answer) => ({
          questionId: answer.questionId,
          answer: answer.answer
        }));
        this.form.patchValue({
          studentName: submission.studentName,
          studentEmail: submission.studentEmail ?? ''
        });
        const selectedAnswer = this.answerId
          ? submission.answers.find((answer) => answer.id === this.answerId)
          : null;
        const selectedQuestionId = selectedAnswer?.questionId ?? this.questionId ?? undefined;

        this.loadQuestions(
          submission.assignmentId,
          selectedAnswer ? [selectedAnswer] : submission.answers,
          selectedQuestionId
        );
      });
      return;
    }

    if (this.assignmentId) this.loadQuestions(this.assignmentId);
  }

  private loadQuestions(
    assignmentId: string,
    answers: { questionId: string; answer: string }[] = [],
    questionId?: string
  ) {
    this.questionApi.getByAssignmentId(assignmentId).subscribe((questions) => {
      this.questions = questionId ? questions.filter((question) => question.id === questionId) : questions;
      this.questions.forEach((question) => {
        const existingAnswer = answers.find((answer) => answer.questionId === question.id)?.answer ?? '';
        this.answers.push(
          this.fb.nonNullable.group({
            questionId: [question.id],
            answer: [existingAnswer, Validators.required]
          })
        );
      });
      this.loading = false;
    });
  }

  save() {
    if (this.submissionId && (this.answerId || this.questionId)) {
      this.saveCurrentAnswer(() => this.router.navigate(['/submissions', this.submissionId]));
      return;
    }

    this.saveSubmission(() => this.router.navigate(['/submissions', this.submissionId]));
  }

  correctQuestion() {
    const questionId = this.questions[0]?.id;
    if (!this.submissionId || !questionId) return;

    this.saving = true;
    this.correctionApi.correctQuestion(this.submissionId, questionId).subscribe({
      next: () => this.router.navigate(['/submissions', this.submissionId]),
      error: (error) => this.handleError(error)
    });
  }

  private saveCurrentAnswer(afterUpdate: () => void) {
    const questionId = this.questions[0]?.id;
    const answer = this.answers.at(0)?.value?.answer;
    if (!this.submissionId || !questionId || !answer) return;

    this.saving = true;
    this.submissionApi.updateAnswer(this.submissionId, questionId, answer).subscribe({
      next: afterUpdate,
      error: (error) => this.handleError(error)
    });
  }

  private saveSubmission(afterUpdate?: () => void) {
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
    const formAnswers = value.answers as CreateStudentAnswerRequest[];
    const request = {
      studentName: value.studentName,
      studentEmail: value.studentEmail || null,
      answers: this.answerId || this.questionId ? this.mergeAnswer(formAnswers) : formAnswers
    };
    this.saving = true;

    if (this.submissionId) {
      this.submissionApi.update(this.submissionId, request).subscribe({
        next: () => (afterUpdate ? afterUpdate() : this.router.navigate(['/submissions', this.submissionId])),
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

  private mergeAnswer(formAnswers: CreateStudentAnswerRequest[]) {
    return [
      ...this.originalAnswers.map((answer) => formAnswers.find((x) => x.questionId === answer.questionId) ?? answer),
      ...formAnswers.filter((answer) => !this.originalAnswers.some((x) => x.questionId === answer.questionId))
    ];
  }

  questionTitle(question: QuestionResponse) {
    return question.text.split('\n')[0];
  }

  multipleChoiceOptions(question: QuestionResponse) {
    return ['A', 'B', 'C', 'D']
      .map((option) => {
        const text = question.text.split('\n').find((line) => line.startsWith(`${option}) `))?.slice(3);
        return text ? { value: option, text } : null;
      })
      .filter((option): option is { value: string; text: string } => option !== null);
  }
}
