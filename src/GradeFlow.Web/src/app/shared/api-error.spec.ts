import { HttpErrorResponse } from '@angular/common/http';
import { apiErrorMessage } from './api-error';

describe('apiErrorMessage', () => {
  const fallback = 'Não foi possível salvar.';

  it('preserves business validation messages', () => {
    expect(apiErrorMessage(new HttpErrorResponse({ error: { error: 'Ordem já está sendo usada.' }, status: 400 }), fallback))
      .toBe('Ordem já está sendo usada.');
  });

  it('shows ASP.NET field validation messages', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: { errors: { Text: ['Informe o enunciado.'], Points: ['Informe os pontos.'] } }
    });
    expect(apiErrorMessage(error, fallback)).toBe('Informe o enunciado. Informe os pontos.');
  });

  it('uses the friendly fallback for network failures and unexpected response bodies', () => {
    for (const body of [null, '<html>Bad Gateway</html>', { error: {} }]) {
      expect(apiErrorMessage(new HttpErrorResponse({ error: body, status: 502 }), fallback)).toBe(fallback);
    }
    expect(apiErrorMessage(new HttpErrorResponse({ status: 0 }), fallback)).toBe(fallback);
  });

  it('preserves ordinary error messages', () => {
    expect(apiErrorMessage(new Error('Falha local.'), fallback)).toBe('Falha local.');
  });

  it('explains forbidden and unavailable resources without exposing technical errors', () => {
    expect(apiErrorMessage(new HttpErrorResponse({ status: 403 }), fallback))
      .toBe('Você não tem permissão para acessar este recurso.');
    expect(apiErrorMessage(new HttpErrorResponse({ status: 404 }), fallback))
      .toBe('Recurso não encontrado ou você não tem permissão para acessá-lo.');
  });
});
