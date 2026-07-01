import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateSubmissionRequest, SubmissionResponse } from '../models/submission.models';

@Injectable({ providedIn: 'root' })
export class SubmissionApiService {
  constructor(private readonly http: HttpClient) {}

  getByAssignmentId(assignmentId: string) {
    return this.http.get<SubmissionResponse[]>(`/api/assignments/${assignmentId}/submissions`);
  }

  getById(id: string) {
    return this.http.get<SubmissionResponse>(`/api/submissions/${id}`);
  }

  create(assignmentId: string, request: CreateSubmissionRequest) {
    return this.http.post<SubmissionResponse>(`/api/assignments/${assignmentId}/submissions`, request);
  }
}
