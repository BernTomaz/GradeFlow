export function passwordStrength(password: string) {
  let score = 0;
  if (password.length >= 6) score++;
  if (password.length >= 10) score++;
  if (/[A-Z]/.test(password) && /[a-z]/.test(password)) score++;
  if (/\d/.test(password)) score++;
  if (/[^A-Za-z0-9]/.test(password)) score++;

  if (!password) return { label: 'Informe uma senha.', level: 'empty' };
  if (score <= 2) return { label: 'Senha fraca.', level: 'weak' };
  if (score <= 4) return { label: 'Senha média.', level: 'medium' };
  return { label: 'Senha forte.', level: 'strong' };
}
