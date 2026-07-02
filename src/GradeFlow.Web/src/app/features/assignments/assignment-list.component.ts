import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BehaviorSubject, switchMap } from 'rxjs';
import { AssignmentApiService } from '../../core/api/assignment-api.service';

@Component({
  selector: 'app-assignment-list',
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './assignment-list.component.html'
})
export class AssignmentListComponent {
  private readonly assignmentApi = inject(AssignmentApiService);
  private readonly reload$ = new BehaviorSubject<void>(undefined);
  protected readonly assignments$ = this.reload$.pipe(switchMap(() => this.assignmentApi.getAll()));

  delete(id: string) {
    if (!confirm('Excluir esta avaliação?')) return;

    this.assignmentApi.delete(id).subscribe(() => this.reload$.next());
  }
}
