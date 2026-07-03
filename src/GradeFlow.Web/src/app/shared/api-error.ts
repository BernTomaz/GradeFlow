export function apiErrorMessage(error: { error?: { error?: string }; message?: string }, fallback: string) {
  return error.error?.error ?? error.message ?? fallback;
}
