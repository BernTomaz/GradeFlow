import { LocalDatePipe } from './local-date.pipe';

describe('LocalDatePipe', () => {
  const pipe = new LocalDatePipe();

  it('interprets API timestamps without a timezone as UTC', () => {
    expect(pipe.transform('2026-11-02T12:30:00')).toBe('2026-11-02T12:30:00Z');
  });

  it('preserves timestamps that already specify their timezone', () => {
    for (const value of ['2026-11-02T12:30:00Z', '2026-11-02T12:30:00-03:00', '2026-11-02T12:30:00+03:00']) {
      expect(pipe.transform(value)).toBe(value);
    }
  });

  it('preserves empty values', () => {
    expect(pipe.transform(null)).toBeNull();
    expect(pipe.transform(undefined)).toBeUndefined();
    expect(pipe.transform('')).toBe('');
  });
});
