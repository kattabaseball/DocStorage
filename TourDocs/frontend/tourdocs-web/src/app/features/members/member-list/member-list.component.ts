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
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '@shared/components/confirm-dialog/confirm-dialog.component';
import { MembersService } from '../members.service';
import { NotificationService } from '@core/services/notification.service';
import { Member } from '../members.models';
import { DEFAULT_PAGINATION, QueryParams } from '@core/models/pagination.model';

@Component({
  selector: 'td-member-list',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatTableModule, MatPaginatorModule, MatSortModule,
    MatButtonModule, MatIconModule, MatMenuModule, MatProgressSpinnerModule, MatProgressBarModule,
    MatDialogModule,
    PageHeaderComponent, SearchInputComponent, AvatarComponent, StatusBadgeComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Members" [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Members'}]">
        <button mat-flat-button color="primary" routerLink="/members/new">
          <mat-icon>person_add</mat-icon>
          Add Member
        </button>
      </td-page-header>

      <div class="td-table-container">
        <div class="td-table-header">
          <td-search-input placeholder="Search members..." (searchChange)="onSearch($event)"></td-search-input>
        </div>

        @if (loading) {
          <div class="table-loading-shade">
            <mat-spinner diameter="40"></mat-spinner>
          </div>
        }

        @if (!loading && members.length === 0) {
          <td-empty-state icon="people" title="No members found"
                          subtitle="Add your first team member to get started"
                          actionText="Add Member" actionLink="/members/new">
          </td-empty-state>
        } @else {
          <table mat-table [dataSource]="members" matSort (matSortChange)="onSort($event)">
            <ng-container matColumnDef="avatar">
              <th mat-header-cell *matHeaderCellDef style="width: 56px;"></th>
              <td mat-cell *matCellDef="let member">
                <td-avatar [name]="member.fullName" [imageUrl]="member.avatarUrl" size="sm"></td-avatar>
              </td>
            </ng-container>

            <ng-container matColumnDef="fullName">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Name</th>
              <td mat-cell *matCellDef="let member">
                <div class="member-name">
                  <span class="member-name__primary">{{ member.fullName }}</span>
                  <span class="member-name__secondary">{{ member.email }}</span>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="department">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Department</th>
              <td mat-cell *matCellDef="let member">{{ member.department }}</td>
            </ng-container>

            <ng-container matColumnDef="documentCompletionPercent">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Doc Completion</th>
              <td mat-cell *matCellDef="let member">
                <div class="completion-bar">
                  <mat-progress-bar mode="determinate" [value]="member.documentCompletionPercent"
                    [color]="member.documentCompletionPercent >= 80 ? 'primary' : 'warn'">
                  </mat-progress-bar>
                  <span class="completion-bar__text">{{ member.documentCompletionPercent }}%</span>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let member">
                <td-status-badge [status]="member.status" type="case"></td-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef style="width: 60px;"></th>
              <td mat-cell *matCellDef="let member">
                <button mat-icon-button [matMenuTriggerFor]="actionMenu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #actionMenu="matMenu">
                  <button mat-menu-item [routerLink]="['/members', member.id]">
                    <mat-icon>visibility</mat-icon> View Details
                  </button>
                  <button mat-menu-item [routerLink]="['/members', member.id, 'edit']">
                    <mat-icon>edit</mat-icon> Edit
                  </button>
                  <button mat-menu-item (click)="deleteMember(member)">
                    <mat-icon>delete</mat-icon> Delete
                  </button>
                </mat-menu>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"
                class="clickable" (click)="viewMember(row)"></tr>
          </table>

          <mat-paginator [length]="totalCount" [pageSize]="queryParams.pageSize"
                         [pageSizeOptions]="[10, 25, 50]"
                         (page)="onPage($event)" showFirstLastButtons>
          </mat-paginator>
        }
      </div>
    </div>
  `,
  styles: [`
    .member-name {
      display: flex;
      flex-direction: column;
    }
    .member-name__primary {
      font-weight: 600;
      color: #263238;
    }
    .member-name__secondary {
      font-size: 12px;
      color: #78909C;
    }
    .completion-bar {
      display: flex;
      align-items: center;
      gap: 8px;
      max-width: 160px;
    }
    .completion-bar__text {
      font-size: 12px;
      font-weight: 600;
      color: #546E7A;
      min-width: 36px;
    }
    table { width: 100%; }
    .clickable { cursor: pointer; }
  `]
})
export class MemberListComponent implements OnInit {
  private readonly membersService = inject(MembersService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly notification = inject(NotificationService);

  members: Member[] = [];
  totalCount = 0;
  loading = false;
  displayedColumns = ['avatar', 'fullName', 'department', 'documentCompletionPercent', 'status', 'actions'];
  queryParams: QueryParams = { ...DEFAULT_PAGINATION };

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(): void {
    this.loading = true;
    this.membersService.getMembers(this.queryParams).subscribe({
      next: (response) => {
        if (response.success) {
          this.members = response.data.items;
          this.totalCount = response.data.totalCount;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onSearch(query: string): void {
    this.queryParams = { ...this.queryParams, search: query, pageNumber: 1 };
    this.loadMembers();
  }

  onSort(sort: Sort): void {
    this.queryParams = {
      ...this.queryParams,
      sortBy: sort.active,
      sortDirection: sort.direction as 'asc' | 'desc',
      pageNumber: 1
    };
    this.loadMembers();
  }

  onPage(event: PageEvent): void {
    this.queryParams = {
      ...this.queryParams,
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    };
    this.loadMembers();
  }

  viewMember(member: Member): void {
    this.router.navigate(['/members', member.id]);
  }

  deleteMember(member: Member): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Member',
        message: `Are you sure you want to delete ${member.fullName}?`,
        confirmText: 'Delete',
        color: 'warn'
      }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.membersService.deleteMember(member.id).subscribe({
          next: () => {
            this.notification.showSuccess('Member deleted');
            this.loadMembers();
          },
          error: () => this.notification.showError('Failed to delete member')
        });
      }
    });
  }
}
