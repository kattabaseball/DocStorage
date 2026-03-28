import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { ApiService } from '@core/services/api.service';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'td-org-settings',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatCardModule, MatSlideToggleModule,
    MatSelectModule, MatProgressSpinnerModule, PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Organization Settings"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Organization'}, {label: 'Settings'}]">
      </td-page-header>

      <form [formGroup]="settingsForm" (ngSubmit)="onSubmit()" class="form-container">
        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">General</h3>
            <mat-form-field appearance="outline">
              <mat-label>Organization Name</mat-label>
              <input matInput formControlName="name">
            </mat-form-field>
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
            <mat-form-field appearance="outline">
              <mat-label>Address</mat-label>
              <textarea matInput formControlName="address" rows="2"></textarea>
            </mat-form-field>
          </div>
        </mat-card>

        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Preferences</h3>
            <mat-form-field appearance="outline">
              <mat-label>Default Language</mat-label>
              <mat-select formControlName="language">
                <mat-option value="en">English</mat-option>
                <mat-option value="fr">French</mat-option>
                <mat-option value="de">German</mat-option>
                <mat-option value="es">Spanish</mat-option>
              </mat-select>
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Timezone</mat-label>
              <mat-select formControlName="timezone">
                <mat-option value="UTC">UTC</mat-option>
                <mat-option value="America/New_York">Eastern Time</mat-option>
                <mat-option value="Europe/London">London</mat-option>
                <mat-option value="Europe/Berlin">Berlin</mat-option>
                <mat-option value="Asia/Dubai">Dubai</mat-option>
              </mat-select>
            </mat-form-field>
            <div class="toggle-row">
              <mat-slide-toggle formControlName="emailNotifications" color="primary">
                Email notifications for document uploads
              </mat-slide-toggle>
            </div>
            <div class="toggle-row">
              <mat-slide-toggle formControlName="expiryReminders" color="primary">
                Automatic expiry reminders (30, 14, 7 days)
              </mat-slide-toggle>
            </div>
          </div>
        </mat-card>

        <div class="form-actions">
          <button mat-flat-button color="primary" type="submit" [disabled]="settingsForm.invalid || saving">
            @if (saving) {
              <mat-spinner diameter="20"></mat-spinner>
            } @else {
              Save Settings
            }
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .toggle-row {
      margin-bottom: 16px;
    }
  `]
})
export class OrgSettingsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ApiService);
  private readonly notification = inject(NotificationService);

  settingsForm!: FormGroup;
  saving = false;

  ngOnInit(): void {
    this.settingsForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      address: [''],
      language: ['en'],
      timezone: ['UTC'],
      emailNotifications: [true],
      expiryReminders: [true]
    });

    this.api.get<any>('organization/settings').subscribe({
      next: (r) => { if (r.success) this.settingsForm.patchValue(r.data); }
    });
  }

  onSubmit(): void {
    if (this.settingsForm.valid) {
      this.saving = true;
      this.api.put('organization/settings', this.settingsForm.value).subscribe({
        next: () => { this.saving = false; this.notification.showSuccess('Settings saved'); },
        error: () => { this.saving = false; }
      });
    }
  }
}
