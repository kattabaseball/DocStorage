import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { DocumentsService } from '../documents.service';
import { Document } from '../documents.models';

@Component({
  selector: 'td-expiry-tracker',
  standalone: true,
  imports: [
    CommonModule, MatTableModule, MatSortModule, MatIconModule,
    MatButtonModule, MatSelectModule, MatFormFieldModule, MatProgressSpinnerModule,
    PageHeaderComponent, StatusBadgeComponent, AvatarComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Expiry Tracker"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Documents', link: '/documents'}, {label: 'Expiry Tracker'}]">
      </td-page-header>

      <div class="tracker-controls">
        <mat-form-field appearance="outline" class="filter-select">
          <mat-label>Time Range</mat-label>
          <mat-select [(value)]="daysAhead" (selectionChange)="loadDocuments()">
            <mat-option [value]="7">Next 7 days</mat-option>
            <mat-option [value]="14">Next 14 days</mat-option>
            <mat-option [value]="30">Next 30 days</mat-option>
            <mat-option [value]="60">Next 60 days</mat-option>
            <mat-option [value]="90">Next 90 days</mat-option>
          </mat-select>
        </mat-form-field>
      </div>

      <div class="td-table-container">
        @if (loading) {
          <div class="table-loading-shade">
            <mat-spinner diameter="40"></mat-spinner>
          </div>
        }

        @if (!loading && documents.length === 0) {
          <td-empty-state icon="event_available"
                          title="No expiring documents"
                          subtitle="No documents are expiring within the selected time range">
          </td-empty-state>
        } @else {
          <table mat-table [dataSource]="documents" matSort (matSortChange)="onSort($event)">
            <ng-container matColumnDef="memberName">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Member</th>
              <td mat-cell *matCellDef="let doc">
                <div class="flex-row gap-8">
                  <td-avatar [name]="doc.memberName" size="sm"></td-avatar>
                  <span>{{ doc.memberName }}</span>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Document</th>
              <td mat-cell *matCellDef="let doc">{{ doc.title }}</td>
            </ng-container>

            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let doc">{{ doc.type }}</td>
            </ng-container>

            <ng-container matColumnDef="expiryDate">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Expiry Date</th>
              <td mat-cell *matCellDef="let doc">{{ doc.expiryDate | date:'mediumDate' }}</td>
            </ng-container>

            <ng-container matColumnDef="daysLeft">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Days Left</th>
              <td mat-cell *matCellDef="let doc">
                <span [class]="getDaysClass(getDaysLeft(doc.expiryDate))">
                  {{ getDaysLeft(doc.expiryDate) }} days
                </span>
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let doc">
                <td-status-badge [status]="doc.status" type="document"></td-status-badge>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"
                [class.row-danger]="getDaysLeft(row.expiryDate) <= 7"
                [class.row-warning]="getDaysLeft(row.expiryDate) > 7 && getDaysLeft(row.expiryDate) <= 14"></tr>
          </table>
        }
      </div>
    </div>
  `,
  styles: [`
    .tracker-controls {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
    }
    .filter-select {
      width: 200px;
      ::ng-deep .mat-mdc-form-field-subscript-wrapper { display: none; }
    }
    table { width: 100%; }
    .row-danger { background: rgba(239, 83, 80, 0.04); }
    .row-warning { background: rgba(255, 167, 38, 0.04); }
    .days-danger { color: #EF5350; font-weight: 600; }
    .days-warning { color: #FFA726; font-weight: 600; }
    .days-ok { color: #66BB6A; font-weight: 500; }
  `]
})
export class ExpiryTrackerComponent implements OnInit {
  private readonly documentsService = inject(DocumentsService);

  documents: Document[] = [];
  loading = false;
  daysAhead = 30;
  displayedColumns = ['memberName', 'title', 'type', 'expiryDate', 'daysLeft', 'status'];

  ngOnInit(): void {
    this.loadDocuments();
  }

  loadDocuments(): void {
    this.loading = true;
    this.documentsService.getExpiringDocuments(this.daysAhead).subscribe({
      next: (response) => {
        if (response.success) {
          this.documents = response.data;
        }
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  getDaysLeft(expiryDate: string | null): number {
    if (!expiryDate) return 999;
    const expiry = new Date(expiryDate);
    const now = new Date();
    return Math.ceil((expiry.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
  }

  getDaysClass(days: number): string {
    if (days <= 7) return 'days-danger';
    if (days <= 14) return 'days-warning';
    return 'days-ok';
  }

  onSort(sort: Sort): void {
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.documents = [...this.documents].sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'expiryDate':
        case 'daysLeft':
          return this.compare(
            new Date(a.expiryDate || '').getTime(),
            new Date(b.expiryDate || '').getTime(),
            isAsc
          );
        case 'memberName': return this.compare(a.memberName, b.memberName, isAsc);
        case 'title': return this.compare(a.title, b.title, isAsc);
        default: return 0;
      }
    });
  }

  private compare(a: string | number, b: string | number, isAsc: boolean): number {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }
}
