import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Store } from '@ngrx/store';
import { Observable, Subject, takeUntil } from 'rxjs';
import { AppState } from '@store/app.state';
import { selectSidebarOpen, selectTheme } from '@store/ui/ui.selectors';
import { UiActions } from '@store/ui/ui.actions';
import { SidenavComponent } from './sidenav/sidenav.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { FooterComponent } from './footer/footer.component';

@Component({
  selector: 'td-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    SidenavComponent,
    ToolbarComponent,
    FooterComponent
  ],
  template: `
    <div class="td-layout" [class.dark-theme]="(theme$ | async) === 'dark'">
      <!-- Sidebar -->
      @if (sidebarOpen$ | async) {
        <aside class="td-sidebar">
          <td-sidenav></td-sidenav>
        </aside>
      }

      <!-- Mobile overlay backdrop -->
      @if (isMobile && (sidebarOpen$ | async)) {
        <div class="td-backdrop" (click)="closeSidebar()"></div>
      }

      <!-- Main content area -->
      <div class="td-content">
        <td-toolbar></td-toolbar>
        <main class="td-main">
          <router-outlet></router-outlet>
        </main>
        <td-footer></td-footer>
      </div>
    </div>
  `,
  styles: [`
    .td-layout {
      display: flex;
      height: 100vh;
      overflow: hidden;
    }

    .td-sidebar {
      width: 260px;
      flex-shrink: 0;
      height: 100vh;
      overflow-y: auto;
      background: #263238;
      box-shadow: 2px 0 8px rgba(0, 0, 0, 0.1);
      z-index: 50;
    }

    .td-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.4);
      z-index: 40;
    }

    .td-content {
      flex: 1;
      display: flex;
      flex-direction: column;
      min-width: 0;
      height: 100vh;
    }

    .td-main {
      flex: 1;
      overflow-y: auto;
      background: #FAFAFA;
    }

    @media (max-width: 768px) {
      .td-sidebar {
        position: fixed;
        left: 0;
        top: 0;
        bottom: 0;
        z-index: 50;
      }
    }
  `]
})
export class LayoutComponent implements OnInit, OnDestroy {
  private readonly store = inject(Store<AppState>);
  private readonly breakpointObserver = inject(BreakpointObserver);
  private destroy$ = new Subject<void>();

  sidebarOpen$: Observable<boolean> = this.store.select(selectSidebarOpen);
  theme$: Observable<string> = this.store.select(selectTheme);
  isMobile = false;

  ngOnInit(): void {
    this.breakpointObserver
      .observe([Breakpoints.Handset, Breakpoints.TabletPortrait])
      .pipe(takeUntil(this.destroy$))
      .subscribe(result => {
        this.isMobile = result.matches;
        if (this.isMobile) {
          this.store.dispatch(UiActions.setSidebarOpen({ open: false }));
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  closeSidebar(): void {
    this.store.dispatch(UiActions.setSidebarOpen({ open: false }));
  }
}
