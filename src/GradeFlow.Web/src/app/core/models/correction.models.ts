export interface CorrectionResponse {
  submissionId: string;
  finalScore: number;
  maxScore: number;
  results: StudentAnswerCorrectionResponse[];
}

export interface StudentAnswerCorrectionResponse {
  questionId: string;
  answer: string;
  isCorrect: boolean;
  scoreAwarded: number;
  feedback?: string | null;
  needsReview: boolean;
}
