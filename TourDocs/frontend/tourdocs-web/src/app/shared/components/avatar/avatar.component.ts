import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'td-avatar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="avatar" [class]="'avatar--' + size" [style.backgroundColor]="bgColor">
      @if (imageUrl) {
        <img [src]="imageUrl" [alt]="name" class="avatar__image" (error)="onImageError()">
      } @else {
        <span class="avatar__initials">{{ initials }}</span>
      }
    </div>
  `,
  styles: [`
    .avatar {
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      overflow: hidden;
      flex-shrink: 0;

      &--sm {
        width: 32px;
        height: 32px;
        font-size: 12px;
      }

      &--md {
        width: 40px;
        height: 40px;
        font-size: 14px;
      }

      &--lg {
        width: 56px;
        height: 56px;
        font-size: 20px;
      }
    }

    .avatar__image {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .avatar__initials {
      color: #FFFFFF;
      font-weight: 600;
      text-transform: uppercase;
      user-select: none;
    }
  `]
})
export class AvatarComponent implements OnChanges {
  @Input() imageUrl: string | null = null;
  @Input() name = '';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';

  initials = '';
  bgColor = '#1565C0';
  private showImage = true;

  ngOnChanges(): void {
    this.initials = this.getInitials(this.name);
    this.bgColor = this.getColorFromName(this.name);
    this.showImage = !!this.imageUrl;
  }

  onImageError(): void {
    this.imageUrl = null;
  }

  private getInitials(name: string): string {
    if (!name) return '?';
    const parts = name.trim().split(/\s+/);
    if (parts.length === 1) {
      return parts[0].charAt(0).toUpperCase();
    }
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  }

  private getColorFromName(name: string): string {
    const colors = [
      '#1565C0', '#00897B', '#7B1FA2', '#C62828',
      '#EF6C00', '#2E7D32', '#AD1457', '#0277BD',
      '#4527A0', '#00695C', '#BF360C', '#1B5E20'
    ];
    if (!name) return colors[0];
    let hash = 0;
    for (let i = 0; i < name.length; i++) {
      hash = name.charCodeAt(i) + ((hash << 5) - hash);
    }
    return colors[Math.abs(hash) % colors.length];
  }
}
