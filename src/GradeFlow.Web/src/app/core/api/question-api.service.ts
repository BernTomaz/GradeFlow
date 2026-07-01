import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateQuestionRequest, QuestionResponse } from '../models/question.models';

@Injectable({ providedIn: 'root' })
export class QuestionApiService {
  constructor(private readonly http: HttpClient) {}

  getByAssignmentId(assignmentId: string) {
    return this.http.get<QuestionResponse[]>(`/api/assignments/${assignmentId}/questions`);
  }

  create(assignmentId: string, request: CreateQuestionRequest) {
    return this.http.post<QuestionResponse>(`/api/assignments/${assignmentId}/questions`, request);
  }
}
