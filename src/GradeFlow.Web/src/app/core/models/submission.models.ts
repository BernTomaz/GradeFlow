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

export interface ReviewStudentAnswerRequest {
  scoreAwarded: number;
  feedback?: string | null;
  isCorrect: boolean;
}

export interface ReviewStudentAnswerResponse {
  answerId: string;
  submissionId: string;
  scoreAwarded: number;
  feedback?: string | null;
  isCorrect: boolean;
  needsReview: boolean;
  finalScore: number;
}

export interface CorrectionLogResponse {
  id: string;
  submissionId: string;
  questionId: string;
  correctionType: string;
  originalAnswer: string;
  expectedAnswer?: string | null;
  score: number;
  message?: string | null;
  createdAt: string;
}
