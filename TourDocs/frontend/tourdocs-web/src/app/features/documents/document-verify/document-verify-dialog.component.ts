import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { DocumentsService } from '../documents.service';
import { NotificationService } from '@core/services/notification.service';

interface VerifyDialogData {
  documentId: string;
  documentTitle: string;
}

@Component({
  selector: 'td-document-verify-dialog',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatDialogModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatRadioModule
  ],
  template: `
    <h2 mat-dialog-title>Review Document</h2>
    <mat-dialog-content>
      <p class="review-doc-name">
        <mat-icon>description</mat-icon>
        {{ data.documentTitle }}
      </p>

      <form [formGroup]="verifyForm">
        <div class="review-decision">
          <mat-radio-group formControlName="approved">
            <mat-radio-button [value]="true" color="primary">
              <span class="decision-label decision-label--approve">
                <mat-icon>check_circle</mat-icon> Approve
              </span>
            </mat-radio-button>
            <mat-radio-button [value]="false" color="warn">
              <span class="decision-label decision-label--reject">
                <mat-icon>cancel</mat-icon> Reject
              </span>
            </mat-radio-button>
          </mat-radio-group>
        </div>

        @if (verifyForm.get('approved')?.value === false) {
          <mat-form-field appearance="outline">
            <mat-label>Rejection Reason</mat-label>
            <textarea matInput formControlName="reason" rows="3"
                      placeholder="Please provide a reason for rejection..."></textarea>
          </mat-form-field>
        }
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button
              [color]="verifyForm.get('approved')?.value ? 'primary' : 'warn'"
              (click)="submit()"
              [disabled]="submitting">
        {{ verifyForm.get('approved')?.value ? 'Approve' : 'Reject' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .review-doc-name {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 15px;
      font-weight: 500;
      color: #263238;
      padding: 12px 16px;
      background: #F5F5F5;
      border-radius: 8px;
      margin-bottom: 24px;
    }

    .review-decision {
      margin-bottom: 24px;

      mat-radio-group {
        display: flex;
        gap: 24px;
      }
    }

    .decision-label {
      display: flex;
      align-items: center;
      gap: 6px;
      font-weight: 500;
    }

    .decision-label--approve mat-icon { color: #66BB6A; }
    .decision-label--reject mat-icon { color: #EF5350; }

    mat-form-field { width: 100%; }
  `]
})
export class DocumentVerifyDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<DocumentVerifyDialogComponent>);
  readonly data = inject<VerifyDialogData>(MAT_DIALOG_DATA);
  private readonly documentsService = inject(DocumentsService);
  private readonly notification = inject(NotificationService);

  verifyForm!: FormGroup;
  submitting = false;

  ngOnInit(): void {
    this.verifyForm = this.fb.group({
      approved: [true],
      reason: ['']
    });
  }

  submit(): void {
    this.submitting = true;
    const formValue = this.verifyForm.value;
    this.documentsService.verifyDocument({
      documentId: this.data.documentId,
      approved: formValue.approved,
      reason: formValue.reason
    }).subscribe({
      next: () => {
        this.submitting = false;
        this.notification.showSuccess(formValue.approved ? 'Document approved' : 'Document rejected');
        this.dialogRef.close(true);
      },
      error: () => {
        this.submitting = false;
      }
    });
  }
}
