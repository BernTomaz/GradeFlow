import { Component, OnDestroy, inject } from '@angular/core';
import { NavigationCancel, NavigationEnd, NavigationError, NavigationStart, Router, RouterLink, RouterOutlet } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthApiService } from './core/api/auth-api.service';

@Component({
  selector: 'app-root',
  imports: [RouterLink, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnDestroy {
  private readonly router = inject(Router);
  protected readonly auth = inject(AuthApiService);
  private readonly events: Subscription;
  private loadingTimer: ReturnType<typeof setTimeout> | null = null;
  protected loading = false;

  constructor() {
    this.events = this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        if (this.loadingTimer) clearTimeout(this.loadingTimer);
        this.loading = true;
        return;
      }

      if (event instanceof NavigationEnd || event instanceof NavigationCancel || event instanceof NavigationError) {
        this.loadingTimer = setTimeout(() => (this.loading = false), 200);
      }
    });
  }

  ngOnDestroy() {
    this.events.unsubscribe();
    if (this.loadingTimer) clearTimeout(this.loadingTimer);
  }

  protected logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
