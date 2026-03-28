import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { SearchInputComponent } from '@shared/components/search-input/search-input.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { ApiService } from '@core/services/api.service';

interface HardCopy {
  id: string;
  documentTitle: string;
  memberName: string;
  documentType: string;
  status: string;
  lastUpdated: string;
  handler: string;
}

@Component({
  selector: 'td-hard-copy-list',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatTableModule, MatPaginatorModule, MatSortModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    PageHeaderComponent, SearchInputComponent, StatusBadgeComponent, AvatarComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Hard Copies"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Hard Copies'}]">
      </td-page-header>

      <div class="td-table-container">
        <div class="td-table-header">
          <td-search-input placeholder="Search hard copies..." (searchChange)="onSearch($event)"></td-search-input>
        </div>

        @if (loading) {
          <div class="table-loading-shade"><mat-spinner diameter="40"></mat-spinner></div>
        }

        @if (!loading && hardCopies.length === 0) {
          <td-empty-state icon="inventory" title="No hard copies tracked"
                          subtitle="Hard copy tracking will appear here once documents require physical handling">
          </td-empty-state>
        } @else {
          <table mat-table [dataSource]="hardCopies" matSort>
            <ng-container matColumnDef="memberName">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Member</th>
              <td mat-cell *matCellDef="let hc">
                <div class="flex-row gap-8">
                  <td-avatar [name]="hc.memberName" size="sm"></td-avatar>
                  <span>{{ hc.memberName }}</span>
                </div>
              </td>
            </ng-container>
            <ng-container matColumnDef="documentTitle">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Document</th>
              <td mat-cell *matCellDef="let hc">{{ hc.documentTitle }}</td>
            </ng-container>
            <ng-container matColumnDef="documentType">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let hc">{{ hc.documentType }}</td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let hc">
                <td-status-badge [status]="hc.status" type="hardcopy"></td-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="handler">
              <th mat-header-cell *matHeaderCellDef>Handler</th>
              <td mat-cell *matCellDef="let hc">{{ hc.handler }}</td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef style="width: 80px;"></th>
              <td mat-cell *matCellDef="let hc">
                <button mat-icon-button [routerLink]="['/hard-copies', hc.id, 'timeline']" matTooltip="View Timeline">
                  <mat-icon>timeline</mat-icon>
                </button>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        }
      </div>
    </div>
  `,
  styles: [`table { width: 100%; }`]
})
export class HardCopyListComponent implements OnInit {
  private readonly api = inject(ApiService);
  hardCopies: HardCopy[] = [];
  loading = false;
  displayedColumns = ['memberName', 'documentTitle', 'documentType', 'status', 'handler', 'actions'];

  ngOnInit(): void { this.loadData(); }

  loadData(): void {
    this.loading = true;
    this.api.get<HardCopy[]>('hard-copies').subscribe({
      next: (r) => { if (r.success) this.hardCopies = r.data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  onSearch(query: string): void {
    this.loadData();
  }
}
