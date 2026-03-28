import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDividerModule } from '@angular/material/divider';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppState } from '@store/app.state';
import { selectUnreadCount, selectRecentNotifications } from '@store/notifications/notifications.selectors';
import { NotificationActions } from '@store/notifications/notifications.actions';
import { AppNotification } from '@core/services/notification.service';
import { TimeAgoPipe } from '@shared/pipes/time-ago.pipe';

@Component({
  selector: 'td-notification-bell',
  standalone: true,
  imports: [CommonModule, RouterModule, MatMenuModule, MatIconModule, MatButtonModule, MatBadgeModule, MatDividerModule, TimeAgoPipe],
  template: `
    <button mat-icon-button [matMenuTriggerFor]="notifMenu"
            [matBadge]="(unreadCount$ | async) || null"
            [matBadgeHidden]="(unreadCount$ | async) === 0"
            matBadgeColor="warn"
            matBadgeSize="small">
      <mat-icon>notifications</mat-icon>
    </button>

    <mat-menu #notifMenu="matMenu" xPosition="before" class="notification-menu">
      <div class="notification-menu__header" (click)="$event.stopPropagation()">
        <span class="notification-menu__title">Notifications</span>
        <button mat-button color="primary" (click)="markAllRead()">Mark all read</button>
      </div>
      <mat-divider></mat-divider>

      @if (recentNotifications$ | async; as notifications) {
        @if (notifications.length === 0) {
          <div class="notification-menu__empty" (click)="$event.stopPropagation()">
            <mat-icon>notifications_none</mat-icon>
            <span>No notifications</span>
          </div>
        } @else {
          @for (notif of notifications; track notif.id) {
            <button mat-menu-item class="notification-item" [class.notification-item--unread]="!notif.isRead"
                    (click)="markRead(notif.id)">
              <mat-icon [class]="'notification-item__icon--' + notif.type">
                {{ getNotifIcon(notif.type) }}
              </mat-icon>
              <div class="notification-item__content">
                <span class="notification-item__title">{{ notif.title }}</span>
                <span class="notification-item__message">{{ notif.message }}</span>
                <span class="notification-item__time">{{ notif.createdAt | timeAgo }}</span>
              </div>
            </button>
          }
        }
      }

      <mat-divider></mat-divider>
      <button mat-menu-item routerLink="/notifications" class="notification-menu__view-all">
        <span>View all notifications</span>
      </button>
    </mat-menu>
  `,
  styles: [`
    .notification-menu__header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 12px 16px;
    }

    .notification-menu__title {
      font-size: 16px;
      font-weight: 600;
      color: #263238;
    }

    .notification-menu__empty {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 24px;
      color: #90A4AE;

      mat-icon {
        font-size: 36px;
        width: 36px;
        height: 36px;
        margin-bottom: 8px;
      }
    }

    .notification-item {
      height: auto !important;
      padding: 12px 16px !important;
      line-height: 1.4 !important;
      white-space: normal !important;

      &--unread {
        background: rgba(21, 101, 192, 0.04);
      }
    }

    .notification-item__content {
      display: flex;
      flex-direction: column;
      margin-left: 8px;
    }

    .notification-item__title {
      font-size: 13px;
      font-weight: 600;
      color: #263238;
    }

    .notification-item__message {
      font-size: 12px;
      color: #546E7A;
      max-width: 250px;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .notification-item__time {
      font-size: 11px;
      color: #90A4AE;
      margin-top: 2px;
    }

    .notification-item__icon--info { color: #1565C0; }
    .notification-item__icon--success { color: #66BB6A; }
    .notification-item__icon--warning { color: #FFA726; }
    .notification-item__icon--error { color: #EF5350; }

    .notification-menu__view-all {
      text-align: center;
      color: #1565C0;
      font-weight: 500;
    }
  `]
})
export class NotificationBellComponent {
  private readonly store = inject(Store<AppState>);
  unreadCount$: Observable<number> = this.store.select(selectUnreadCount);
  recentNotifications$: Observable<AppNotification[]> = this.store.select(selectRecentNotifications);

  getNotifIcon(type: string): string {
    switch (type) {
      case 'success': return 'check_circle';
      case 'warning': return 'warning';
      case 'error': return 'error';
      default: return 'info';
    }
  }

  markRead(id: string): void {
    this.store.dispatch(NotificationActions.markAsRead({ id }));
  }

  markAllRead(): void {
    this.store.dispatch(NotificationActions.markAllAsRead());
  }
}
