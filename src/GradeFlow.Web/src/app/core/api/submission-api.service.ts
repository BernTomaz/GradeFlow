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

  update(id: string, request: CreateSubmissionRequest) {
    return this.http.put<void>(`/api/submissions/${id}`, request);
  }

  updateStudentInfo(id: string, request: Pick<CreateSubmissionRequest, 'studentName' | 'studentEmail'>) {
    return this.http.put<void>(`/api/submissions/${id}/student`, request);
  }

  updateAnswer(submissionId: string, questionId: string, answer: string) {
    return this.http.put<void>(`/api/submissions/${submissionId}/questions/${questionId}/answer`, { answer });
  }

  delete(id: string) {
    return this.http.delete<void>(`/api/submissions/${id}`);
  }
}
