import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { ApiService } from '@core/services/api.service';
import { NotificationService } from '@core/services/notification.service';

interface SubscriptionResponse {
  id: string;
  organizationId: string;
  plan: number;
  status: string;
  maxMembers: number;
  maxCasesMonthly: number;
  maxExternalUsers: number;
  maxStorageBytes: number;
  currentPeriodStart: string;
  currentPeriodEnd: string;
  createdAt: string;
}

interface SubscriptionUsageResponse {
  currentMembers: number;
  maxMembers: number;
  currentCasesThisMonth: number;
  maxCasesMonthly: number;
  currentStorageBytes: number;
  maxStorageBytes: number;
  memberUsagePercent: number;
  storageUsagePercent: number;
}

interface PlanCard {
  name: string;
  value: number;
  price: string;
  description: string;
  features: string[];
  highlighted: boolean;
}

@Component({
  selector: 'td-subscription',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatProgressBarModule, MatProgressSpinnerModule,
    MatChipsModule, PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Subscription"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Organization'}, {label: 'Subscription'}]">
      </td-page-header>

      @if (loading) {
        <div class="loading-container">
          <mat-spinner diameter="48"></mat-spinner>
        </div>
      } @else {

        <!-- Current Plan Summary -->
        @if (subscription) {
          <mat-card class="current-plan-card">
            <div class="current-plan-card__content">
              <div class="current-plan-card__info">
                <div class="current-plan-card__label">Current Plan</div>
                <div class="current-plan-card__name">{{ getPlanName(subscription.plan) }}</div>
                <div class="current-plan-card__status">
                  <span class="status-badge status-badge--{{ subscription.status.toLowerCase() }}">
                    {{ subscription.status }}
                  </span>
                </div>
              </div>
              <div class="current-plan-card__period">
                <mat-icon>calendar_today</mat-icon>
                <span>{{ subscription.currentPeriodStart | date:'mediumDate' }} - {{ subscription.currentPeriodEnd | date:'mediumDate' }}</span>
              </div>
            </div>
          </mat-card>
        }

        <!-- Usage Statistics -->
        @if (usage) {
          <h2 class="section-title">Usage</h2>
          <div class="usage-grid">
            <mat-card class="usage-card">
              <div class="usage-card__header">
                <mat-icon class="usage-card__icon">people</mat-icon>
                <span class="usage-card__label">Members</span>
              </div>
              <div class="usage-card__values">
                <span class="usage-card__current">{{ usage.currentMembers }}</span>
                <span class="usage-card__separator">/</span>
                <span class="usage-card__max">{{ usage.maxMembers }}</span>
              </div>
              <mat-progress-bar
                [mode]="'determinate'"
                [value]="usage.memberUsagePercent"
                [color]="getProgressColor(usage.memberUsagePercent)">
              </mat-progress-bar>
            </mat-card>

            <mat-card class="usage-card">
              <div class="usage-card__header">
                <mat-icon class="usage-card__icon">cloud_upload</mat-icon>
                <span class="usage-card__label">Storage</span>
              </div>
              <div class="usage-card__values">
                <span class="usage-card__current">{{ formatBytes(usage.currentStorageBytes) }}</span>
                <span class="usage-card__separator">/</span>
                <span class="usage-card__max">{{ formatBytes(usage.maxStorageBytes) }}</span>
              </div>
              <mat-progress-bar
                [mode]="'determinate'"
                [value]="usage.storageUsagePercent"
                [color]="getProgressColor(usage.storageUsagePercent)">
              </mat-progress-bar>
            </mat-card>

            <mat-card class="usage-card">
              <div class="usage-card__header">
                <mat-icon class="usage-card__icon">work</mat-icon>
                <span class="usage-card__label">Cases This Month</span>
              </div>
              <div class="usage-card__values">
                <span class="usage-card__current">{{ usage.currentCasesThisMonth }}</span>
                <span class="usage-card__separator">/</span>
                <span class="usage-card__max">{{ usage.maxCasesMonthly }}</span>
              </div>
              <mat-progress-bar
                [mode]="'determinate'"
                [value]="getCasesPercent()"
                [color]="getProgressColor(getCasesPercent())">
              </mat-progress-bar>
            </mat-card>
          </div>
        }

        <!-- Plan Cards -->
        <h2 class="section-title">Available Plans</h2>
        <div class="plans-grid">
          @for (plan of plans; track plan.value) {
            <mat-card class="plan-card" [class.plan-card--active]="subscription?.plan === plan.value"
                      [class.plan-card--highlighted]="plan.highlighted">
              @if (subscription?.plan === plan.value) {
                <div class="plan-card__badge">Current Plan</div>
              }
              @if (plan.highlighted && subscription?.plan !== plan.value) {
                <div class="plan-card__badge plan-card__badge--recommended">Recommended</div>
              }
              <div class="plan-card__header">
                <h3 class="plan-card__name">{{ plan.name }}</h3>
                <div class="plan-card__price">{{ plan.price }}</div>
                <p class="plan-card__description">{{ plan.description }}</p>
              </div>
              <div class="plan-card__features">
                @for (feature of plan.features; track feature) {
                  <div class="plan-card__feature">
                    <mat-icon class="plan-card__feature-icon">check_circle</mat-icon>
                    <span>{{ feature }}</span>
                  </div>
                }
              </div>
              <div class="plan-card__action">
                @if (subscription?.plan === plan.value) {
                  <button mat-stroked-button disabled class="plan-card__button">
                    Current Plan
                  </button>
                } @else {
                  <button mat-flat-button color="primary" class="plan-card__button"
                          [disabled]="updating"
                          (click)="onUpgrade(plan.value)">
                    @if (updating && updatingPlan === plan.value) {
                      <mat-spinner diameter="20"></mat-spinner>
                    } @else if (subscription && plan.value > subscription.plan) {
                      Upgrade
                    } @else {
                      Switch Plan
                    }
                  </button>
                }
              </div>
            </mat-card>
          }
        </div>

      }
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 1200px;
    }

    .loading-container {
      display: flex;
      justify-content: center;
      padding: 64px 0;
    }

    .section-title {
      font-size: 18px;
      font-weight: 600;
      color: #263238;
      margin: 32px 0 16px;
    }

    /* Current Plan Card */
    .current-plan-card {
      margin-bottom: 8px;
    }

    .current-plan-card__content {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 8px;
      flex-wrap: wrap;
      gap: 16px;
    }

    .current-plan-card__label {
      font-size: 13px;
      color: #78909C;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      margin-bottom: 4px;
    }

    .current-plan-card__name {
      font-size: 22px;
      font-weight: 700;
      color: #263238;
    }

    .current-plan-card__status {
      margin-top: 4px;
    }

    .current-plan-card__period {
      display: flex;
      align-items: center;
      gap: 8px;
      color: #546E7A;
      font-size: 14px;
    }

    .status-badge {
      display: inline-block;
      padding: 2px 10px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
      text-transform: capitalize;
    }

    .status-badge--active {
      background: #E8F5E9;
      color: #2E7D32;
    }

    .status-badge--trialing {
      background: #E3F2FD;
      color: #1565C0;
    }

    .status-badge--past_due {
      background: #FFF3E0;
      color: #E65100;
    }

    .status-badge--canceled {
      background: #FFEBEE;
      color: #C62828;
    }

    /* Usage Grid */
    .usage-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 16px;
      margin-bottom: 8px;
    }

    .usage-card {
      padding: 8px;
    }

    .usage-card__header {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 12px;
    }

    .usage-card__icon {
      color: #546E7A;
      font-size: 20px;
      width: 20px;
      height: 20px;
    }

    .usage-card__label {
      font-size: 14px;
      font-weight: 500;
      color: #37474F;
    }

    .usage-card__values {
      display: flex;
      align-items: baseline;
      gap: 4px;
      margin-bottom: 12px;
    }

    .usage-card__current {
      font-size: 28px;
      font-weight: 700;
      color: #263238;
    }

    .usage-card__separator {
      font-size: 16px;
      color: #B0BEC5;
    }

    .usage-card__max {
      font-size: 16px;
      color: #78909C;
    }

    /* Plans Grid */
    .plans-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 24px;
      margin-bottom: 32px;
    }

    .plan-card {
      position: relative;
      padding: 0;
      display: flex;
      flex-direction: column;
      border: 2px solid transparent;
      transition: border-color 0.2s, box-shadow 0.2s;
    }

    .plan-card:hover {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }

    .plan-card--active {
      border-color: #26A69A;
    }

    .plan-card--highlighted:not(.plan-card--active) {
      border-color: #1565C0;
    }

    .plan-card__badge {
      position: absolute;
      top: -1px;
      right: 16px;
      background: #26A69A;
      color: #fff;
      padding: 4px 12px;
      font-size: 11px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      border-radius: 0 0 6px 6px;
    }

    .plan-card__badge--recommended {
      background: #1565C0;
    }

    .plan-card__header {
      padding: 24px 24px 16px;
    }

    .plan-card__name {
      font-size: 20px;
      font-weight: 700;
      color: #263238;
      margin: 0 0 4px;
    }

    .plan-card__price {
      font-size: 14px;
      font-weight: 600;
      color: #546E7A;
      margin-bottom: 8px;
    }

    .plan-card__description {
      font-size: 13px;
      color: #78909C;
      margin: 0;
      line-height: 1.5;
    }

    .plan-card__features {
      flex: 1;
      padding: 0 24px 16px;
      display: flex;
      flex-direction: column;
      gap: 10px;
    }

    .plan-card__feature {
      display: flex;
      align-items: flex-start;
      gap: 8px;
      font-size: 13px;
      color: #37474F;
      line-height: 1.4;
    }

    .plan-card__feature-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
      color: #26A69A;
      flex-shrink: 0;
      margin-top: 1px;
    }

    .plan-card__action {
      padding: 16px 24px 24px;
    }

    .plan-card__button {
      width: 100%;
    }

    /* Responsive */
    @media (max-width: 1024px) {
      .plans-grid {
        grid-template-columns: 1fr;
        max-width: 480px;
      }

      .usage-grid {
        grid-template-columns: 1fr;
      }
    }

    @media (min-width: 1025px) and (max-width: 1200px) {
      .plans-grid {
        gap: 16px;
      }
    }
  `]
})
export class SubscriptionComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly notification = inject(NotificationService);

  subscription: SubscriptionResponse | null = null;
  usage: SubscriptionUsageResponse | null = null;
  loading = true;
  updating = false;
  updatingPlan: number | null = null;

  plans: PlanCard[] = [
    {
      name: 'Starter',
      value: 0,
      price: 'Free',
      description: 'For small teams getting started with document management.',
      features: [
        'Up to 10 members',
        '1 GB storage',
        '25 cases per month',
        'Basic document tracking',
        'Email support'
      ],
      highlighted: false
    },
    {
      name: 'Professional',
      value: 1,
      price: '$49 / month',
      description: 'For growing organizations with advanced needs.',
      features: [
        'Up to 100 members',
        '25 GB storage',
        'Unlimited cases',
        'Document version history',
        'Hard copy chain-of-custody',
        'Priority email support',
        'Custom checklists'
      ],
      highlighted: true
    },
    {
      name: 'Enterprise',
      value: 2,
      price: 'Contact Sales',
      description: 'For large organizations requiring full platform capabilities.',
      features: [
        'Unlimited members',
        '500 GB storage',
        'Unlimited cases',
        'Advanced audit logging',
        'Custom branding',
        'SSO / SAML integration',
        'Dedicated support manager',
        'API access',
        'SLA guarantee'
      ],
      highlighted: false
    }
  ];

  ngOnInit(): void {
    this.loadData();
  }

  getPlanName(planValue: number): string {
    const plan = this.plans.find(p => p.value === planValue);
    return plan?.name ?? 'Unknown';
  }

  getProgressColor(percent: number): 'primary' | 'accent' | 'warn' {
    if (percent >= 90) return 'warn';
    if (percent >= 70) return 'accent';
    return 'primary';
  }

  getCasesPercent(): number {
    if (!this.usage || this.usage.maxCasesMonthly === 0) return 0;
    return (this.usage.currentCasesThisMonth / this.usage.maxCasesMonthly) * 100;
  }

  formatBytes(bytes: number): string {
    if (bytes === 0) return '0 B';
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];
    const k = 1024;
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + units[i];
  }

  onUpgrade(planValue: number): void {
    this.updating = true;
    this.updatingPlan = planValue;

    this.api.put<SubscriptionResponse>('subscription', { plan: planValue }).subscribe({
      next: (response) => {
        if (response.success) {
          this.subscription = response.data;
          this.notification.showSuccess('Subscription plan updated successfully.');
        }
        this.updating = false;
        this.updatingPlan = null;
      },
      error: () => {
        this.updating = false;
        this.updatingPlan = null;
      }
    });
  }

  private loadData(): void {
    this.loading = true;

    this.api.get<SubscriptionResponse>('subscription').subscribe({
      next: (response) => {
        if (response.success) {
          this.subscription = response.data;
        }
        this.loadUsage();
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  private loadUsage(): void {
    this.api.get<SubscriptionUsageResponse>('subscription/usage').subscribe({
      next: (response) => {
        if (response.success) {
          this.usage = response.data;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
