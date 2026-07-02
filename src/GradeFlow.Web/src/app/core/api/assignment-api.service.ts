import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AssignmentResponse, SaveAssignmentRequest } from '../models/assignment.models';

@Injectable({ providedIn: 'root' })
export class AssignmentApiService {
  private readonly baseUrl = '/api/assignments';

  constructor(private readonly http: HttpClient) {}

  getAll() {
    return this.http.get<AssignmentResponse[]>(this.baseUrl);
  }

  getById(id: string) {
    return this.http.get<AssignmentResponse>(`${this.baseUrl}/${id}`);
  }

  create(request: SaveAssignmentRequest) {
    return this.http.post<AssignmentResponse>(this.baseUrl, request);
  }

  update(id: string, request: SaveAssignmentRequest) {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
