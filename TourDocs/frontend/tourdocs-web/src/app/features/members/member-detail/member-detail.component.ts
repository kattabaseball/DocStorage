import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { TimeAgoPipe } from '@shared/pipes/time-ago.pipe';
import { MembersService } from '../members.service';
import { MemberDetail } from '../members.models';

@Component({
  selector: 'td-member-detail',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatTabsModule, MatCardModule, MatIconModule,
    MatButtonModule, MatTableModule, MatChipsModule, MatProgressSpinnerModule,
    PageHeaderComponent, AvatarComponent, StatusBadgeComponent, TimeAgoPipe
  ],
  template: `
    <div class="page-container">
      @if (loading) {
        <div class="flex-center" style="height: 300px;">
          <mat-spinner diameter="40"></mat-spinner>
        </div>
      } @else if (member) {
        <td-page-header title="{{ member.fullName }}"
                        [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Members', link: '/members'}, {label: member.fullName}]">
          <button mat-stroked-button [routerLink]="['/members', member.id, 'edit']">
            <mat-icon>edit</mat-icon> Edit
          </button>
        </td-page-header>

        <div class="member-profile">
          <mat-card class="member-profile__header">
            <td-avatar [name]="member.fullName" [imageUrl]="member.avatarUrl" size="lg"></td-avatar>
            <div class="member-profile__info">
              <h2>{{ member.fullName }}</h2>
              <p class="text-muted">{{ member.position }} - {{ member.department }}</p>
              <div class="member-profile__meta">
                <span><mat-icon>email</mat-icon> {{ member.email }}</span>
                <span><mat-icon>phone</mat-icon> {{ member.phone }}</span>
                <span><mat-icon>flag</mat-icon> {{ member.nationality }}</span>
              </div>
            </div>
            <td-status-badge [status]="member.status" type="case"></td-status-badge>
          </mat-card>
        </div>

        <mat-tab-group class="mt-24" animationDuration="200ms">
          <mat-tab label="Profile">
            <div class="grid-2col mt-16">
              <mat-card>
                <mat-card-header><mat-card-title>Personal Information</mat-card-title></mat-card-header>
                <mat-card-content>
                  <div class="info-grid">
                    <div class="info-item"><span class="info-label">Full Name</span><span>{{ member.fullName }}</span></div>
                    <div class="info-item"><span class="info-label">Date of Birth</span><span>{{ member.dateOfBirth | date:'mediumDate' }}</span></div>
                    <div class="info-item"><span class="info-label">Nationality</span><span>{{ member.nationality }}</span></div>
                    <div class="info-item"><span class="info-label">Passport No.</span><span>{{ member.passportNumber }}</span></div>
                    <div class="info-item"><span class="info-label">Email</span><span>{{ member.email }}</span></div>
                    <div class="info-item"><span class="info-label">Phone</span><span>{{ member.phone }}</span></div>
                  </div>
                </mat-card-content>
              </mat-card>

              <mat-card>
                <mat-card-header><mat-card-title>Address & Emergency</mat-card-title></mat-card-header>
                <mat-card-content>
                  <div class="info-grid">
                    <div class="info-item"><span class="info-label">Address</span><span>{{ member.address }}</span></div>
                    <div class="info-item"><span class="info-label">City</span><span>{{ member.city }}</span></div>
                    <div class="info-item"><span class="info-label">Country</span><span>{{ member.country }}</span></div>
                    <div class="info-item"><span class="info-label">Emergency Contact</span><span>{{ member.emergencyContactName }}</span></div>
                    <div class="info-item"><span class="info-label">Emergency Phone</span><span>{{ member.emergencyContactPhone }}</span></div>
                  </div>
                </mat-card-content>
              </mat-card>
            </div>
          </mat-tab>

          <mat-tab label="Documents">
            <div class="mt-16">
              @if (!member.documents || member.documents.length === 0) {
                <p class="text-muted text-center">No documents uploaded yet.</p>
              } @else {
                <table mat-table [dataSource]="member.documents" class="full-width">
                  <ng-container matColumnDef="title">
                    <th mat-header-cell *matHeaderCellDef>Document</th>
                    <td mat-cell *matCellDef="let doc">{{ doc.title }}</td>
                  </ng-container>
                  <ng-container matColumnDef="type">
                    <th mat-header-cell *matHeaderCellDef>Type</th>
                    <td mat-cell *matCellDef="let doc">{{ doc.type }}</td>
                  </ng-container>
                  <ng-container matColumnDef="status">
                    <th mat-header-cell *matHeaderCellDef>Status</th>
                    <td mat-cell *matCellDef="let doc"><td-status-badge [status]="doc.status" type="document"></td-status-badge></td>
                  </ng-container>
                  <ng-container matColumnDef="expiryDate">
                    <th mat-header-cell *matHeaderCellDef>Expiry</th>
                    <td mat-cell *matCellDef="let doc">{{ doc.expiryDate ? (doc.expiryDate | date:'mediumDate') : 'N/A' }}</td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="docColumns"></tr>
                  <tr mat-row *matRowDef="let row; columns: docColumns;"></tr>
                </table>
              }
            </div>
          </mat-tab>

          <mat-tab label="Travel History">
            <div class="mt-16">
              @if (!member.travelHistory || member.travelHistory.length === 0) {
                <p class="text-muted text-center">No travel history available.</p>
              } @else {
                <table mat-table [dataSource]="member.travelHistory" class="full-width">
                  <ng-container matColumnDef="caseName">
                    <th mat-header-cell *matHeaderCellDef>Case</th>
                    <td mat-cell *matCellDef="let t">{{ t.caseName }}</td>
                  </ng-container>
                  <ng-container matColumnDef="destination">
                    <th mat-header-cell *matHeaderCellDef>Destination</th>
                    <td mat-cell *matCellDef="let t">{{ t.destination }}</td>
                  </ng-container>
                  <ng-container matColumnDef="departureDate">
                    <th mat-header-cell *matHeaderCellDef>Departure</th>
                    <td mat-cell *matCellDef="let t">{{ t.departureDate | date:'mediumDate' }}</td>
                  </ng-container>
                  <ng-container matColumnDef="status">
                    <th mat-header-cell *matHeaderCellDef>Status</th>
                    <td mat-cell *matCellDef="let t"><td-status-badge [status]="t.status" type="case"></td-status-badge></td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="travelColumns"></tr>
                  <tr mat-row *matRowDef="let row; columns: travelColumns;"></tr>
                </table>
              }
            </div>
          </mat-tab>
        </mat-tab-group>
      }
    </div>
  `,
  styles: [`
    .member-profile__header {
      display: flex;
      align-items: center;
      gap: 20px;
      padding: 24px;
    }
    .member-profile__info h2 { margin: 0 0 4px; }
    .member-profile__meta {
      display: flex;
      gap: 16px;
      margin-top: 8px;
      font-size: 13px;
      color: #546E7A;
    }
    .member-profile__meta span {
      display: flex;
      align-items: center;
      gap: 4px;
    }
    .member-profile__meta mat-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
    }
    .info-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      padding: 16px 0;
    }
    .info-item {
      display: flex;
      flex-direction: column;
    }
    .info-label {
      font-size: 12px;
      color: #78909C;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      margin-bottom: 4px;
    }
    .full-width { width: 100%; }
  `]
})
export class MemberDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly membersService = inject(MembersService);

  member: MemberDetail | null = null;
  loading = true;
  docColumns = ['title', 'type', 'status', 'expiryDate'];
  travelColumns = ['caseName', 'destination', 'departureDate', 'status'];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.membersService.getMember(id).subscribe({
        next: (response) => {
          if (response.success) {
            this.member = response.data;
          }
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
    }
  }
}
