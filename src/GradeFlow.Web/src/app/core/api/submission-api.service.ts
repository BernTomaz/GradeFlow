import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AssignmentReportResponse, CorrectionLogResponse, CreateSubmissionRequest, ImportSubmissionsResponse, ReviewStudentAnswerRequest, ReviewStudentAnswerResponse, SubmissionResponse } from '../models/submission.models';

@Injectable({ providedIn: 'root' })
export class SubmissionApiService {
  constructor(private readonly http: HttpClient) {}

  getByAssignmentId(assignmentId: string) {
    return this.http.get<SubmissionResponse[]>(`/api/assignments/${assignmentId}/submissions`);
  }

  getById(id: string) {
    return this.http.get<SubmissionResponse>(`/api/submissions/${id}`);
  }

  getCorrectionLogs(id: string) {
    return this.http.get<CorrectionLogResponse[]>(`/api/submissions/${id}/correction-logs`);
  }

  getReport(assignmentId: string) {
    return this.http.get<AssignmentReportResponse>(`/api/assignments/${assignmentId}/report`);
  }

  exportCsv(assignmentId: string) {
    return this.http.get(`/api/assignments/${assignmentId}/export/csv`, {
      responseType: 'blob'
    });
  }

  exportExcel(assignmentId: string) {
    return this.http.get(`/api/assignments/${assignmentId}/export/excel`, {
      responseType: 'blob'
    });
  }

  exportPdf(assignmentId: string) {
    return this.http.get(`/api/assignments/${assignmentId}/export/pdf`, {
      responseType: 'blob'
    });
  }

  create(assignmentId: string, request: CreateSubmissionRequest) {
    return this.http.post<SubmissionResponse>(`/api/assignments/${assignmentId}/submissions`, request);
  }

  importCsv(assignmentId: string, file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ImportSubmissionsResponse>(`/api/assignments/${assignmentId}/submissions/import`, formData);
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

  reviewAnswer(answerId: string, request: ReviewStudentAnswerRequest) {
    return this.http.put<ReviewStudentAnswerResponse>(`/api/student-answers/${answerId}/review`, request);
  }

  delete(id: string) {
    return this.http.delete<void>(`/api/submissions/${id}`);
  }
}
