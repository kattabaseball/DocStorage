import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';

interface ExpiringDoc {
  documentId: string;
  title: string;
  memberName: string;
  expiryDate: string;
  daysUntilExpiry: number;
}

@Component({
  selector: 'td-expiring-docs-widget',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatIconModule, MatButtonModule, RouterModule, StatusBadgeComponent],
  template: `
    <div class="widget-card">
      <div class="widget-card__header">
        <h3 class="widget-card__title">Expiring Documents</h3>
        <a mat-button color="primary" routerLink="/documents/expiry">View All</a>
      </div>
      <div class="widget-card__body" style="padding: 0;">
        @if (documents.length === 0) {
          <p class="text-muted text-center" style="padding: 32px;">No documents expiring soon.</p>
        } @else {
          <table mat-table [dataSource]="documents" class="expiry-table">
            <ng-container matColumnDef="memberName">
              <th mat-header-cell *matHeaderCellDef>Member</th>
              <td mat-cell *matCellDef="let doc">{{ doc.memberName }}</td>
            </ng-container>

            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Document</th>
              <td mat-cell *matCellDef="let doc">{{ doc.title }}</td>
            </ng-container>

            <ng-container matColumnDef="daysUntilExpiry">
              <th mat-header-cell *matHeaderCellDef>Expires In</th>
              <td mat-cell *matCellDef="let doc">
                <span [class]="getDaysClass(doc.daysUntilExpiry)">
                  {{ doc.daysUntilExpiry }} days
                </span>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        }
      </div>
    </div>
  `,
  styles: [`
    .expiry-table {
      width: 100%;
    }

    .days-danger {
      color: #EF5350;
      font-weight: 600;
    }

    .days-warning {
      color: #FFA726;
      font-weight: 600;
    }

    .days-ok {
      color: #66BB6A;
      font-weight: 500;
    }

    .text-muted {
      color: #90A4AE;
      font-size: 14px;
      text-align: center;
    }
  `]
})
export class ExpiringDocsWidgetComponent {
  @Input() documents: ExpiringDoc[] = [];

  displayedColumns = ['memberName', 'title', 'daysUntilExpiry'];

  getDaysClass(days: number): string {
    if (days <= 7) return 'days-danger';
    if (days <= 14) return 'days-warning';
    return 'days-ok';
  }
}
