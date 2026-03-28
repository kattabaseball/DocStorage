import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { ApiService } from '@core/services/api.service';

interface MyCase {
  id: string;
  name: string;
  destination: string;
  departureDate: string;
  status: string;
  myReadinessPercent: number;
  pendingDocs: number;
}

@Component({
  selector: 'td-my-cases',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatCardModule, MatIconModule,
    MatButtonModule, MatBadgeModule, MatProgressBarModule,
    MatProgressSpinnerModule, PageHeaderComponent, StatusBadgeComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="My Cases"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'My Cases'}]">
      </td-page-header>

      @if (loading) {
        <div class="flex-center" style="height: 300px;"><mat-spinner diameter="40"></mat-spinner></div>
      } @else if (cases.length === 0) {
        <td-empty-state icon="work_off" title="No upcoming cases"
                        subtitle="You haven't been assigned to any cases yet">
        </td-empty-state>
      } @else {
        <div class="grid-2col">
          @for (c of cases; track c.id) {
            <mat-card class="case-card">
              <div class="case-card__header">
                <div>
                  <h3 class="case-card__name">{{ c.name }}</h3>
                  <p class="case-card__destination">
                    <mat-icon>flight_takeoff</mat-icon> {{ c.destination }}
                  </p>
                </div>
                <td-status-badge [status]="c.status" type="case"></td-status-badge>
              </div>

              <div class="case-card__info">
                <div class="case-card__date">
                  <mat-icon>calendar_today</mat-icon>
                  <span>{{ c.departureDate | date:'mediumDate' }}</span>
                </div>
                @if (c.pendingDocs > 0) {
                  <div class="case-card__badge">
                    <mat-icon [matBadge]="c.pendingDocs" matBadgeColor="warn" matBadgeSize="small">description</mat-icon>
                    <span class="text-warn">{{ c.pendingDocs }} pending docs</span>
                  </div>
                }
              </div>

              <div class="case-card__readiness">
                <div class="case-card__readiness-label">
                  <span>My Readiness</span>
                  <span class="text-bold">{{ c.myReadinessPercent }}%</span>
                </div>
                <mat-progress-bar mode="determinate" [value]="c.myReadinessPercent"
                  [color]="c.myReadinessPercent >= 80 ? 'primary' : 'warn'">
                </mat-progress-bar>
              </div>

              <button mat-stroked-button color="primary" [routerLink]="['/portal/my-documents']" [queryParams]="{caseId: c.id}" class="mt-16">
                View Required Documents
              </button>
            </mat-card>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .case-card { padding: 24px; }
    .case-card__header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px; }
    .case-card__name { font-size: 18px; font-weight: 600; margin: 0 0 4px; }
    .case-card__destination { display: flex; align-items: center; gap: 4px; font-size: 13px; color: #546E7A; margin: 0; }
    .case-card__destination mat-icon { font-size: 16px; width: 16px; height: 16px; }
    .case-card__info { display: flex; align-items: center; gap: 16px; margin-bottom: 16px; font-size: 13px; color: #546E7A; }
    .case-card__date { display: flex; align-items: center; gap: 4px; }
    .case-card__date mat-icon { font-size: 16px; width: 16px; height: 16px; }
    .case-card__badge { display: flex; align-items: center; gap: 4px; }
    .case-card__readiness-label { display: flex; justify-content: space-between; margin-bottom: 8px; font-size: 13px; }
    .text-warn { color: #F44336; }
  `]
})
export class MyCasesComponent implements OnInit {
  private readonly api = inject(ApiService);
  cases: MyCase[] = [];
  loading = false;

  ngOnInit(): void {
    this.loading = true;
    this.api.get<MyCase[]>('portal/my-cases').subscribe({
      next: (r) => { if (r.success) this.cases = r.data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
