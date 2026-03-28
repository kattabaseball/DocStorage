import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';

@Component({
  selector: 'td-nav-item',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule, MatBadgeModule],
  template: `
    <a [routerLink]="link"
       routerLinkActive="td-nav-item--active"
       class="td-nav-item"
       [class.td-nav-item--child]="isChild">
      <mat-icon class="td-nav-item__icon">{{ icon }}</mat-icon>
      <span class="td-nav-item__label">{{ label }}</span>
      @if (badgeCount && badgeCount > 0) {
        <span class="td-nav-item__badge">{{ badgeCount > 99 ? '99+' : badgeCount }}</span>
      }
    </a>
  `,
  styles: [`
    .td-nav-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 10px 20px;
      color: #B0BEC5;
      text-decoration: none;
      transition: all 0.2s ease;
      border-left: 3px solid transparent;
      cursor: pointer;
      font-size: 14px;
      font-weight: 400;

      &:hover {
        background: #37474F;
        color: #ECEFF1;
      }

      &--active {
        background: rgba(21, 101, 192, 0.15);
        color: #FFFFFF;
        border-left-color: #1565C0;
        font-weight: 500;

        .td-nav-item__icon {
          color: #42A5F5;
        }
      }

      &--child {
        padding-left: 56px;
        font-size: 13px;
      }
    }

    .td-nav-item__icon {
      font-size: 20px;
      width: 20px;
      height: 20px;
      flex-shrink: 0;
    }

    .td-nav-item__label {
      flex: 1;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .td-nav-item__badge {
      background: #EF5350;
      color: #FFFFFF;
      font-size: 11px;
      font-weight: 600;
      padding: 2px 8px;
      border-radius: 12px;
      min-width: 20px;
      text-align: center;
    }
  `]
})
export class NavItemComponent {
  @Input() icon = '';
  @Input() label = '';
  @Input() link = '';
  @Input() badgeCount: number | null = null;
  @Input() isChild = false;
}
