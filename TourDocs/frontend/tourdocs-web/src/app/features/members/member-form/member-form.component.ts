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
import { MembersService } from '../members.service';

@Component({
  selector: 'td-member-form',
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
      <td-page-header [title]="isEditMode ? 'Edit Member' : 'Add New Member'"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Members', link: '/members'}, {label: isEditMode ? 'Edit' : 'New Member'}]">
      </td-page-header>

      <form [formGroup]="memberForm" (ngSubmit)="onSubmit()" class="form-container">
        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Personal Information</h3>
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>First Name</mat-label>
                <input matInput formControlName="firstName">
                @if (memberForm.get('firstName')?.hasError('required') && memberForm.get('firstName')?.touched) {
                  <mat-error>Required</mat-error>
                }
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Last Name</mat-label>
                <input matInput formControlName="lastName">
                @if (memberForm.get('lastName')?.hasError('required') && memberForm.get('lastName')?.touched) {
                  <mat-error>Required</mat-error>
                }
              </mat-form-field>
            </div>
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Email</mat-label>
                <input matInput formControlName="email" type="email">
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Phone</mat-label>
                <input matInput formControlName="phone">
              </mat-form-field>
            </div>
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Date of Birth</mat-label>
                <input matInput [matDatepicker]="dobPicker" formControlName="dateOfBirth">
                <mat-datepicker-toggle matIconSuffix [for]="dobPicker"></mat-datepicker-toggle>
                <mat-datepicker #dobPicker></mat-datepicker>
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Nationality</mat-label>
                <input matInput formControlName="nationality">
              </mat-form-field>
            </div>
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Passport Number</mat-label>
                <input matInput formControlName="passportNumber">
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Status</mat-label>
                <mat-select formControlName="status">
                  <mat-option value="Active">Active</mat-option>
                  <mat-option value="Inactive">Inactive</mat-option>
                  <mat-option value="Suspended">Suspended</mat-option>
                </mat-select>
              </mat-form-field>
            </div>
          </div>
        </mat-card>

        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Organization</h3>
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Department</mat-label>
                <input matInput formControlName="department">
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Position</mat-label>
                <input matInput formControlName="position">
              </mat-form-field>
            </div>
          </div>
        </mat-card>

        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Address</h3>
            <mat-form-field appearance="outline">
              <mat-label>Address</mat-label>
              <input matInput formControlName="address">
            </mat-form-field>
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>City</mat-label>
                <input matInput formControlName="city">
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Country</mat-label>
                <input matInput formControlName="country">
              </mat-form-field>
            </div>
          </div>
        </mat-card>

        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Emergency Contact</h3>
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Contact Name</mat-label>
                <input matInput formControlName="emergencyContactName">
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Contact Phone</mat-label>
                <input matInput formControlName="emergencyContactPhone">
              </mat-form-field>
            </div>
          </div>
        </mat-card>

        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Notes</h3>
            <mat-form-field appearance="outline">
              <mat-label>Additional Notes</mat-label>
              <textarea matInput formControlName="notes" rows="4"></textarea>
            </mat-form-field>
          </div>
        </mat-card>

        <div class="form-actions">
          <button mat-button type="button" routerLink="/members">Cancel</button>
          <button mat-flat-button color="primary" type="submit"
                  [disabled]="memberForm.invalid || saving">
            @if (saving) {
              <mat-spinner diameter="20"></mat-spinner>
            } @else {
              {{ isEditMode ? 'Update Member' : 'Create Member' }}
            }
          </button>
        </div>
      </form>
    </div>
  `
})
export class MemberFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly membersService = inject(MembersService);
  private readonly notification = inject(NotificationService);

  memberForm!: FormGroup;
  isEditMode = false;
  saving = false;
  memberId: string | null = null;

  ngOnInit(): void {
    this.isEditMode = this.route.snapshot.data['mode'] === 'edit';
    this.memberId = this.route.snapshot.paramMap.get('id');

    this.memberForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      dateOfBirth: [''],
      nationality: [''],
      passportNumber: [''],
      status: ['Active'],
      department: [''],
      position: [''],
      address: [''],
      city: [''],
      country: [''],
      emergencyContactName: [''],
      emergencyContactPhone: [''],
      notes: ['']
    });

    if (this.isEditMode && this.memberId) {
      this.membersService.getMember(this.memberId).subscribe({
        next: (response) => {
          if (response.success) {
            this.memberForm.patchValue(response.data);
          }
        }
      });
    }
  }

  onSubmit(): void {
    if (this.memberForm.valid) {
      this.saving = true;
      const data = this.memberForm.value;
      const request = this.isEditMode && this.memberId
        ? this.membersService.updateMember(this.memberId, data)
        : this.membersService.createMember(data);

      request.subscribe({
        next: () => {
          this.saving = false;
          this.notification.showSuccess(this.isEditMode ? 'Member updated successfully' : 'Member created successfully');
          this.router.navigate(['/members']);
        },
        error: () => {
          this.saving = false;
        }
      });
    }
  }
}
