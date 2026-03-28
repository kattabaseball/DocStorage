import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { TimeAgoPipe } from '@shared/pipes/time-ago.pipe';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';

interface RecentActivity {
  id: string;
  action: string;
  entityType: string;
  entityId: string;
  userName: string;
  details: string;
  createdAt: string;
}

@Component({
  selector: 'td-recent-activity-widget',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule, RouterModule, TimeAgoPipe, AvatarComponent],
  template: `
    <div class="widget-card">
      <div class="widget-card__header">
        <h3 class="widget-card__title">Recent Activity</h3>
        <a mat-button color="primary" routerLink="/audit">View All</a>
      </div>
      <div class="widget-card__body activity-list">
        @if (activities.length === 0) {
          <p class="text-muted text-center">No recent activity.</p>
        } @else {
          @for (activity of activities; track activity.id) {
            <div class="activity-item">
              <div class="activity-item__icon" [style.backgroundColor]="getIconColor(activity.action)">
                <mat-icon>{{ getIcon(activity.action) }}</mat-icon>
              </div>
              <div class="activity-item__content">
                <span class="activity-item__text">
                  <strong>{{ activity.userName || 'System' }}</strong> {{ activity.action }}
                  @if (activity.details) {
                    <span> — {{ activity.details }}</span>
                  }
                </span>
                <span class="activity-item__time">{{ activity.createdAt | timeAgo }}</span>
              </div>
            </div>
          }
        }
      </div>
    </div>
  `,
  styles: [`
    .activity-list {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .activity-item {
      display: flex;
      align-items: flex-start;
      gap: 12px;
    }

    .activity-item__icon {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;

      mat-icon {
        font-size: 18px;
        width: 18px;
        height: 18px;
        color: #FFFFFF;
      }
    }

    .activity-item__content {
      display: flex;
      flex-direction: column;
    }

    .activity-item__text {
      font-size: 13px;
      color: #37474F;
      line-height: 1.4;

      strong {
        font-weight: 600;
      }
    }

    .activity-item__time {
      font-size: 12px;
      color: #90A4AE;
      margin-top: 2px;
    }

    .text-muted {
      color: #90A4AE;
      font-size: 14px;
    }

    .text-center {
      text-align: center;
      padding-top: 32px;
    }
  `]
})
export class RecentActivityWidgetComponent {
  @Input() activities: RecentActivity[] = [];

  getIcon(action: string): string {
    const lower = action.toLowerCase();
    if (lower.includes('upload')) return 'upload';
    if (lower.includes('verif')) return 'verified';
    if (lower.includes('reject')) return 'cancel';
    if (lower.includes('creat')) return 'add_circle';
    if (lower.includes('updat') || lower.includes('settings')) return 'edit';
    if (lower.includes('delet') || lower.includes('remov')) return 'delete';
    if (lower.includes('login')) return 'login';
    if (lower.includes('expir')) return 'warning';
    return 'info';
  }

  getIconColor(action: string): string {
    const lower = action.toLowerCase();
    if (lower.includes('upload')) return '#42A5F5';
    if (lower.includes('verif')) return '#66BB6A';
    if (lower.includes('reject')) return '#EF5350';
    if (lower.includes('creat')) return '#7B1FA2';
    if (lower.includes('updat') || lower.includes('settings')) return '#546E7A';
    if (lower.includes('delet') || lower.includes('remov')) return '#EF5350';
    if (lower.includes('login')) return '#42A5F5';
    if (lower.includes('expir')) return '#FFA726';
    return '#90A4AE';
  }
}
