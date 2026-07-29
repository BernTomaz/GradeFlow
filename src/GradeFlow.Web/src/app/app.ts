import { Component, OnDestroy, inject } from '@angular/core';
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
  private readonly router = inject(Router);
  protected readonly auth = inject(AuthApiService);
  private readonly events: Subscription;
  private readonly mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
  private readonly mediaListener = () => this.applyTheme();
  private loadingTimer: ReturnType<typeof setTimeout> | null = null;
  private loadingDelay: ReturnType<typeof setTimeout> | null = null;
  private routeAnimationTimer: ReturnType<typeof setTimeout> | null = null;
  protected loading = false;
  protected routeAnimating = false;
  protected theme = localStorage.getItem('gradeflow-theme') ?? 'system';
  protected sidebarCollapsed = localStorage.getItem('gradeflow-sidebar') === 'collapsed';
  protected settingsOpen = false;

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

}
