export enum SubmissionStatus {
  Pending = 0,
  Corrected = 1,
  Reviewed = 2
}

export interface CreateSubmissionRequest {
  studentName: string;
  studentEmail?: string | null;
  answers: CreateStudentAnswerRequest[];
}

export interface CreateStudentAnswerRequest {
  questionId: string;
  answer: string;
}

export interface SubmissionResponse {
  id: string;
  assignmentId: string;
  studentName: string;
  studentEmail?: string | null;
  status: SubmissionStatus;
  finalScore: number;
  submittedAt: string;
  answers: StudentAnswerResponse[];
}

export interface StudentAnswerResponse {
  id: string;
  questionId: string;
  answer: string;
  scoreAwarded: number;
  isCorrect: boolean;
  feedback?: string | null;
  needsReview: boolean;
}
