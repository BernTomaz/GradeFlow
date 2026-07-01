import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map, switchMap } from 'rxjs';
import { CorrectionApiService } from '../../core/api/correction-api.service';

@Component({
  selector: 'app-correction-result',
  imports: [AsyncPipe, RouterLink],
  templateUrl: './correction-result.component.html'
})
export class CorrectionResultComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly correctionApi = inject(CorrectionApiService);
  private readonly submissionId$ = this.route.paramMap.pipe(map((params) => params.get('id')!));
  protected readonly correction$ = this.submissionId$.pipe(
    switchMap((id) => this.correctionApi.correct(id))
  );
}
