import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { SearchInputComponent } from '@shared/components/search-input/search-input.component';
import { DocumentCardComponent, DocumentCardData } from '@shared/components/document-card/document-card.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { DocumentsService } from '../documents.service';
import { DocumentUploadDialogComponent } from '../document-upload/document-upload-dialog.component';
import { DocumentVerifyDialogComponent } from '../document-verify/document-verify-dialog.component';
import { Document } from '../documents.models';
import { DEFAULT_PAGINATION, QueryParams } from '@core/models/pagination.model';

@Component({
  selector: 'td-document-vault',
  standalone: true,
  imports: [
    CommonModule, MatButtonModule, MatIconModule, MatSelectModule, MatFormFieldModule,
    MatDialogModule, MatProgressSpinnerModule, MatPaginatorModule,
    PageHeaderComponent, SearchInputComponent, DocumentCardComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Document Vault"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Documents'}]">
        <button mat-flat-button color="primary" (click)="openUploadDialog()">
          <mat-icon>upload</mat-icon>
          Upload Document
        </button>
      </td-page-header>

      <div class="vault-toolbar">
        <td-search-input placeholder="Search documents..." (searchChange)="onSearch($event)"></td-search-input>
        <mat-form-field appearance="outline" class="filter-select">
          <mat-label>Category</mat-label>
          <mat-select [(value)]="selectedCategory" (selectionChange)="onCategoryChange()">
            <mat-option value="">All Categories</mat-option>
            <mat-option value="passport">Passport</mat-option>
            <mat-option value="visa">Visa</mat-option>
            <mat-option value="medical">Medical</mat-option>
            <mat-option value="insurance">Insurance</mat-option>
            <mat-option value="license">License</mat-option>
            <mat-option value="certificate">Certificate</mat-option>
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline" class="filter-select">
          <mat-label>Status</mat-label>
          <mat-select [(value)]="selectedStatus" (selectionChange)="onStatusChange()">
            <mat-option value="">All Statuses</mat-option>
            <mat-option value="Uploaded">Uploaded</mat-option>
            <mat-option value="Reviewing">Reviewing</mat-option>
            <mat-option value="Verified">Verified</mat-option>
            <mat-option value="Rejected">Rejected</mat-option>
            <mat-option value="Expired">Expired</mat-option>
          </mat-select>
        </mat-form-field>
      </div>

      @if (loading) {
        <div class="flex-center" style="height: 300px;">
          <mat-spinner diameter="40"></mat-spinner>
        </div>
      } @else if (documents.length === 0) {
        <td-empty-state icon="folder_open"
                        title="No documents found"
                        subtitle="Upload your first document to get started"
                        actionText="Upload Document">
        </td-empty-state>
      } @else {
        @for (group of groupedDocuments; track group.category) {
          <div class="document-group">
            <h3 class="document-group__title">{{ group.category }}</h3>
            <div class="grid-3col">
              @for (doc of group.docs; track doc.id) {
                <td-document-card [document]="doc"
                                  (view)="onView($event)"
                                  (download)="onDownload($event)"
                                  (verify)="onVerify($event)">
                </td-document-card>
              }
            </div>
          </div>
        }

        <mat-paginator [length]="totalCount"
                       [pageSize]="pageSize"
                       [pageSizeOptions]="[25, 50, 100]"
                       (page)="onPage($event)"
                       showFirstLastButtons>
        </mat-paginator>
      }
    </div>
  `,
  styles: [`
    .vault-toolbar {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 24px;
      flex-wrap: wrap;
    }

    .filter-select {
      width: 180px;

      ::ng-deep .mat-mdc-form-field-subscript-wrapper {
        display: none;
      }
    }

    .document-group {
      margin-bottom: 32px;
    }

    .document-group__title {
      font-size: 18px;
      font-weight: 600;
      color: #263238;
      margin-bottom: 16px;
      padding-bottom: 8px;
      border-bottom: 2px solid #E3F2FD;
    }
  `]
})
export class DocumentVaultComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly documentsService = inject(DocumentsService);
  private readonly dialog = inject(MatDialog);

  documents: Document[] = [];
  groupedDocuments: { category: string; docs: DocumentCardData[] }[] = [];
  loading = false;
  selectedCategory = '';
  selectedStatus = '';
  searchQuery = '';
  totalCount = 0;
  pageSize = 25;
  currentPage = 0;

  ngOnInit(): void {
    this.loadDocuments();
  }

  loadDocuments(): void {
    this.loading = true;
    const params: QueryParams = { ...DEFAULT_PAGINATION, pageSize: this.pageSize, pageNumber: this.currentPage + 1 };
    if (this.searchQuery) params.search = this.searchQuery;

    this.documentsService.getDocuments(params).subscribe({
      next: (response) => {
        if (response.success) {
          this.documents = response.data.items;
          this.totalCount = response.data.totalCount;
          this.groupDocuments();
        }
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  onSearch(query: string): void {
    this.searchQuery = query;
    this.currentPage = 0;
    this.loadDocuments();
  }

  onPage(event: PageEvent): void {
    this.currentPage = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadDocuments();
  }

  onCategoryChange(): void {
    this.groupDocuments();
  }

  onStatusChange(): void {
    this.groupDocuments();
  }

  openUploadDialog(): void {
    const dialogRef = this.dialog.open(DocumentUploadDialogComponent, {
      width: '600px',
      maxHeight: '90vh',
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadDocuments();
    });
  }

  onView(doc: DocumentCardData): void {
    window.open(`/documents/${doc.id}`, '_blank');
  }

  onDownload(doc: DocumentCardData): void {
    const foundDoc = this.documents.find(d => d.id === doc.id);
    if (!foundDoc) return;

    this.documentsService.downloadDocument(foundDoc.id).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = foundDoc.fileName || foundDoc.title;
        anchor.click();
        URL.revokeObjectURL(url);
      },
      error: () => {
        // Fallback: open the file URL directly if download endpoint fails
        window.open(foundDoc.fileUrl, '_blank');
      }
    });
  }

  onVerify(doc: DocumentCardData): void {
    const dialogRef = this.dialog.open(DocumentVerifyDialogComponent, {
      width: '500px',
      data: { documentId: doc.id, documentTitle: doc.title }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadDocuments();
    });
  }

  private groupDocuments(): void {
    let filtered = this.documents;
    if (this.selectedCategory) {
      filtered = filtered.filter(d => d.category.toLowerCase() === this.selectedCategory);
    }
    if (this.selectedStatus) {
      filtered = filtered.filter(d => d.status === this.selectedStatus);
    }

    const groups = new Map<string, DocumentCardData[]>();
    filtered.forEach(doc => {
      const category = doc.category || 'Other';
      if (!groups.has(category)) groups.set(category, []);
      groups.get(category)!.push({
        id: doc.id,
        title: doc.title,
        type: doc.documentType,
        category: doc.category,
        status: doc.status,
        expiryDate: doc.expiryDate || undefined,
        uploadedDate: doc.uploadedAt || doc.createdAt,
        memberName: doc.memberName
      });
    });

    this.groupedDocuments = Array.from(groups.entries()).map(([category, docs]) => ({ category, docs }));
  }
}
