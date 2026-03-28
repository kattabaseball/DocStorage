import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { StatusBadgeComponent } from '../status-badge/status-badge.component';
import { TimeAgoPipe } from '../../pipes/time-ago.pipe';

export interface DocumentCardData {
  id: string;
  title: string;
  type: string;
  category: string;
  status: string;
  expiryDate?: string;
  uploadedDate: string;
  memberName?: string;
  fileSize?: number;
  thumbnailUrl?: string;
}

@Component({
  selector: 'td-document-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule, MatTooltipModule, StatusBadgeComponent, TimeAgoPipe],
  template: `
    <div class="document-card" [class]="'document-card--' + document.status.toLowerCase()">
      <div class="document-card__header">
        <div class="document-card__icon">
          <mat-icon>{{ getCategoryIcon(document.category) }}</mat-icon>
        </div>
        <div>
          <h4 class="document-card__title">{{ document.title }}</h4>
          <span class="document-card__type">{{ document.type }}</span>
        </div>
      </div>

      <div class="document-card__body">
        <td-status-badge [status]="document.status" type="document"></td-status-badge>
      </div>

      <div class="document-card__meta">
        @if (document.memberName) {
          <span class="document-card__meta-item">
            <mat-icon>person</mat-icon>
            {{ document.memberName }}
          </span>
        }
        <span class="document-card__meta-item">
          <mat-icon>schedule</mat-icon>
          {{ document.uploadedDate | timeAgo }}
        </span>
        @if (document.expiryDate) {
          <span class="document-card__meta-item"
                [class.document-card__expiry--warning]="isExpiringSoon(document.expiryDate)"
                [class.document-card__expiry--danger]="isExpired(document.expiryDate)">
            <mat-icon>event</mat-icon>
            {{ formatExpiry(document.expiryDate) }}
          </span>
        }
      </div>

      <div class="document-card__actions">
        <button mat-icon-button matTooltip="View" (click)="view.emit(document)">
          <mat-icon>visibility</mat-icon>
        </button>
        <button mat-icon-button matTooltip="Download" (click)="download.emit(document)">
          <mat-icon>download</mat-icon>
        </button>
        @if (document.status === 'Uploaded' || document.status === 'Reviewing') {
          <button mat-icon-button matTooltip="Verify" color="primary" (click)="verify.emit(document)">
            <mat-icon>verified</mat-icon>
          </button>
        }
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class DocumentCardComponent {
  @Input() document!: DocumentCardData;
  @Output() view = new EventEmitter<DocumentCardData>();
  @Output() download = new EventEmitter<DocumentCardData>();
  @Output() verify = new EventEmitter<DocumentCardData>();

  getCategoryIcon(category: string): string {
    const iconMap: Record<string, string> = {
      'passport': 'badge',
      'visa': 'flight',
      'medical': 'medical_services',
      'insurance': 'health_and_safety',
      'license': 'card_membership',
      'certificate': 'workspace_premium',
      'identification': 'fingerprint',
      'financial': 'account_balance',
      'travel': 'luggage',
      'legal': 'gavel'
    };
    return iconMap[category.toLowerCase()] || 'description';
  }

  isExpiringSoon(expiryDate: string): boolean {
    const expiry = new Date(expiryDate);
    const now = new Date();
    const daysUntilExpiry = Math.ceil((expiry.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
    return daysUntilExpiry > 0 && daysUntilExpiry <= 30;
  }

  isExpired(expiryDate: string): boolean {
    return new Date(expiryDate) < new Date();
  }

  formatExpiry(expiryDate: string): string {
    const expiry = new Date(expiryDate);
    const now = new Date();
    const daysUntilExpiry = Math.ceil((expiry.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));

    if (daysUntilExpiry < 0) {
      return `Expired ${Math.abs(daysUntilExpiry)} days ago`;
    }
    if (daysUntilExpiry === 0) {
      return 'Expires today';
    }
    if (daysUntilExpiry <= 30) {
      return `Expires in ${daysUntilExpiry} days`;
    }
    return `Expires ${expiry.toLocaleDateString()}`;
  }
}
