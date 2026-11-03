import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter, Router } from '@angular/router';
import { QuestionType } from '../../core/models/question.models';
import { QuestionCreateComponent } from './question-create.component';

describe('QuestionCreateComponent numeric answer', () => {
  for (const editing of [false, true]) {
    it(`sends the numeric answer as text when ${editing ? 'editing' : 'creating'}`, async () => {
      await TestBed.configureTestingModule({
        imports: [QuestionCreateComponent],
        providers: [
          provideHttpClient(), provideHttpClientTesting(), provideRouter([]),
          { provide: ActivatedRoute, useValue: { snapshot: {
            paramMap: convertToParamMap({ assignmentId: 'assignment', ...(editing ? { questionId: 'question' } : {}) }),
            url: editing ? [{ path: 'edit' }] : []
          } } }
        ]
      }).compileComponents();
      spyOn(TestBed.inject(Router), 'navigate').and.resolveTo(true);
      const http = TestBed.inject(HttpTestingController);
      const fixture = TestBed.createComponent(QuestionCreateComponent);
      if (editing) {
        http.expectOne('/api/questions/question').flush({
          id: 'question', assignmentId: 'assignment', text: 'Quanto é dez?',
          type: QuestionType.Numeric, points: 1, order: 1,
          answerKey: { correctAnswer: '10', tolerance: 0.1 }
        });
      }
      fixture.detectChanges();
      const type = fixture.nativeElement.querySelector('select[formControlName="type"]') as HTMLSelectElement;
      type.selectedIndex = QuestionType.Numeric;
      type.dispatchEvent(new Event('change'));
      fixture.detectChanges();
      const text = fixture.nativeElement.querySelector('textarea') as HTMLTextAreaElement;
      text.value = 'Quanto é dez?';
      text.dispatchEvent(new Event('input'));
      const answer = fixture.nativeElement.querySelector('input[formControlName="correctAnswer"]') as HTMLInputElement;
      answer.value = '10.05';
      answer.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit', { bubbles: true, cancelable: true }));

      const request = http.expectOne(editing ? '/api/questions/question' : '/api/assignments/assignment/questions');
      expect(request.request.method).toBe(editing ? 'PUT' : 'POST');
      expect(request.request.body.answerKey.correctAnswer).toBe('10.05');
      request.flush({});
      http.verify();
    });
  }
});
