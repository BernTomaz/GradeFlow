import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AssignmentApiService } from '../../core/api/assignment-api.service';

@Component({
  selector: 'app-assignment-create',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './assignment-create.component.html'
})
export class AssignmentCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly assignmentApi = inject(AssignmentApiService);
  private readonly router = inject(Router);
  protected readonly assignmentId = this.route.snapshot.paramMap.get('id');
  protected loading = !!this.assignmentId;
  protected readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    subject: [''],
    description: ['']
  });

  constructor() {
    if (!this.assignmentId) return;

    this.assignmentApi.getById(this.assignmentId).subscribe((assignment) => {
      this.form.patchValue({
        title: assignment.title,
        subject: assignment.subject ?? '',
        description: assignment.description ?? ''
      });
      this.loading = false;
    });
  }

  save() {
    if (this.form.invalid) return;

    const request = this.form.getRawValue();
    if (this.assignmentId) {
      this.assignmentApi.update(this.assignmentId, request).subscribe(() => {
        this.router.navigate(['/assignments', this.assignmentId]);
      });
      return;
    }

    this.assignmentApi.create(request).subscribe((assignment) => {
      this.router.navigate(['/assignments', assignment.id]);
    });
  }
}
