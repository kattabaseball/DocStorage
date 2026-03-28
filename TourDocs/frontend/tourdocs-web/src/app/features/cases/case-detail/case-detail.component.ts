import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { ConfirmDialogComponent } from '@shared/components/confirm-dialog/confirm-dialog.component';
import { TimeAgoPipe } from '@shared/pipes/time-ago.pipe';
import { NotificationService } from '@core/services/notification.service';
import { CasesService } from '../cases.service';
import { CaseDetail, CaseAccess, GrantAccessRequest } from '../cases.models';

@Component({
  selector: 'td-case-detail',
  standalone: true,
  imports: [
    CommonModule, RouterModule, ReactiveFormsModule,
    MatTabsModule, MatCardModule, MatIconModule,
    MatButtonModule, MatTableModule, MatProgressBarModule, MatProgressSpinnerModule,
    MatChipsModule, MatDialogModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatDatepickerModule, MatNativeDateModule,
    PageHeaderComponent, StatusBadgeComponent, AvatarComponent, ConfirmDialogComponent, TimeAgoPipe
  ],
  template: `
    <div class="page-container">
      @if (loading) {
        <div class="flex-center" style="height: 300px;"><mat-spinner diameter="40"></mat-spinner></div>
      } @else if (caseDetail) {
        <td-page-header [title]="caseDetail.name"
                        [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Cases', link: '/cases'}, {label: caseDetail.name}]">
          <td-status-badge [status]="caseDetail.status" type="case"></td-status-badge>
          <button mat-stroked-button [routerLink]="['/cases', caseDetail.id, 'edit']">
            <mat-icon>edit</mat-icon> Edit
          </button>
        </td-page-header>

        <div class="grid-4col mb-24">
          <mat-card class="info-card">
            <mat-icon>flight_takeoff</mat-icon>
            <div><span class="info-card__label">Destination</span><span class="info-card__value">{{ caseDetail.destination }}</span></div>
          </mat-card>
          <mat-card class="info-card">
            <mat-icon>calendar_today</mat-icon>
            <div><span class="info-card__label">Departure</span><span class="info-card__value">{{ caseDetail.departureDate | date:'mediumDate' }}</span></div>
          </mat-card>
          <mat-card class="info-card">
            <mat-icon>people</mat-icon>
            <div><span class="info-card__label">Members</span><span class="info-card__value">{{ caseDetail.memberCount }}</span></div>
          </mat-card>
          <mat-card class="info-card">
            <mat-icon>assessment</mat-icon>
            <div><span class="info-card__label">Readiness</span><span class="info-card__value">{{ caseDetail.readinessPercent }}%</span></div>
          </mat-card>
        </div>

        <mat-tab-group animationDuration="200ms">
          <mat-tab label="Overview">
            <mat-card class="mt-16">
              <mat-card-content>
                <p>{{ caseDetail.description }}</p>
                <div class="mt-16">
                  <strong>Checklist:</strong> {{ caseDetail.checklistName }}
                </div>
              </mat-card-content>
            </mat-card>
          </mat-tab>

          <mat-tab label="Members">
            <div class="mt-16">
              <table mat-table [dataSource]="caseDetail.members">
                <ng-container matColumnDef="member">
                  <th mat-header-cell *matHeaderCellDef>Member</th>
                  <td mat-cell *matCellDef="let m">
                    <div class="flex-row gap-8">
                      <td-avatar [name]="m.memberName" [imageUrl]="m.avatarUrl" size="sm"></td-avatar>
                      <span>{{ m.memberName }}</span>
                    </div>
                  </td>
                </ng-container>
                <ng-container matColumnDef="readiness">
                  <th mat-header-cell *matHeaderCellDef>Readiness</th>
                  <td mat-cell *matCellDef="let m">
                    <div class="completion-bar">
                      <mat-progress-bar mode="determinate" [value]="m.readinessPercent"
                        [color]="m.readinessPercent >= 80 ? 'primary' : 'warn'"></mat-progress-bar>
                      <span>{{ m.readinessPercent }}%</span>
                    </div>
                  </td>
                </ng-container>
                <ng-container matColumnDef="docs">
                  <th mat-header-cell *matHeaderCellDef>Documents</th>
                  <td mat-cell *matCellDef="let m">{{ m.totalDocs - m.pendingDocs }}/{{ m.totalDocs }}</td>
                </ng-container>
                <tr mat-header-row *matHeaderRowDef="memberColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: memberColumns;"></tr>
              </table>
            </div>
          </mat-tab>

          <mat-tab label="Checklist">
            <div class="mt-16">
              <table mat-table [dataSource]="caseDetail.checklist">
                <ng-container matColumnDef="name">
                  <th mat-header-cell *matHeaderCellDef>Item</th>
                  <td mat-cell *matCellDef="let item">{{ item.name }}</td>
                </ng-container>
                <ng-container matColumnDef="category">
                  <th mat-header-cell *matHeaderCellDef>Category</th>
                  <td mat-cell *matCellDef="let item">{{ item.category }}</td>
                </ng-container>
                <ng-container matColumnDef="required">
                  <th mat-header-cell *matHeaderCellDef>Required</th>
                  <td mat-cell *matCellDef="let item">
                    <mat-icon [style.color]="item.required ? '#EF5350' : '#90A4AE'">
                      {{ item.required ? 'check_circle' : 'remove_circle_outline' }}
                    </mat-icon>
                  </td>
                </ng-container>
                <ng-container matColumnDef="progress">
                  <th mat-header-cell *matHeaderCellDef>Progress</th>
                  <td mat-cell *matCellDef="let item">{{ item.completedCount }}/{{ item.totalCount }}</td>
                </ng-container>
                <tr mat-header-row *matHeaderRowDef="checklistColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: checklistColumns;"></tr>
              </table>
            </div>
          </mat-tab>

          <mat-tab label="Sharing">
            <div class="mt-16">
              <mat-card class="sharing-card">
                <mat-card-content>
                  <h3 class="sharing-card__title">
                    <mat-icon>share</mat-icon> Share Case Documents
                  </h3>
                  <p class="text-muted mb-16">
                    Grant external parties access to view or download documents in this case.
                  </p>

                  <form [formGroup]="shareForm" (ngSubmit)="grantAccess()" class="share-form">
                    <mat-form-field appearance="outline">
                      <mat-label>Email Address</mat-label>
                      <input matInput formControlName="email" type="email" placeholder="partner@example.com">
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>Permission</mat-label>
                      <mat-select formControlName="permission">
                        <mat-option value="View">View Only</mat-option>
                        <mat-option value="ViewDownload">View & Download</mat-option>
                        <mat-option value="ViewDownloadRequest">View, Download & Request</mat-option>
                      </mat-select>
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>Expires (optional)</mat-label>
                      <input matInput [matDatepicker]="expiryPicker" formControlName="expiresAt">
                      <mat-datepicker-toggle matIconSuffix [for]="expiryPicker"></mat-datepicker-toggle>
                      <mat-datepicker #expiryPicker></mat-datepicker>
                    </mat-form-field>
                    <button mat-flat-button color="primary" type="submit"
                            [disabled]="shareForm.invalid || sharing">
                      <mat-icon>person_add</mat-icon> Grant Access
                    </button>
                  </form>
                </mat-card-content>
              </mat-card>

              @if (sharedAccess.length > 0) {
                <h4 class="mt-24 mb-16">People with access</h4>
                <table mat-table [dataSource]="sharedAccess" class="full-width">
                  <ng-container matColumnDef="email">
                    <th mat-header-cell *matHeaderCellDef>Email</th>
                    <td mat-cell *matCellDef="let a">{{ a.email }}</td>
                  </ng-container>
                  <ng-container matColumnDef="permission">
                    <th mat-header-cell *matHeaderCellDef>Permission</th>
                    <td mat-cell *matCellDef="let a">
                      <mat-chip-set>
                        <mat-chip [highlighted]="true">{{ a.permission }}</mat-chip>
                      </mat-chip-set>
                    </td>
                  </ng-container>
                  <ng-container matColumnDef="grantedAt">
                    <th mat-header-cell *matHeaderCellDef>Granted</th>
                    <td mat-cell *matCellDef="let a">{{ a.grantedAt | timeAgo }}</td>
                  </ng-container>
                  <ng-container matColumnDef="expiresAt">
                    <th mat-header-cell *matHeaderCellDef>Expires</th>
                    <td mat-cell *matCellDef="let a">{{ a.expiresAt ? (a.expiresAt | date:'mediumDate') : 'Never' }}</td>
                  </ng-container>
                  <ng-container matColumnDef="actions">
                    <th mat-header-cell *matHeaderCellDef></th>
                    <td mat-cell *matCellDef="let a">
                      <button mat-icon-button color="warn" (click)="revokeAccess(a)">
                        <mat-icon>person_remove</mat-icon>
                      </button>
                    </td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="accessColumns"></tr>
                  <tr mat-row *matRowDef="let row; columns: accessColumns;"></tr>
                </table>
              } @else {
                <p class="text-muted text-center mt-24">No external access granted yet.</p>
              }
            </div>
          </mat-tab>

          <mat-tab label="Audit Trail">
            <div class="audit-list mt-16">
              @for (entry of caseDetail.auditTrail; track entry.id) {
                <div class="audit-entry">
                  <div class="audit-entry__action">
                    <strong>{{ entry.action }}</strong>
                    <span class="text-muted">{{ entry.description }}</span>
                  </div>
                  <div class="audit-entry__meta">
                    <span>{{ entry.performedBy }}</span>
                    <span class="text-muted">{{ entry.performedAt | timeAgo }}</span>
                  </div>
                </div>
              }
            </div>
          </mat-tab>
        </mat-tab-group>
      }
    </div>
  `,
  styles: [`
    .info-card {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 16px;
    }
    .info-card mat-icon { color: #1565C0; font-size: 28px; width: 28px; height: 28px; }
    .info-card__label { display: block; font-size: 12px; color: #78909C; text-transform: uppercase; }
    .info-card__value { display: block; font-size: 18px; font-weight: 600; color: #263238; }
    table { width: 100%; }
    .completion-bar { display: flex; align-items: center; gap: 8px; max-width: 160px; }
    .audit-list { display: flex; flex-direction: column; gap: 12px; }
    .audit-entry { display: flex; justify-content: space-between; padding: 12px 16px; background: #FAFAFA; border-radius: 8px; }
    .audit-entry__action { display: flex; flex-direction: column; gap: 4px; }
    .audit-entry__meta { display: flex; flex-direction: column; align-items: flex-end; gap: 4px; font-size: 13px; }
    .sharing-card__title { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
    .sharing-card__title mat-icon { color: #1565C0; }
    .share-form { display: flex; gap: 12px; align-items: flex-start; flex-wrap: wrap; }
    .share-form mat-form-field { flex: 1; min-width: 200px; }
    .share-form button { margin-top: 8px; height: 48px; }
    .full-width { width: 100%; }
  `]
})
export class CaseDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly casesService = inject(CasesService);
  private readonly fb = inject(FormBuilder);
  private readonly dialog = inject(MatDialog);
  private readonly notification = inject(NotificationService);

  caseDetail: CaseDetail | null = null;
  loading = true;
  sharing = false;
  sharedAccess: CaseAccess[] = [];
  memberColumns = ['member', 'readiness', 'docs'];
  checklistColumns = ['name', 'category', 'required', 'progress'];
  accessColumns = ['email', 'permission', 'grantedAt', 'expiresAt', 'actions'];

  shareForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    permission: ['View', Validators.required],
    expiresAt: ['']
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.casesService.getCase(id).subscribe({
        next: (r) => {
          if (r.success) this.caseDetail = r.data;
          this.loading = false;
        },
        error: () => { this.loading = false; }
      });
    }
  }

  grantAccess(): void {
    if (this.shareForm.invalid || !this.caseDetail) return;
    this.sharing = true;
    const formValue = this.shareForm.value;
    const request: GrantAccessRequest = {
      email: formValue.email,
      permission: formValue.permission,
      expiresAt: formValue.expiresAt ? new Date(formValue.expiresAt).toISOString() : undefined
    };

    this.casesService.grantAccess(this.caseDetail.id, request).subscribe({
      next: (r) => {
        if (r.success) {
          this.sharedAccess = [...this.sharedAccess, r.data];
          this.shareForm.reset({ permission: 'View' });
          this.notification.showSuccess(`Access granted to ${request.email}`);
        }
        this.sharing = false;
      },
      error: () => {
        this.sharing = false;
        this.notification.showError('Failed to grant access');
      }
    });
  }

  revokeAccess(access: CaseAccess): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Revoke Access',
        message: `Remove access for ${access.email}?`,
        confirmText: 'Revoke',
        color: 'warn'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.casesService.revokeAccess(access.id).subscribe({
          next: () => {
            this.sharedAccess = this.sharedAccess.filter(a => a.id !== access.id);
            this.notification.showSuccess('Access revoked');
          },
          error: () => this.notification.showError('Failed to revoke access')
        });
      }
    });
  }
}
