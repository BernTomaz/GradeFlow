import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AssignmentApiService } from '../../core/api/assignment-api.service';

@Component({
  selector: 'app-assignment-list',
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './assignment-list.component.html'
})
export class AssignmentListComponent {
  private readonly assignmentApi = inject(AssignmentApiService);
  protected readonly assignments$ = this.assignmentApi.getAll();
}
