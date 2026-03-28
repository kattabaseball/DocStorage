import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppState } from '@store/app.state';
import { selectAllNotifications, selectNotificationsLoading } from '@store/notifications/notifications.selectors';
import { NotificationActions } from '@store/notifications/notifications.actions';
import { AppNotification } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { TimeAgoPipe } from '@shared/pipes/time-ago.pipe';

@Component({
  selector: 'td-notification-list',
  standalone: true,
  imports: [
    CommonModule, MatButtonModule, MatIconModule, MatCardModule,
    MatDividerModule, PageHeaderComponent, EmptyStateComponent, TimeAgoPipe
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Notifications"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Notifications'}]">
        <button mat-stroked-button (click)="markAllRead()">
          <mat-icon>done_all</mat-icon>
          Mark All as Read
        </button>
      </td-page-header>

      @if (notifications$ | async; as notifications) {
        @if (notifications.length === 0) {
          <td-empty-state icon="notifications_none" title="No notifications"
                          subtitle="You're all caught up! Notifications will appear here.">
          </td-empty-state>
        } @else {
          <div class="notification-list">
            @for (notif of notifications; track notif.id) {
              <mat-card class="notification-card" [class.notification-card--unread]="!notif.isRead"
                        (click)="markRead(notif)">
                <div class="notification-card__icon" [class]="'notification-card__icon--' + notif.type">
                  <mat-icon>{{ getIcon(notif.type) }}</mat-icon>
                </div>
                <div class="notification-card__content">
                  <h4 class="notification-card__title">{{ notif.title }}</h4>
                  <p class="notification-card__message">{{ notif.message }}</p>
                  <span class="notification-card__time">{{ notif.createdAt | timeAgo }}</span>
                </div>
                @if (!notif.isRead) {
                  <div class="notification-card__unread-dot"></div>
                }
              </mat-card>
            }
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .notification-list { display: flex; flex-direction: column; gap: 8px; }
    .notification-card {
      display: flex;
      align-items: flex-start;
      gap: 16px;
      padding: 16px 20px;
      cursor: pointer;
      transition: background 0.2s;
    }
    .notification-card:hover { background: #F5F5F5; }
    .notification-card--unread { background: rgba(21, 101, 192, 0.04); border-left: 3px solid #1565C0; }
    .notification-card__icon {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }
    .notification-card__icon mat-icon { color: #FFFFFF; font-size: 20px; width: 20px; height: 20px; }
    .notification-card__icon--info { background: #42A5F5; }
    .notification-card__icon--success { background: #66BB6A; }
    .notification-card__icon--warning { background: #FFA726; }
    .notification-card__icon--error { background: #EF5350; }
    .notification-card__content { flex: 1; }
    .notification-card__title { font-size: 14px; font-weight: 600; color: #263238; margin: 0 0 4px; }
    .notification-card__message { font-size: 13px; color: #546E7A; margin: 0 0 6px; }
    .notification-card__time { font-size: 12px; color: #90A4AE; }
    .notification-card__unread-dot {
      width: 8px; height: 8px;
      border-radius: 50%;
      background: #1565C0;
      flex-shrink: 0;
      margin-top: 6px;
    }
  `]
})
export class NotificationListComponent implements OnInit {
  private readonly store = inject(Store<AppState>);
  notifications$: Observable<AppNotification[]> = this.store.select(selectAllNotifications);

  ngOnInit(): void {
    this.store.dispatch(NotificationActions.loadNotifications());
  }

  getIcon(type: string): string {
    switch (type) {
      case 'success': return 'check_circle';
      case 'warning': return 'warning';
      case 'error': return 'error';
      default: return 'info';
    }
  }

  markRead(notif: AppNotification): void {
    if (!notif.isRead) {
      this.store.dispatch(NotificationActions.markAsRead({ id: notif.id }));
    }
  }

  markAllRead(): void {
    this.store.dispatch(NotificationActions.markAllAsRead());
  }
}
