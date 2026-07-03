import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CorrectionResponse } from '../models/correction.models';

@Injectable({ providedIn: 'root' })
export class CorrectionApiService {
  constructor(private readonly http: HttpClient) {}

  correct(submissionId: string) {
    return this.http.post<CorrectionResponse>(`/api/submissions/${submissionId}/correct`, {});
  }

  correctQuestion(submissionId: string, questionId: string) {
    return this.http.post<CorrectionResponse>(`/api/submissions/${submissionId}/questions/${questionId}/correct`, {});
  }
}
