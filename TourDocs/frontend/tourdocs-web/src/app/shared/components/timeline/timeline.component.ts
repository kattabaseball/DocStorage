import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { TimeAgoPipe } from '../../pipes/time-ago.pipe';

export interface TimelineStep {
  icon: string;
  title: string;
  subtitle?: string;
  date: string;
  status: 'completed' | 'active' | 'pending';
  color?: string;
}

@Component({
  selector: 'td-timeline',
  standalone: true,
  imports: [CommonModule, MatIconModule, TimeAgoPipe],
  template: `
    <div class="timeline">
      @for (step of steps; track step.title; let last = $last) {
        <div class="timeline__item" [class]="'timeline__item--' + step.status">
          <div class="timeline__marker">
            <div class="timeline__dot" [style.backgroundColor]="getColor(step)">
              <mat-icon class="timeline__dot-icon">{{ step.icon }}</mat-icon>
            </div>
            @if (!last) {
              <div class="timeline__line" [class.timeline__line--completed]="step.status === 'completed'"></div>
            }
          </div>
          <div class="timeline__content">
            <h4 class="timeline__title">{{ step.title }}</h4>
            @if (step.subtitle) {
              <p class="timeline__subtitle">{{ step.subtitle }}</p>
            }
            <span class="timeline__date">{{ step.date | timeAgo }}</span>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .timeline {
      display: flex;
      flex-direction: column;
    }

    .timeline__item {
      display: flex;
      gap: 16px;
      min-height: 72px;
    }

    .timeline__marker {
      display: flex;
      flex-direction: column;
      align-items: center;
      flex-shrink: 0;
    }

    .timeline__dot {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      z-index: 1;
    }

    .timeline__dot-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
      color: #FFFFFF;
    }

    .timeline__line {
      width: 2px;
      flex: 1;
      background: #E0E0E0;
      margin: 4px 0;

      &--completed {
        background: #66BB6A;
      }
    }

    .timeline__content {
      padding-bottom: 24px;
    }

    .timeline__title {
      font-size: 14px;
      font-weight: 600;
      color: #263238;
      margin: 0 0 4px;
      line-height: 36px;
    }

    .timeline__subtitle {
      font-size: 13px;
      color: #546E7A;
      margin: 0 0 4px;
    }

    .timeline__date {
      font-size: 12px;
      color: #90A4AE;
    }

    .timeline__item--pending {
      opacity: 0.5;

      .timeline__dot {
        background: #B0BEC5 !important;
      }
    }
  `]
})
export class TimelineComponent {
  @Input() steps: TimelineStep[] = [];

  getColor(step: TimelineStep): string {
    if (step.color) return step.color;
    switch (step.status) {
      case 'completed': return '#66BB6A';
      case 'active': return '#1565C0';
      case 'pending': return '#B0BEC5';
      default: return '#B0BEC5';
    }
  }
}
