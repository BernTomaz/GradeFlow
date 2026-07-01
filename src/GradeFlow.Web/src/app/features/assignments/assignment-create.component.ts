import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AssignmentApiService } from '../../core/api/assignment-api.service';

@Component({
  selector: 'app-assignment-create',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './assignment-create.component.html'
})
export class AssignmentCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly assignmentApi = inject(AssignmentApiService);
  private readonly router = inject(Router);
  protected readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    subject: [''],
    description: ['']
  });

  save() {
    if (this.form.invalid) return;

    this.assignmentApi.create(this.form.getRawValue()).subscribe((assignment) => {
      this.router.navigate(['/assignments', assignment.id]);
    });
  }
}
