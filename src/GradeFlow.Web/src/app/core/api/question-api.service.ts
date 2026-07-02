import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateQuestionRequest, QuestionResponse } from '../models/question.models';

@Injectable({ providedIn: 'root' })
export class QuestionApiService {
  constructor(private readonly http: HttpClient) {}

  getByAssignmentId(assignmentId: string) {
    return this.http.get<QuestionResponse[]>(`/api/assignments/${assignmentId}/questions`);
  }

  getById(id: string) {
    return this.http.get<QuestionResponse>(`/api/questions/${id}`);
  }

  create(assignmentId: string, request: CreateQuestionRequest) {
    return this.http.post<QuestionResponse>(`/api/assignments/${assignmentId}/questions`, request);
  }

  update(id: string, request: CreateQuestionRequest) {
    return this.http.put<void>(`/api/questions/${id}`, request);
  }

  delete(id: string) {
    return this.http.delete<void>(`/api/questions/${id}`);
  }
}
