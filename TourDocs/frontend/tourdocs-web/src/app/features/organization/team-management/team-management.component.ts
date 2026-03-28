import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { ApiService } from '@core/services/api.service';
import { NotificationService } from '@core/services/notification.service';

interface TeamMember {
  id: string;
  name: string;
  email: string;
  role: string;
  status: string;
  joinedAt: string;
  avatarUrl: string | null;
}

@Component({
  selector: 'td-team-management',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatTableModule, MatButtonModule, MatIconModule,
    MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatCardModule, MatMenuModule,
    MatProgressSpinnerModule, PageHeaderComponent, AvatarComponent, StatusBadgeComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Team Management"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Organization'}, {label: 'Team'}]">
        <button mat-flat-button color="primary" (click)="showInviteForm = true">
          <mat-icon>person_add</mat-icon>
          Invite Member
        </button>
      </td-page-header>

      @if (showInviteForm) {
        <mat-card class="invite-card mb-24">
          <h3>Invite Team Member</h3>
          <form [formGroup]="inviteForm" (ngSubmit)="sendInvite()" class="invite-form">
            <mat-form-field appearance="outline">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" type="email" placeholder="colleague@company.com">
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Role</mat-label>
              <mat-select formControlName="role">
                <mat-option value="OrgOwner">Owner</mat-option>
                <mat-option value="OrgAdmin">Admin</mat-option>
                <mat-option value="OrgMember">Member</mat-option>
                <mat-option value="OrgViewer">Viewer</mat-option>
              </mat-select>
            </mat-form-field>
            <div class="invite-actions">
              <button mat-button type="button" (click)="showInviteForm = false">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="inviteForm.invalid || inviting">
                {{ inviting ? 'Sending...' : 'Send Invite' }}
              </button>
            </div>
          </form>
        </mat-card>
      }

      <div class="td-table-container">
        @if (loading) {
          <div class="table-loading-shade"><mat-spinner diameter="40"></mat-spinner></div>
        }

        @if (!loading && teamMembers.length === 0) {
          <td-empty-state icon="group" title="No team members" subtitle="Invite team members to collaborate">
          </td-empty-state>
        } @else {
          <table mat-table [dataSource]="teamMembers">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let m">
                <div class="flex-row gap-8">
                  <td-avatar [name]="m.name" [imageUrl]="m.avatarUrl" size="sm"></td-avatar>
                  <div>
                    <div class="text-bold">{{ m.name }}</div>
                    <div class="text-muted" style="font-size: 12px;">{{ m.email }}</div>
                  </div>
                </div>
              </td>
            </ng-container>
            <ng-container matColumnDef="role">
              <th mat-header-cell *matHeaderCellDef>Role</th>
              <td mat-cell *matCellDef="let m">{{ m.role }}</td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let m">
                <td-status-badge [status]="m.status" type="case"></td-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="joinedAt">
              <th mat-header-cell *matHeaderCellDef>Joined</th>
              <td mat-cell *matCellDef="let m">{{ m.joinedAt | date:'mediumDate' }}</td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef style="width: 60px;"></th>
              <td mat-cell *matCellDef="let m">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item><mat-icon>edit</mat-icon> Change Role</button>
                  <button mat-menu-item><mat-icon>block</mat-icon> Deactivate</button>
                  <button mat-menu-item><mat-icon>delete</mat-icon> Remove</button>
                </mat-menu>
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
    table { width: 100%; }
    .invite-card { padding: 24px; }
    .invite-card h3 { margin: 0 0 16px; font-size: 16px; font-weight: 600; }
    .invite-form { display: flex; gap: 12px; align-items: flex-start; flex-wrap: wrap; }
    .invite-form mat-form-field { flex: 1; min-width: 200px; }
    .invite-actions { display: flex; gap: 8px; align-items: center; padding-top: 4px; }
  `]
})
export class TeamManagementComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ApiService);
  private readonly notification = inject(NotificationService);

  teamMembers: TeamMember[] = [];
  loading = false;
  showInviteForm = false;
  inviting = false;
  displayedColumns = ['name', 'role', 'status', 'joinedAt', 'actions'];

  inviteForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    role: ['OrgMember', Validators.required]
  });

  ngOnInit(): void { this.loadTeam(); }

  loadTeam(): void {
    this.loading = true;
    this.api.get<TeamMember[]>('organization/team').subscribe({
      next: (r) => { if (r.success) this.teamMembers = r.data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  sendInvite(): void {
    if (this.inviteForm.valid) {
      this.inviting = true;
      this.api.post('organization/invite', this.inviteForm.value).subscribe({
        next: () => {
          this.inviting = false;
          this.showInviteForm = false;
          this.notification.showSuccess('Invitation sent');
          this.inviteForm.reset({ role: 'OrgMember' });
          this.loadTeam();
        },
        error: () => { this.inviting = false; }
      });
    }
  }
}
