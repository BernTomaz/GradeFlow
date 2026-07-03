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
  protected pendingDeleteId: string | null = null;
  protected readonly assignments$ = this.reload$.pipe(switchMap(() => this.assignmentApi.getAll()));

  askDelete(id: string) {
    this.pendingDeleteId = id;
  }

  delete() {
    const id = this.pendingDeleteId;
    if (!id) return;

    this.pendingDeleteId = null;
    this.assignmentApi.delete(id).subscribe(() => this.reload$.next());
  }
}
