import { QuestionType } from '../core/models/question.models';

export function questionTitle(text: string) {
  return text.split('\n')[0];
}

export function buildQuestionText(type: QuestionType, text: string, optionA: string, optionB: string, optionC: string, optionD: string) {
  if (type !== QuestionType.MultipleChoice) return text;

  return [
    text,
    `A) ${optionA}`,
    `B) ${optionB}`,
    `C) ${optionC}`,
    `D) ${optionD}`
  ].filter((line) => !line.endsWith(') ')).join('\n');
}

export function parseQuestionText(type: QuestionType, text: string) {
  if (type !== QuestionType.MultipleChoice) return { text };

  const lines = text.split('\n');
  return {
    text: lines[0] ?? '',
    optionA: readOption(lines, 'A'),
    optionB: readOption(lines, 'B'),
    optionC: readOption(lines, 'C'),
    optionD: readOption(lines, 'D')
  };
}

export function multipleChoiceOptions(text: string) {
  return ['A', 'B', 'C', 'D']
    .map((option) => {
      const value = readOption(text.split('\n'), option);
      return value ? { value: option, text: value } : null;
    })
    .filter((option): option is { value: string; text: string } => option !== null);
}

function readOption(lines: string[], option: string) {
  return lines.find((line) => line.startsWith(`${option}) `))?.slice(3) ?? '';
}
