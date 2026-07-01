export enum QuestionType {
  MultipleChoice = 0,
  TrueFalse = 1,
  Numeric = 2,
  ShortText = 3
}

export const questionTypeOptions = [
  { value: QuestionType.MultipleChoice, label: 'Múltipla escolha' },
  { value: QuestionType.TrueFalse, label: 'Verdadeiro ou falso' },
  { value: QuestionType.Numeric, label: 'Numérica' },
  { value: QuestionType.ShortText, label: 'Texto curto' }
];

export interface QuestionResponse {
  id: string;
  assignmentId: string;
  text: string;
  type: QuestionType;
  points: number;
  order: number;
  answerKey?: AnswerKeyResponse | null;
}

export interface AnswerKeyResponse {
  id: string;
  questionId: string;
  correctAnswer: string;
  acceptedAnswers?: string[] | null;
  keywords?: string[] | null;
  tolerance?: number | null;
  feedbackCorrect?: string | null;
  feedbackIncorrect?: string | null;
}

export interface CreateQuestionRequest {
  text: string;
  type: QuestionType;
  points: number;
  order: number;
  answerKey: {
    correctAnswer: string;
    acceptedAnswers?: string[] | null;
    keywords?: string[] | null;
    tolerance?: number | null;
    feedbackCorrect?: string | null;
    feedbackIncorrect?: string | null;
  };
}
