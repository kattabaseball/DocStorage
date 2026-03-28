import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { SearchInputComponent } from '@shared/components/search-input/search-input.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '@shared/components/confirm-dialog/confirm-dialog.component';
import { CasesService } from '../cases.service';
import { NotificationService } from '@core/services/notification.service';
import { Case } from '../cases.models';
import { DEFAULT_PAGINATION, QueryParams } from '@core/models/pagination.model';

@Component({
  selector: 'td-case-list',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatTableModule, MatPaginatorModule, MatSortModule,
    MatButtonModule, MatIconModule, MatMenuModule, MatProgressSpinnerModule, MatProgressBarModule,
    MatDialogModule,
    PageHeaderComponent, SearchInputComponent, StatusBadgeComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Cases"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Cases'}]">
        <button mat-flat-button color="primary" routerLink="/cases/create">
          <mat-icon>add</mat-icon>
          Create Case
        </button>
      </td-page-header>

      <div class="td-table-container">
        <div class="td-table-header">
          <td-search-input placeholder="Search cases..." (searchChange)="onSearch($event)"></td-search-input>
        </div>

        @if (loading) {
          <div class="table-loading-shade"><mat-spinner diameter="40"></mat-spinner></div>
        }

        @if (!loading && cases.length === 0) {
          <td-empty-state icon="work_off" title="No cases found"
                          subtitle="Create your first case to start organizing documents"
                          actionText="Create Case" actionLink="/cases/create">
          </td-empty-state>
        } @else {
          <table mat-table [dataSource]="cases" matSort (matSortChange)="onSort($event)">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Case Name</th>
              <td mat-cell *matCellDef="let c">
                <span class="text-bold">{{ c.name }}</span>
              </td>
            </ng-container>
            <ng-container matColumnDef="destination">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Destination</th>
              <td mat-cell *matCellDef="let c">{{ c.destination }}</td>
            </ng-container>
            <ng-container matColumnDef="departureDate">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Departure</th>
              <td mat-cell *matCellDef="let c">{{ c.departureDate | date:'mediumDate' }}</td>
            </ng-container>
            <ng-container matColumnDef="memberCount">
              <th mat-header-cell *matHeaderCellDef>Members</th>
              <td mat-cell *matCellDef="let c">{{ c.memberCount }}</td>
            </ng-container>
            <ng-container matColumnDef="readinessPercent">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Readiness</th>
              <td mat-cell *matCellDef="let c">
                <div class="completion-bar">
                  <mat-progress-bar mode="determinate" [value]="c.readinessPercent"
                    [color]="c.readinessPercent >= 80 ? 'primary' : 'warn'"></mat-progress-bar>
                  <span class="completion-text">{{ c.readinessPercent }}%</span>
                </div>
              </td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let c">
                <td-status-badge [status]="c.status" type="case"></td-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef style="width: 60px;"></th>
              <td mat-cell *matCellDef="let c">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/cases', c.id]"><mat-icon>visibility</mat-icon> View</button>
                  <button mat-menu-item [routerLink]="['/cases', c.id, 'edit']"><mat-icon>edit</mat-icon> Edit</button>
                  <button mat-menu-item (click)="deleteCase(c)"><mat-icon>delete</mat-icon> Delete</button>
                </mat-menu>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"
                class="clickable" (click)="viewCase(row)"></tr>
          </table>

          <mat-paginator [length]="totalCount" [pageSize]="params.pageSize"
                         [pageSizeOptions]="[10, 25, 50]" (page)="onPage($event)" showFirstLastButtons>
          </mat-paginator>
        }
      </div>
    </div>
  `,
  styles: [`
    table { width: 100%; }
    .completion-bar { display: flex; align-items: center; gap: 8px; max-width: 160px; }
    .completion-text { font-size: 12px; font-weight: 600; color: #546E7A; min-width: 36px; }
    .clickable { cursor: pointer; }
  `]
})
export class CaseListComponent implements OnInit {
  private readonly casesService = inject(CasesService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly notification = inject(NotificationService);

  cases: Case[] = [];
  totalCount = 0;
  loading = false;
  displayedColumns = ['name', 'destination', 'departureDate', 'memberCount', 'readinessPercent', 'status', 'actions'];
  params: QueryParams = { ...DEFAULT_PAGINATION };

  ngOnInit(): void { this.loadCases(); }

  loadCases(): void {
    this.loading = true;
    this.casesService.getCases(this.params).subscribe({
      next: (r) => { if (r.success) { this.cases = r.data.items; this.totalCount = r.data.totalCount; } this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  onSearch(q: string): void { this.params = { ...this.params, search: q, pageNumber: 1 }; this.loadCases(); }
  onSort(s: Sort): void { this.params = { ...this.params, sortBy: s.active, sortDirection: s.direction as 'asc' | 'desc', pageNumber: 1 }; this.loadCases(); }
  onPage(e: PageEvent): void { this.params = { ...this.params, pageNumber: e.pageIndex + 1, pageSize: e.pageSize }; this.loadCases(); }
  viewCase(c: Case): void { this.router.navigate(['/cases', c.id]); }
  deleteCase(c: Case): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Case',
        message: `Are you sure you want to delete "${c.name}"?`,
        confirmText: 'Delete',
        color: 'warn'
      }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.casesService.deleteCase(c.id).subscribe({
          next: () => {
            this.notification.showSuccess('Case deleted');
            this.loadCases();
          },
          error: () => this.notification.showError('Failed to delete case')
        });
      }
    });
  }
}
