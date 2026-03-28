import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

export interface Breadcrumb {
  label: string;
  link?: string;
  icon?: string;
}

@Component({
  selector: 'td-page-header',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule],
  template: `
    <div class="page-header">
      <div class="page-header__left">
        <nav class="page-header__breadcrumbs" aria-label="Breadcrumb">
          @for (crumb of breadcrumbs; track crumb.label; let last = $last) {
            @if (!last) {
              <a [routerLink]="crumb.link" class="page-header__breadcrumb-item">
                @if (crumb.icon) {
                  <mat-icon class="page-header__breadcrumb-icon">{{ crumb.icon }}</mat-icon>
                }
                <span>{{ crumb.label }}</span>
              </a>
              <mat-icon class="page-header__breadcrumb-separator">chevron_right</mat-icon>
            } @else {
              <span class="page-header__breadcrumb-item page-header__breadcrumb-item--active">
                @if (crumb.icon) {
                  <mat-icon class="page-header__breadcrumb-icon">{{ crumb.icon }}</mat-icon>
                }
                <span>{{ crumb.label }}</span>
              </span>
            }
          }
        </nav>
        <h1 class="page-header__title">{{ title }}</h1>
      </div>
      <div class="page-header__actions">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      margin-bottom: 24px;
      flex-wrap: wrap;
      gap: 16px;
    }

    .page-header__breadcrumbs {
      display: flex;
      align-items: center;
      gap: 4px;
      margin-bottom: 8px;
    }

    .page-header__breadcrumb-item {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 13px;
      color: #78909C;
      text-decoration: none;
      transition: color 0.2s;

      &:hover {
        color: #1565C0;
      }

      &--active {
        color: #37474F;
        font-weight: 500;
      }
    }

    .page-header__breadcrumb-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
    }

    .page-header__breadcrumb-separator {
      font-size: 16px;
      width: 16px;
      height: 16px;
      color: #B0BEC5;
    }

    .page-header__title {
      font-size: 24px;
      font-weight: 700;
      color: #263238;
      margin: 0;
    }

    .page-header__actions {
      display: flex;
      align-items: center;
      gap: 8px;
    }
  `]
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() breadcrumbs: Breadcrumb[] = [];
}
