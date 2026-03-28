import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '@shared/components/status-badge/status-badge.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { DocumentUploadDialogComponent } from '@features/documents/document-upload/document-upload-dialog.component';
import { ApiService } from '@core/services/api.service';

interface MyDocument {
  id: string;
  title: string;
  type: string;
  status: string;
  expiryDate: string | null;
  uploadedAt: string;
}

@Component({
  selector: 'td-my-documents',
  standalone: true,
  imports: [
    CommonModule, MatTableModule, MatButtonModule, MatIconModule,
    MatDialogModule, MatProgressSpinnerModule,
    PageHeaderComponent, StatusBadgeComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="My Documents"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'My Documents'}]">
        <button mat-flat-button color="primary" (click)="openUpload()">
          <mat-icon>upload</mat-icon> Upload Document
        </button>
      </td-page-header>

      @if (loading) {
        <div class="flex-center" style="height: 300px;"><mat-spinner diameter="40"></mat-spinner></div>
      } @else if (documents.length === 0) {
        <td-empty-state icon="folder_open" title="No documents"
                        subtitle="Upload your documents to get started">
        </td-empty-state>
      } @else {
        <div class="td-table-container">
          <table mat-table [dataSource]="documents">
            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Document</th>
              <td mat-cell *matCellDef="let d">{{ d.title }}</td>
            </ng-container>
            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let d">{{ d.type }}</td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let d"><td-status-badge [status]="d.status" type="document"></td-status-badge></td>
            </ng-container>
            <ng-container matColumnDef="expiryDate">
              <th mat-header-cell *matHeaderCellDef>Expiry</th>
              <td mat-cell *matCellDef="let d">{{ d.expiryDate ? (d.expiryDate | date:'mediumDate') : 'N/A' }}</td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef style="width: 80px;"></th>
              <td mat-cell *matCellDef="let d">
                <button mat-icon-button><mat-icon>download</mat-icon></button>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="columns"></tr>
            <tr mat-row *matRowDef="let row; columns: columns;"></tr>
          </table>
        </div>
      }
    </div>
  `
})
export class MyDocumentsComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly dialog = inject(MatDialog);

  documents: MyDocument[] = [];
  loading = false;
  columns = ['title', 'type', 'status', 'expiryDate', 'actions'];

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.api.get<MyDocument[]>('portal/my-documents').subscribe({
      next: (r) => { if (r.success) this.documents = r.data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  openUpload(): void {
    const ref = this.dialog.open(DocumentUploadDialogComponent, { width: '600px', disableClose: true });
    ref.afterClosed().subscribe(result => { if (result) this.load(); });
  }
}
