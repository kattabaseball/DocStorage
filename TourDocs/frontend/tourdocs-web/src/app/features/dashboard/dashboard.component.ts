import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatCardComponent } from '@shared/components/stat-card/stat-card.component';
import { DocumentHealthWidgetComponent } from './widgets/document-health-widget/document-health-widget.component';
import { CaseReadinessWidgetComponent } from './widgets/case-readiness-widget/case-readiness-widget.component';
import { ExpiringDocsWidgetComponent } from './widgets/expiring-docs-widget/expiring-docs-widget.component';
import { RecentActivityWidgetComponent } from './widgets/recent-activity-widget/recent-activity-widget.component';
import { ApiService } from '@core/services/api.service';

interface DashboardSummary {
  totalMembers: number;
  activeCases: number;
  totalDocuments: number;
  pendingVerifications: number;
  expiringDocuments: number;
  pendingHardCopyRequests: number;
  pendingDocumentRequests: number;
  unreadNotifications: number;
  verifiedDocuments: number;
  uploadedDocuments: number;
  rejectedDocuments: number;
  expiredDocuments: number;
}

interface ExpiringDoc {
  documentId: string;
  title: string;
  memberName: string;
  expiryDate: string;
  daysUntilExpiry: number;
}

interface CaseReadiness {
  caseId: string;
  caseName: string;
  readyPercent: number;
  pendingPercent: number;
}

interface RecentActivity {
  id: string;
  action: string;
  entityType: string;
  entityId: string;
  userName: string;
  details: string;
  createdAt: string;
}

@Component({
  selector: 'td-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    PageHeaderComponent,
    StatCardComponent,
    DocumentHealthWidgetComponent,
    CaseReadinessWidgetComponent,
    ExpiringDocsWidgetComponent,
    RecentActivityWidgetComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Dashboard" [breadcrumbs]="[{label: 'Home', icon: 'home'}]"></td-page-header>

      <div class="grid-4col">
        <td-stat-card
          icon="people"
          label="Total Members"
          [value]="summary.totalMembers"
          color="primary">
        </td-stat-card>
        <td-stat-card
          icon="description"
          label="Documents"
          [value]="summary.totalDocuments"
          color="accent">
        </td-stat-card>
        <td-stat-card
          icon="verified"
          label="Verified"
          [value]="summary.verifiedDocuments"
          color="success">
        </td-stat-card>
        <td-stat-card
          icon="event_busy"
          label="Expiring Soon"
          [value]="summary.expiringDocuments"
          color="warn">
        </td-stat-card>
      </div>

      <div class="grid-2col mt-24">
        <td-document-health-widget [summary]="summary"></td-document-health-widget>
        <td-case-readiness-widget [cases]="caseReadiness"></td-case-readiness-widget>
      </div>

      <div class="grid-2col mt-24">
        <td-expiring-docs-widget [documents]="expiringDocs"></td-expiring-docs-widget>
        <td-recent-activity-widget [activities]="recentActivities"></td-recent-activity-widget>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  private readonly api = inject(ApiService);

  summary: DashboardSummary = {
    totalMembers: 0, activeCases: 0, totalDocuments: 0,
    pendingVerifications: 0, expiringDocuments: 0,
    pendingHardCopyRequests: 0, pendingDocumentRequests: 0,
    unreadNotifications: 0, verifiedDocuments: 0,
    uploadedDocuments: 0, rejectedDocuments: 0, expiredDocuments: 0
  };

  expiringDocs: ExpiringDoc[] = [];
  caseReadiness: CaseReadiness[] = [];
  recentActivities: RecentActivity[] = [];

  ngOnInit(): void {
    this.api.get<any>('dashboard/summary').subscribe({
      next: (r) => { if (r.success) this.summary = r.data; }
    });

    this.api.get<any>('dashboard/expiring-documents', { daysAhead: '30' }).subscribe({
      next: (r) => { if (r.success) this.expiringDocs = r.data; }
    });

    this.api.get<any>('dashboard/case-readiness').subscribe({
      next: (r) => { if (r.success) this.caseReadiness = r.data; }
    });

    this.api.get<any>('dashboard/recent-activity', { count: '5' }).subscribe({
      next: (r) => { if (r.success) this.recentActivities = r.data; }
    });
  }
}
