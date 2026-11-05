import { HttpErrorResponse } from '@angular/common/http';

export function apiErrorMessage(error: unknown, fallback: string): string {
  if (!error || typeof error !== 'object') return fallback;

  const body = 'error' in error ? error.error : null;
  if (body && typeof body === 'object') {
    if ('error' in body && typeof body.error === 'string' && body.error.trim()) {
      return body.error;
    }
    if ('errors' in body && body.errors && typeof body.errors === 'object') {
      const messages = Object.values(body.errors)
        .flatMap(value => Array.isArray(value) ? value : [])
        .filter((value): value is string => typeof value === 'string' && !!value.trim());
      if (messages.length) return messages.join(' ');
    }
  }

  if (error instanceof HttpErrorResponse) {
    if (error.status === 403) return 'Você não tem permissão para acessar este recurso.';
    if (error.status === 404) return 'Recurso não encontrado ou você não tem permissão para acessá-lo.';
    return fallback;
  }
  return 'message' in error && typeof error.message === 'string' && error.message.trim()
    ? error.message
    : fallback;
}
