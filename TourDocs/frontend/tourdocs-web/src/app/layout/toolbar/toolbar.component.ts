import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { distinctUntilChanged } from 'rxjs/operators';
import { AppState } from '@store/app.state';
import { UiActions } from '@store/ui/ui.actions';
import { LoadingService } from '@core/services/loading.service';
import { UserMenuComponent } from './user-menu/user-menu.component';
import { NotificationBellComponent } from './notification-bell/notification-bell.component';
import { SearchBarComponent } from './search-bar/search-bar.component';

@Component({
  selector: 'td-toolbar',
  standalone: true,
  imports: [
    CommonModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
    UserMenuComponent,
    NotificationBellComponent,
    SearchBarComponent
  ],
  template: `
    <mat-toolbar class="td-toolbar">
      <button mat-icon-button (click)="toggleSidebar()">
        <mat-icon>menu</mat-icon>
      </button>

      <span class="spacer"></span>

      <td-search-bar></td-search-bar>
      <td-notification-bell></td-notification-bell>
      <td-user-menu></td-user-menu>
    </mat-toolbar>
    @if (loading$ | async) {
      <mat-progress-bar mode="indeterminate" class="td-toolbar__progress"></mat-progress-bar>
    }
  `,
  styles: [`
    .td-toolbar {
      background: #FFFFFF;
      color: #263238;
      border-bottom: 1px solid #E0E0E0;
      height: 64px;
      box-shadow: 0 1px 4px rgba(0, 0, 0, 0.04);
      z-index: 100;
      flex-shrink: 0;
    }

    .spacer {
      flex: 1 1 auto;
    }

    .td-toolbar__progress {
      position: absolute;
      top: 64px;
      left: 0;
      right: 0;
      z-index: 101;
    }
  `]
})
export class ToolbarComponent {
  private readonly store = inject(Store<AppState>);
  private readonly loadingService = inject(LoadingService);

  loading$: Observable<boolean> = this.loadingService.loading$.pipe(distinctUntilChanged());

  toggleSidebar(): void {
    this.store.dispatch(UiActions.toggleSidebar());
  }
}
