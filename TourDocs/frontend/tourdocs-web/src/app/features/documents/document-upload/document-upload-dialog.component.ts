import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { FileUploadComponent, SelectedFile } from '@shared/components/file-upload/file-upload.component';
import { FileUploadService, DocumentUploadMetadata } from '@core/services/file-upload.service';
import { NotificationService } from '@core/services/notification.service';
import { ApiService } from '@core/services/api.service';

interface MemberOption {
  id: string;
  fullName: string;
}

@Component({
  selector: 'td-document-upload-dialog',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatDialogModule,
    MatFormFieldModule, MatInputModule, MatSelectModule,
    MatDatepickerModule, MatNativeDateModule, MatButtonModule,
    MatIconModule, MatProgressBarModule, MatChipsModule, FileUploadComponent
  ],
  template: `
    <h2 mat-dialog-title>Upload Document{{ selectedFiles.length > 1 ? 's' : '' }}</h2>
    <mat-dialog-content>
      <td-file-upload accept=".pdf,.jpg,.jpeg,.png,.doc,.docx"
                      [maxSizeMB]="25"
                      [multiple]="true"
                      (filesSelected)="onFilesSelected($event)">
      </td-file-upload>

      @if (selectedFiles.length > 1) {
        <div class="file-list">
          @for (file of selectedFiles; track file.file.name) {
            <div class="file-list__item">
              <mat-icon class="file-list__icon">description</mat-icon>
              <span class="file-list__name">{{ file.file.name }}</span>
              <span class="file-list__size">{{ formatFileSize(file.file.size) }}</span>
            </div>
          }
        </div>
      }

      <form [formGroup]="uploadForm" class="mt-16">
        <mat-form-field appearance="outline">
          <mat-label>Member</mat-label>
          <mat-select formControlName="memberId">
            @if (members.length === 0) {
              <mat-option disabled>No members found — add members first</mat-option>
            }
            @for (member of members; track member.id) {
              <mat-option [value]="member.id">{{ member.fullName }}</mat-option>
            }
          </mat-select>
          @if (uploadForm.get('memberId')?.hasError('required') && uploadForm.get('memberId')?.touched) {
            <mat-error>Member is required</mat-error>
          }
          @if (members.length === 0) {
            <mat-hint>Go to Members > Add Member first</mat-hint>
          }
        </mat-form-field>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Document Type</mat-label>
            <mat-select formControlName="documentTypeId">
              <mat-option value="Passport">Passport</mat-option>
              <mat-option value="Visa">Visa</mat-option>
              <mat-option value="MedicalCertificate">Medical Certificate</mat-option>
              <mat-option value="InsurancePolicy">Insurance Policy</mat-option>
              <mat-option value="License">License</mat-option>
              <mat-option value="WorkPermit">Work Permit</mat-option>
              <mat-option value="BankStatement">Bank Statement</mat-option>
              <mat-option value="Other">Other</mat-option>
            </mat-select>
            @if (uploadForm.get('documentTypeId')?.hasError('required') && uploadForm.get('documentTypeId')?.touched) {
              <mat-error>Document type is required</mat-error>
            }
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Category</mat-label>
            <mat-select formControlName="categoryId">
              <mat-option value="Identity">Identity</mat-option>
              <mat-option value="Travel">Travel</mat-option>
              <mat-option value="Medical">Medical</mat-option>
              <mat-option value="Financial">Financial</mat-option>
              <mat-option value="Legal">Legal</mat-option>
              <mat-option value="Professional">Professional</mat-option>
              <mat-option value="Photos">Photos</mat-option>
            </mat-select>
            @if (uploadForm.get('categoryId')?.hasError('required') && uploadForm.get('categoryId')?.touched) {
              <mat-error>Category is required</mat-error>
            }
          </mat-form-field>
        </div>

        <mat-form-field appearance="outline">
          <mat-label>Expiry Date (optional)</mat-label>
          <input matInput [matDatepicker]="expiryPicker" formControlName="expiryDate">
          <mat-datepicker-toggle matIconSuffix [for]="expiryPicker"></mat-datepicker-toggle>
          <mat-datepicker #expiryPicker></mat-datepicker>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Notes (optional)</mat-label>
          <textarea matInput formControlName="notes" rows="3"></textarea>
        </mat-form-field>
      </form>

      @if (uploading) {
        <div class="upload-progress">
          <p class="upload-progress__text">
            Uploading {{ currentUploadIndex + 1 }} of {{ selectedFiles.length }}...
          </p>
          <mat-progress-bar mode="determinate" [value]="uploadProgress"></mat-progress-bar>
        </div>
      }
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close [disabled]="uploading">Cancel</button>
      <button mat-flat-button color="primary" (click)="upload()"
              [disabled]="uploadForm.invalid || selectedFiles.length === 0 || uploading">
        {{ uploading ? 'Uploading...' : selectedFiles.length > 1 ? 'Upload ' + selectedFiles.length + ' Files' : 'Upload' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    mat-form-field { width: 100%; }
    .form-row {
      display: flex;
      gap: 12px;
    }

    .file-list {
      margin: 12px 0;
      padding: 12px;
      background: #F5F5F5;
      border-radius: 8px;
      max-height: 120px;
      overflow-y: auto;
    }

    .file-list__item {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 4px 0;
      font-size: 13px;
    }

    .file-list__icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
      color: #546E7A;
    }

    .file-list__name {
      flex: 1;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .file-list__size {
      color: #90A4AE;
      font-size: 12px;
      flex-shrink: 0;
    }

    .upload-progress {
      margin-top: 16px;
    }

    .upload-progress__text {
      font-size: 13px;
      color: #546E7A;
      margin-bottom: 8px;
    }
  `]
})
export class DocumentUploadDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<DocumentUploadDialogComponent>);
  private readonly uploadService = inject(FileUploadService);
  private readonly notification = inject(NotificationService);
  private readonly api = inject(ApiService);

  uploadForm!: FormGroup;
  selectedFiles: SelectedFile[] = [];
  members: MemberOption[] = [];
  uploading = false;
  uploadProgress = 0;
  currentUploadIndex = 0;

  ngOnInit(): void {
    this.uploadForm = this.fb.group({
      memberId: ['', Validators.required],
      documentTypeId: ['', Validators.required],
      categoryId: ['', Validators.required],
      expiryDate: [''],
      notes: ['']
    });

    this.loadMembers();
  }

  loadMembers(): void {
    this.api.get<any>('members', { PageSize: '200', PageNumber: '1' }).subscribe({
      next: (r) => {
        if (r.success && r.data?.items) {
          this.members = r.data.items.map((m: any) => ({
            id: m.id,
            fullName: m.fullName || `${m.legalFirstName} ${m.legalLastName}`.trim() || m.email || 'Unknown'
          }));
        }
      },
      error: () => {
        this.notification.showError('Failed to load members. Please add members first.');
      }
    });
  }

  onFilesSelected(files: SelectedFile[]): void {
    this.selectedFiles = files;
  }

  async upload(): Promise<void> {
    if (this.uploadForm.invalid || this.selectedFiles.length === 0) return;

    this.uploading = true;
    const formValue = this.uploadForm.value;
    const metadata: DocumentUploadMetadata = {
      memberId: formValue.memberId,
      documentTypeId: formValue.documentTypeId,
      categoryId: formValue.categoryId,
      expiryDate: formValue.expiryDate ? new Date(formValue.expiryDate).toISOString() : undefined,
      notes: formValue.notes
    };

    let successCount = 0;
    for (let i = 0; i < this.selectedFiles.length; i++) {
      this.currentUploadIndex = i;
      const file = this.selectedFiles[i].file;

      try {
        await new Promise<void>((resolve, reject) => {
          this.uploadService.uploadDocument(file, metadata).subscribe({
            next: (progress) => {
              this.uploadProgress = progress.progress;
              if (progress.status === 'complete') {
                successCount++;
                resolve();
              }
            },
            error: (err) => reject(err)
          });
        });
      } catch {
        this.notification.showError(`Failed to upload ${file.name}`);
      }
    }

    this.uploading = false;
    if (successCount > 0) {
      const msg = successCount === 1
        ? 'Document uploaded successfully'
        : `${successCount} documents uploaded successfully`;
      this.notification.showSuccess(msg);
      this.dialogRef.close(true);
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1048576) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / 1048576).toFixed(1) + ' MB';
  }
}
