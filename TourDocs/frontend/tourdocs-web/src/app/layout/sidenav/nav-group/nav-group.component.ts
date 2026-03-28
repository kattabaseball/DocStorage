import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { trigger, state, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'td-nav-group',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  animations: [
    trigger('expandCollapse', [
      state('collapsed', style({ height: '0', opacity: '0' })),
      state('expanded', style({ height: '*', opacity: '1' })),
      transition('collapsed <=> expanded', animate('200ms ease-in-out'))
    ])
  ],
  template: `
    <div class="td-nav-group">
      <button class="td-nav-group__header" (click)="toggle()">
        <mat-icon class="td-nav-group__icon">{{ icon }}</mat-icon>
        <span class="td-nav-group__label">{{ label }}</span>
        <mat-icon class="td-nav-group__chevron" [class.td-nav-group__chevron--expanded]="expanded">
          expand_more
        </mat-icon>
      </button>
      <div class="td-nav-group__children" [@expandCollapse]="expanded ? 'expanded' : 'collapsed'">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .td-nav-group__header {
      display: flex;
      align-items: center;
      gap: 12px;
      width: 100%;
      padding: 10px 20px;
      color: #B0BEC5;
      background: none;
      border: none;
      border-left: 3px solid transparent;
      cursor: pointer;
      font-size: 14px;
      font-family: inherit;
      transition: all 0.2s ease;
      text-align: left;

      &:hover {
        background: #37474F;
        color: #ECEFF1;
      }
    }

    .td-nav-group__icon {
      font-size: 20px;
      width: 20px;
      height: 20px;
      flex-shrink: 0;
    }

    .td-nav-group__label {
      flex: 1;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .td-nav-group__chevron {
      font-size: 18px;
      width: 18px;
      height: 18px;
      transition: transform 0.2s ease;

      &--expanded {
        transform: rotate(180deg);
      }
    }

    .td-nav-group__children {
      overflow: hidden;
    }
  `]
})
export class NavGroupComponent {
  @Input() icon = '';
  @Input() label = '';
  @Input() expanded = false;

  toggle(): void {
    this.expanded = !this.expanded;
  }
}
