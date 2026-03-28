import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TimelineComponent, TimelineStep } from '@shared/components/timeline/timeline.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { ApiService } from '@core/services/api.service';

interface HardCopyDetail {
  id: string;
  documentTitle: string;
  memberName: string;
  status: string;
  timeline: TimelineStep[];
}

@Component({
  selector: 'td-hard-copy-timeline',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatCardModule, MatIconModule,
    MatButtonModule, MatProgressSpinnerModule,
    PageHeaderComponent, TimelineComponent, StatusBadgeComponent
  ],
  template: `
    <div class="page-container">
      @if (loading) {
        <div class="flex-center" style="height: 300px;"><mat-spinner diameter="40"></mat-spinner></div>
      } @else if (hardCopy) {
        <td-page-header [title]="hardCopy.documentTitle + ' - Timeline'"
                        [breadcrumbs]="[
                          {label: 'Home', link: '/', icon: 'home'},
                          {label: 'Hard Copies', link: '/hard-copies'},
                          {label: 'Timeline'}
                        ]">
          <td-status-badge [status]="hardCopy.status" type="hardcopy"></td-status-badge>
        </td-page-header>

        <div class="grid-2col">
          <mat-card>
            <mat-card-header>
              <mat-card-title>Tracking Timeline</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <td-timeline [steps]="hardCopy.timeline"></td-timeline>
            </mat-card-content>
          </mat-card>

          <mat-card>
            <mat-card-header>
              <mat-card-title>Details</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="detail-grid">
                <div class="detail-item">
                  <span class="detail-label">Document</span>
                  <span class="detail-value">{{ hardCopy.documentTitle }}</span>
                </div>
                <div class="detail-item">
                  <span class="detail-label">Member</span>
                  <span class="detail-value">{{ hardCopy.memberName }}</span>
                </div>
                <div class="detail-item">
                  <span class="detail-label">Current Status</span>
                  <td-status-badge [status]="hardCopy.status" type="hardcopy"></td-status-badge>
                </div>
              </div>
            </mat-card-content>
          </mat-card>
        </div>
      }
    </div>
  `,
  styles: [`
    .detail-grid { display: flex; flex-direction: column; gap: 16px; padding: 16px 0; }
    .detail-item { display: flex; flex-direction: column; }
    .detail-label { font-size: 12px; color: #78909C; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 4px; }
    .detail-value { font-size: 14px; font-weight: 500; color: #263238; }
  `]
})
export class HardCopyTimelineComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);

  hardCopy: HardCopyDetail | null = null;
  loading = true;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.get<HardCopyDetail>(`hard-copies/${id}`).subscribe({
        next: (r) => { if (r.success) this.hardCopy = r.data; this.loading = false; },
        error: () => { this.loading = false; }
      });
    }
  }
}
