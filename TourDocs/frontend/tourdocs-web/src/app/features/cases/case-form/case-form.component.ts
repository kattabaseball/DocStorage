import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { NotificationService } from '@core/services/notification.service';
import { CasesService } from '../cases.service';

@Component({
  selector: 'td-case-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule,
    MatFormFieldModule, MatInputModule, MatSelectModule,
    MatDatepickerModule, MatNativeDateModule, MatButtonModule,
    MatIconModule, MatCardModule, MatProgressSpinnerModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header [title]="isEdit ? 'Edit Case' : 'Create Case'"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Cases', link: '/cases'}, {label: isEdit ? 'Edit' : 'Create'}]">
      </td-page-header>

      <form [formGroup]="caseForm" (ngSubmit)="onSubmit()" class="form-container">
        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Case Information</h3>
            <mat-form-field appearance="outline">
              <mat-label>Case Name</mat-label>
              <input matInput formControlName="name" placeholder="e.g., Europe Trip 2026">
              @if (caseForm.get('name')?.hasError('required') && caseForm.get('name')?.touched) {
                <mat-error>Case name is required</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" rows="4"></textarea>
            </mat-form-field>

            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Destination</mat-label>
                <input matInput formControlName="destination">
                @if (caseForm.get('destination')?.hasError('required') && caseForm.get('destination')?.touched) {
                  <mat-error>Destination is required</mat-error>
                }
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Status</mat-label>
                <mat-select formControlName="status">
                  <mat-option value="Draft">Draft</mat-option>
                  <mat-option value="Active">Active</mat-option>
                  <mat-option value="Ready">Ready</mat-option>
                  <mat-option value="InProgress">In Progress</mat-option>
                  <mat-option value="Completed">Completed</mat-option>
                  <mat-option value="Cancelled">Cancelled</mat-option>
                </mat-select>
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Departure Date</mat-label>
                <input matInput [matDatepicker]="depPicker" formControlName="departureDate">
                <mat-datepicker-toggle matIconSuffix [for]="depPicker"></mat-datepicker-toggle>
                <mat-datepicker #depPicker></mat-datepicker>
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Return Date</mat-label>
                <input matInput [matDatepicker]="retPicker" formControlName="returnDate">
                <mat-datepicker-toggle matIconSuffix [for]="retPicker"></mat-datepicker-toggle>
                <mat-datepicker #retPicker></mat-datepicker>
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline">
              <mat-label>Checklist</mat-label>
              <mat-select formControlName="checklistId">
                <mat-option value="cl-schengen">Schengen Countries</mat-option>
                <mat-option value="cl-usa">United States</mat-option>
                <mat-option value="cl-uk">United Kingdom</mat-option>
                <mat-option value="cl-generic">Generic International</mat-option>
              </mat-select>
            </mat-form-field>
          </div>
        </mat-card>

        <div class="form-actions">
          <button mat-button type="button" routerLink="/cases">Cancel</button>
          <button mat-flat-button color="primary" type="submit"
                  [disabled]="caseForm.invalid || saving">
            @if (saving) {
              <mat-spinner diameter="20"></mat-spinner>
            } @else {
              {{ isEdit ? 'Update Case' : 'Create Case' }}
            }
          </button>
        </div>
      </form>
    </div>
  `
})
export class CaseFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly casesService = inject(CasesService);
  private readonly notification = inject(NotificationService);

  caseForm!: FormGroup;
  isEdit = false;
  saving = false;
  caseId: string | null = null;

  ngOnInit(): void {
    this.isEdit = this.route.snapshot.data['mode'] === 'edit';
    this.caseId = this.route.snapshot.paramMap.get('id');

    this.caseForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      destination: ['', Validators.required],
      status: ['Draft'],
      departureDate: [''],
      returnDate: [''],
      checklistId: ['']
    });

    if (this.isEdit && this.caseId) {
      this.casesService.getCase(this.caseId).subscribe({
        next: (r) => { if (r.success) this.caseForm.patchValue(r.data); }
      });
    }
  }

  onSubmit(): void {
    if (this.caseForm.valid) {
      this.saving = true;
      const data = this.caseForm.value;
      const req = this.isEdit && this.caseId
        ? this.casesService.updateCase(this.caseId, data)
        : this.casesService.createCase(data);

      req.subscribe({
        next: () => {
          this.saving = false;
          this.notification.showSuccess(this.isEdit ? 'Case updated' : 'Case created');
          this.router.navigate(['/cases']);
        },
        error: () => {
          this.saving = false;
          this.notification.showError('Failed to save case. Please try again.');
        }
      });
    }
  }
}
