import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { map, switchMap } from 'rxjs';
import { CorrectionApiService } from '../../core/api/correction-api.service';
import { SubmissionApiService } from '../../core/api/submission-api.service';

@Component({
  selector: 'app-submission-detail',
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './submission-detail.component.html'
})
export class SubmissionDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly submissionApi = inject(SubmissionApiService);
  private readonly correctionApi = inject(CorrectionApiService);
  private readonly submissionId$ = this.route.paramMap.pipe(map((params) => params.get('id')!));
  protected readonly submission$ = this.submissionId$.pipe(
    switchMap((id) => this.submissionApi.getById(id))
  );

  correct(submissionId: string) {
    this.correctionApi.correct(submissionId).subscribe(() => {
      this.router.navigate(['/submissions', submissionId, 'correction']);
    });
  }
}
