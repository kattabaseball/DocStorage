import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { TimeAgoPipe } from '@shared/pipes/time-ago.pipe';
import { ApiService } from '@core/services/api.service';
import { QueryParams, DEFAULT_PAGINATION } from '@core/models/pagination.model';

interface AuditEntry {
  id: string;
  action: string;
  entityType: string;
  entityId: string;
  entityName: string;
  description: string;
  performedBy: string;
  performedByAvatar: string | null;
  ipAddress: string;
  timestamp: string;
}

@Component({
  selector: 'td-audit-log-list',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatTableModule, MatPaginatorModule, MatSortModule,
    MatFormFieldModule, MatInputModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatChipsModule,
    PageHeaderComponent, AvatarComponent, EmptyStateComponent, TimeAgoPipe
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Audit Log"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Audit Log'}]">
      </td-page-header>

      <div class="filters-bar" [formGroup]="filterForm">
        <mat-form-field appearance="outline" class="filter-field">
          <mat-label>Action Type</mat-label>
          <mat-select formControlName="actionType" (selectionChange)="applyFilters()">
            <mat-option value="">All Actions</mat-option>
            <mat-option value="Create">Create</mat-option>
            <mat-option value="Update">Update</mat-option>
            <mat-option value="Delete">Delete</mat-option>
            <mat-option value="Upload">Upload</mat-option>
            <mat-option value="Verify">Verify</mat-option>
            <mat-option value="Reject">Reject</mat-option>
            <mat-option value="Login">Login</mat-option>
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline" class="filter-field">
          <mat-label>Entity Type</mat-label>
          <mat-select formControlName="entityType" (selectionChange)="applyFilters()">
            <mat-option value="">All Entities</mat-option>
            <mat-option value="Document">Document</mat-option>
            <mat-option value="Member">Member</mat-option>
            <mat-option value="Case">Case</mat-option>
            <mat-option value="HardCopy">Hard Copy</mat-option>
            <mat-option value="Checklist">Checklist</mat-option>
            <mat-option value="User">User</mat-option>
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline" class="filter-field">
          <mat-label>From Date</mat-label>
          <input matInput [matDatepicker]="fromPicker" formControlName="fromDate" (dateChange)="applyFilters()">
          <mat-datepicker-toggle matIconSuffix [for]="fromPicker"></mat-datepicker-toggle>
          <mat-datepicker #fromPicker></mat-datepicker>
        </mat-form-field>
        <mat-form-field appearance="outline" class="filter-field">
          <mat-label>To Date</mat-label>
          <input matInput [matDatepicker]="toPicker" formControlName="toDate" (dateChange)="applyFilters()">
          <mat-datepicker-toggle matIconSuffix [for]="toPicker"></mat-datepicker-toggle>
          <mat-datepicker #toPicker></mat-datepicker>
        </mat-form-field>
        <button mat-stroked-button (click)="resetFilters()">
          <mat-icon>clear</mat-icon> Clear
        </button>
      </div>

      <div class="td-table-container">
        @if (loading) {
          <div class="table-loading-shade"><mat-spinner diameter="40"></mat-spinner></div>
        }

        @if (!loading && auditEntries.length === 0) {
          <td-empty-state icon="history" title="No audit entries" subtitle="Activity will be logged here">
          </td-empty-state>
        } @else {
          <table mat-table [dataSource]="auditEntries" matSort (matSortChange)="onSort($event)">
            <ng-container matColumnDef="timestamp">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Time</th>
              <td mat-cell *matCellDef="let e">{{ e.timestamp | timeAgo }}</td>
            </ng-container>
            <ng-container matColumnDef="performedBy">
              <th mat-header-cell *matHeaderCellDef>User</th>
              <td mat-cell *matCellDef="let e">
                <div class="flex-row gap-8">
                  <td-avatar [name]="e.performedBy" [imageUrl]="e.performedByAvatar" size="sm"></td-avatar>
                  <span>{{ e.performedBy }}</span>
                </div>
              </td>
            </ng-container>
            <ng-container matColumnDef="action">
              <th mat-header-cell *matHeaderCellDef>Action</th>
              <td mat-cell *matCellDef="let e">
                <mat-chip>{{ e.action }}</mat-chip>
              </td>
            </ng-container>
            <ng-container matColumnDef="entityType">
              <th mat-header-cell *matHeaderCellDef>Entity</th>
              <td mat-cell *matCellDef="let e">{{ e.entityType }}</td>
            </ng-container>
            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>Description</th>
              <td mat-cell *matCellDef="let e">{{ e.description }}</td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>

          <mat-paginator [length]="totalCount" [pageSize]="params.pageSize"
                         [pageSizeOptions]="[25, 50, 100]"
                         (page)="onPage($event)" showFirstLastButtons>
          </mat-paginator>
        }
      </div>
    </div>
  `,
  styles: [`
    .filters-bar { display: flex; gap: 12px; margin-bottom: 16px; flex-wrap: wrap; align-items: flex-start; }
    .filter-field { width: 180px; }
    .filter-field ::ng-deep .mat-mdc-form-field-subscript-wrapper { display: none; }
    table { width: 100%; }
  `]
})
export class AuditLogListComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ApiService);

  auditEntries: AuditEntry[] = [];
  totalCount = 0;
  loading = false;
  displayedColumns = ['timestamp', 'performedBy', 'action', 'entityType', 'description'];
  params: QueryParams = { ...DEFAULT_PAGINATION, pageSize: 50 };

  filterForm: FormGroup = this.fb.group({
    actionType: [''],
    entityType: [''],
    fromDate: [''],
    toDate: ['']
  });

  ngOnInit(): void { this.loadAudit(); }

  loadAudit(): void {
    this.loading = true;
    this.api.getPaginated<AuditEntry>('audit-logs', this.params).subscribe({
      next: (r) => { if (r.success) { this.auditEntries = r.data.items; this.totalCount = r.data.totalCount; } this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  applyFilters(): void { this.params.pageNumber = 1; this.loadAudit(); }
  resetFilters(): void { this.filterForm.reset(); this.applyFilters(); }
  onSort(s: Sort): void { this.params = { ...this.params, sortBy: s.active, sortDirection: s.direction as 'asc' | 'desc' }; this.loadAudit(); }
  onPage(e: PageEvent): void { this.params = { ...this.params, pageNumber: e.pageIndex + 1, pageSize: e.pageSize }; this.loadAudit(); }
}
