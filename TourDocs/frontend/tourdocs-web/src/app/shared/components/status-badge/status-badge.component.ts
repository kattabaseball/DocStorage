import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';

type StatusType = 'document' | 'case' | 'hardcopy' | 'request';

interface StatusConfig {
  label: string;
  color: string;
  backgroundColor: string;
}

const STATUS_MAP: Record<StatusType, Record<string, StatusConfig>> = {
  document: {
    Uploaded: { label: 'Uploaded', color: '#1565C0', backgroundColor: 'rgba(66, 165, 245, 0.12)' },
    Reviewing: { label: 'Reviewing', color: '#E65100', backgroundColor: 'rgba(255, 167, 38, 0.12)' },
    Verified: { label: 'Verified', color: '#2E7D32', backgroundColor: 'rgba(102, 187, 106, 0.12)' },
    Rejected: { label: 'Rejected', color: '#C62828', backgroundColor: 'rgba(239, 83, 80, 0.12)' },
    Expired: { label: 'Expired', color: '#616161', backgroundColor: 'rgba(189, 189, 189, 0.12)' },
    PendingUpload: { label: 'Pending', color: '#78909C', backgroundColor: 'rgba(120, 144, 156, 0.12)' },
    ReUploadRequired: { label: 'Re-upload', color: '#E65100', backgroundColor: 'rgba(255, 167, 38, 0.12)' },
  },
  case: {
    Draft: { label: 'Draft', color: '#78909C', backgroundColor: 'rgba(120, 144, 156, 0.12)' },
    Active: { label: 'Active', color: '#1565C0', backgroundColor: 'rgba(66, 165, 245, 0.12)' },
    Ready: { label: 'Ready', color: '#2E7D32', backgroundColor: 'rgba(102, 187, 106, 0.12)' },
    InProgress: { label: 'In Progress', color: '#E65100', backgroundColor: 'rgba(255, 167, 38, 0.12)' },
    Completed: { label: 'Completed', color: '#2E7D32', backgroundColor: 'rgba(102, 187, 106, 0.12)' },
    Cancelled: { label: 'Cancelled', color: '#616161', backgroundColor: 'rgba(189, 189, 189, 0.12)' },
  },
  hardcopy: {
    WithMember: { label: 'With Member', color: '#1565C0', backgroundColor: 'rgba(144, 202, 249, 0.2)' },
    Collected: { label: 'Collected', color: '#F57F17', backgroundColor: 'rgba(255, 224, 130, 0.2)' },
    WithHandler: { label: 'With Handler', color: '#7B1FA2', backgroundColor: 'rgba(206, 147, 216, 0.2)' },
    AtAuthority: { label: 'At Authority', color: '#C62828', backgroundColor: 'rgba(239, 83, 80, 0.12)' },
    Returned: { label: 'Returned', color: '#2E7D32', backgroundColor: 'rgba(165, 214, 167, 0.2)' },
  },
  request: {
    Pending: { label: 'Pending', color: '#E65100', backgroundColor: 'rgba(255, 167, 38, 0.12)' },
    Approved: { label: 'Approved', color: '#2E7D32', backgroundColor: 'rgba(102, 187, 106, 0.12)' },
    Denied: { label: 'Denied', color: '#C62828', backgroundColor: 'rgba(239, 83, 80, 0.12)' },
    Fulfilled: { label: 'Fulfilled', color: '#1565C0', backgroundColor: 'rgba(66, 165, 245, 0.12)' },
  }
};

@Component({
  selector: 'td-status-badge',
  standalone: true,
  imports: [CommonModule, MatChipsModule],
  template: `
    <span class="status-badge"
          [style.color]="config.color"
          [style.backgroundColor]="config.backgroundColor">
      <span class="status-badge__dot" [style.backgroundColor]="config.color"></span>
      {{ config.label }}
    </span>
  `,
  styles: [`
    .status-badge {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 4px 12px;
      border-radius: 16px;
      font-size: 12px;
      font-weight: 600;
      letter-spacing: 0.3px;
      white-space: nowrap;
    }

    .status-badge__dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      flex-shrink: 0;
    }
  `]
})
export class StatusBadgeComponent implements OnChanges {
  @Input() status = '';
  @Input() type: StatusType = 'document';

  config: StatusConfig = { label: '', color: '#78909C', backgroundColor: 'rgba(120, 144, 156, 0.12)' };

  ngOnChanges(): void {
    const typeMap = STATUS_MAP[this.type];
    if (typeMap && typeMap[this.status]) {
      this.config = typeMap[this.status];
    } else {
      this.config = { label: this.status, color: '#78909C', backgroundColor: 'rgba(120, 144, 156, 0.12)' };
    }
  }
}
