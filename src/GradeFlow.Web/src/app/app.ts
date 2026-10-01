import { Component, ElementRef, HostListener, OnDestroy, inject } from '@angular/core';
import { NavigationCancel, NavigationEnd, NavigationError, NavigationStart, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthApiService } from './core/api/auth-api.service';

@Component({
  selector: 'app-root',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnDestroy {
  private readonly element = inject(ElementRef<HTMLElement>);
  private readonly router = inject(Router);
  protected readonly auth = inject(AuthApiService);
  private readonly events: Subscription;
  private readonly mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
  private readonly mediaListener = () => this.applyTheme();
  private loadingTimer: ReturnType<typeof setTimeout> | null = null;
  private loadingDelay: ReturnType<typeof setTimeout> | null = null;
  private routeAnimationTimer: ReturnType<typeof setTimeout> | null = null;
  private assistantCloseTimer: ReturnType<typeof setTimeout> | null = null;
  protected loading = false;
  protected routeAnimating = false;
  protected theme = localStorage.getItem('gradeflow-theme') ?? 'system';
  protected sidebarCollapsed = localStorage.getItem('gradeflow-sidebar') === 'collapsed';
  protected settingsOpen = false;
  protected assistantOpen = localStorage.getItem('gradeflow-assistant-open') === 'true';
  protected assistantClosing = false;
  protected assistantQuestion = '';
  protected assistantAnswer = 'Oi, sou o Flow. Pergunte sobre avaliações, gabaritos, respostas, correção, revisão ou relatórios.';
  protected assistantSuggestions = [
    'Criar avaliação',
    'Montar gabarito',
    'Registrar respostas',
    'Corrigir respostas',
    'Revisar nota',
    'Gerar relatório'
  ];

  constructor() {
    this.applyTheme();
    this.mediaQuery.addEventListener('change', this.mediaListener);
    this.events = this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        if (this.loadingTimer) clearTimeout(this.loadingTimer);
        if (this.loadingDelay) clearTimeout(this.loadingDelay);
        this.loadingDelay = setTimeout(() => (this.loading = true), 250);
        return;
      }

      if (event instanceof NavigationEnd || event instanceof NavigationCancel || event instanceof NavigationError) {
        if (this.loadingDelay) clearTimeout(this.loadingDelay);
        this.loading = false;
        this.routeAnimating = false;
        requestAnimationFrame(() => {
          this.routeAnimating = true;
          if (this.routeAnimationTimer) clearTimeout(this.routeAnimationTimer);
          this.routeAnimationTimer = setTimeout(() => (this.routeAnimating = false), 220);
        });
      }
    });
  }

  ngOnDestroy() {
    this.events.unsubscribe();
    this.mediaQuery.removeEventListener('change', this.mediaListener);
    if (this.loadingTimer) clearTimeout(this.loadingTimer);
    if (this.loadingDelay) clearTimeout(this.loadingDelay);
    if (this.routeAnimationTimer) clearTimeout(this.routeAnimationTimer);
    if (this.assistantCloseTimer) clearTimeout(this.assistantCloseTimer);
  }

  protected setTheme(theme: string) {
    this.theme = theme;
    localStorage.setItem('gradeflow-theme', theme);
    this.applyTheme();
  }

  protected toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
    localStorage.setItem('gradeflow-sidebar', this.sidebarCollapsed ? 'collapsed' : 'open');
  }

  protected toggleSettings() {
    this.settingsOpen = !this.settingsOpen;
  }

  protected toggleAssistant() {
    if (!this.assistantOpen) {
      this.assistantOpen = true;
      this.assistantClosing = false;
      localStorage.setItem('gradeflow-assistant-open', 'true');
      return;
    }

    this.assistantClosing = true;
    if (this.assistantCloseTimer) clearTimeout(this.assistantCloseTimer);
    this.assistantCloseTimer = setTimeout(() => {
      this.assistantOpen = false;
      this.assistantClosing = false;
      localStorage.setItem('gradeflow-assistant-open', 'false');
    }, 180);
  }

  @HostListener('document:click', ['$event'])
  protected closeAssistantOnOutsideClick(event: MouseEvent) {
    if (!this.assistantOpen || this.assistantClosing) return;
    if (this.element.nativeElement.querySelector('.assistant-widget')?.contains(event.target as Node)) return;

    this.toggleAssistant();
  }

  protected updateAssistantQuestion(value: string) {
    this.assistantQuestion = value;
  }

  protected askAssistantSuggestion(question: string) {
    this.assistantQuestion = question;
  }

  protected askAssistant() {
    const question = this.normalize(this.assistantQuestion);

    if (!question) {
      this.assistantAnswer = 'Escreva uma pergunta curta sobre o fluxo do GradeFlow.';
      return;
    }

    if (question.includes('avaliacao') || question.includes('prova')) {
      this.assistantAnswer = 'Crie a avaliação em "Nova avaliação" e depois cadastre as questões e o gabarito dela.';
    } else if (question.includes('questao') || question.includes('gabarito')) {
      this.assistantAnswer = 'No detalhe da avaliação, adicione questões com tipo, enunciado, peso e resposta esperada.';
    } else if (question.includes('resposta') || question.includes('submissao') || question.includes('aluno')) {
      this.assistantAnswer = 'Abra a avaliação e registre uma submissão com as respostas do aluno para liberar a correção.';
    } else if (question.includes('corr')) {
      this.assistantAnswer = 'A correção automática compara as respostas com o gabarito usando as estratégias do motor de correção.';
    } else if (question.includes('revisao') || question.includes('nota')) {
      this.assistantAnswer = 'Use a tela de resultado para revisar itens pendentes ou ajustar manualmente quando necessário.';
    } else if (question.includes('relatorio') || question.includes('export')) {
      this.assistantAnswer = 'Os relatórios ficam no contexto da avaliação e mostram desempenho, notas e itens corrigidos.';
    } else {
      this.assistantAnswer = 'Por enquanto respondo sobre avaliações, gabaritos, respostas, correção, revisão e relatórios.';
    }
  }

  protected logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }

  private applyTheme() {
    const theme = this.theme === 'system'
      ? (this.mediaQuery.matches ? 'dark' : 'light')
      : this.theme;
    document.documentElement.dataset['theme'] = theme;
  }

  private normalize(value: string) {
    return value.trim().toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
  }

}
