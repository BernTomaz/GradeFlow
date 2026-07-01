export enum AssignmentStatus {
  Draft = 0,
  Open = 1,
  Closed = 2
}

export interface AssignmentResponse {
  id: string;
  title: string;
  description?: string | null;
  subject?: string | null;
  totalPoints: number;
  status: AssignmentStatus;
  createdAt: string;
  updatedAt?: string | null;
}

export interface SaveAssignmentRequest {
  title: string;
  description?: string | null;
  subject?: string | null;
}
