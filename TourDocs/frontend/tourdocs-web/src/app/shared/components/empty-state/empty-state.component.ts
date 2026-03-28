import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'td-empty-state',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule, MatButtonModule],
  template: `
    <div class="empty-state">
      <mat-icon class="empty-state__icon">{{ icon }}</mat-icon>
      <h3 class="empty-state__title">{{ title }}</h3>
      @if (subtitle) {
        <p class="empty-state__subtitle">{{ subtitle }}</p>
      }
      @if (actionText) {
        @if (actionLink) {
          <a mat-flat-button color="primary" [routerLink]="actionLink" class="empty-state__action">
            {{ actionText }}
          </a>
        } @else {
          <button mat-flat-button color="primary" class="empty-state__action" (click)="onActionClick()">
            {{ actionText }}
          </button>
        }
      }
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 64px 24px;
      text-align: center;
    }

    .empty-state__icon {
      font-size: 72px;
      width: 72px;
      height: 72px;
      color: #B0BEC5;
      margin-bottom: 24px;
    }

    .empty-state__title {
      font-size: 20px;
      font-weight: 600;
      color: #37474F;
      margin: 0 0 8px;
    }

    .empty-state__subtitle {
      font-size: 14px;
      color: #78909C;
      margin: 0 0 24px;
      max-width: 400px;
    }
  `]
})
export class EmptyStateComponent {
  @Input() icon = 'inbox';
  @Input() title = 'No items found';
  @Input() subtitle = '';
  @Input() actionText = '';
  @Input() actionLink = '';

  onActionClick(): void {
    // Override in parent via event binding if needed
  }
}
