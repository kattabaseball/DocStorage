import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'td-stat-card',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <div class="stat-card" [class]="'stat-card--' + color">
      <div class="stat-card__icon" [class]="'stat-card__icon--' + color">
        <mat-icon>{{ icon }}</mat-icon>
      </div>
      <div class="stat-card__content">
        <div class="stat-card__label">{{ label }}</div>
        <div class="stat-card__value">{{ value }}</div>
        @if (trend !== undefined && trend !== null) {
          <div class="stat-card__trend" [class]="'stat-card__trend--' + trendDirection">
            <mat-icon>{{ trendDirection === 'up' ? 'trending_up' : 'trending_down' }}</mat-icon>
            <span>{{ trend }}%</span>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`:host { display: block; }`]
})
export class StatCardComponent {
  @Input() icon = 'info';
  @Input() label = '';
  @Input() value: string | number = 0;
  @Input() trend: number | null = null;
  @Input() trendDirection: 'up' | 'down' = 'up';
  @Input() color: 'primary' | 'accent' | 'warn' | 'success' = 'primary';
}
