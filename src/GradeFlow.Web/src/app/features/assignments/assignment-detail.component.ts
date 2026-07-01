import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { combineLatest, map, switchMap } from 'rxjs';
import { AssignmentApiService } from '../../core/api/assignment-api.service';
import { QuestionApiService } from '../../core/api/question-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';
import { questionTypeOptions } from '../../core/models/question.models';

@Component({
  selector: 'app-assignment-detail',
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './assignment-detail.component.html'
})
export class AssignmentDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly assignmentApi = inject(AssignmentApiService);
  private readonly questionApi = inject(QuestionApiService);
  private readonly submissionApi = inject(SubmissionApiService);
  private readonly assignmentId$ = this.route.paramMap.pipe(map((params) => params.get('id')!));

  protected readonly vm$ = this.assignmentId$.pipe(
    switchMap((id) =>
      combineLatest({
        assignment: this.assignmentApi.getById(id),
        questions: this.questionApi.getByAssignmentId(id),
        submissions: this.submissionApi.getByAssignmentId(id)
      })
    )
  );

  typeLabel(type: number) {
    return questionTypeOptions.find((option) => option.value === type)?.label ?? 'Tipo desconhecido';
  }
}
