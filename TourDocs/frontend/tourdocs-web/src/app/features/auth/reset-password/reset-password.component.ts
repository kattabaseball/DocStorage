import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ApiService } from '@core/services/api.service';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'td-reset-password',
  standalone: true,
  imports: [
    CommonModule, RouterModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatProgressSpinnerModule
  ],
  template: `
    <div class="auth-form-container">
      <div class="auth-card">
        <div class="auth-card__logo">
          <mat-icon class="auth-card__icon">description</mat-icon>
          <h1>TourDocs</h1>
        </div>

        @if (success) {
          <div class="success-message">
            <mat-icon class="success-icon">check_circle</mat-icon>
            <h2>Password Reset</h2>
            <p>Your password has been successfully reset.</p>
            <a mat-flat-button color="primary" routerLink="/auth/login">Sign In</a>
          </div>
        } @else {
          <h2>Reset Password</h2>
          <p class="text-muted mb-16">Enter your new password below.</p>

          @if (error) {
            <div class="error-banner mb-16">{{ error }}</div>
          }

          <form [formGroup]="resetForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline">
              <mat-label>New Password</mat-label>
              <input matInput [type]="hidePassword ? 'password' : 'text'" formControlName="newPassword">
              <button mat-icon-button matIconSuffix type="button" (click)="hidePassword = !hidePassword">
                <mat-icon>{{ hidePassword ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
              @if (resetForm.get('newPassword')?.hasError('required') && resetForm.get('newPassword')?.touched) {
                <mat-error>Password is required</mat-error>
              }
              @if (resetForm.get('newPassword')?.hasError('minlength') && resetForm.get('newPassword')?.touched) {
                <mat-error>Minimum 8 characters</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Confirm Password</mat-label>
              <input matInput [type]="hideConfirm ? 'password' : 'text'" formControlName="confirmPassword">
              <button mat-icon-button matIconSuffix type="button" (click)="hideConfirm = !hideConfirm">
                <mat-icon>{{ hideConfirm ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
              @if (resetForm.get('confirmPassword')?.touched && resetForm.hasError('passwordMismatch')) {
                <mat-error>Passwords do not match</mat-error>
              }
            </mat-form-field>

            <button mat-flat-button color="primary" type="submit" class="full-width"
                    [disabled]="resetForm.invalid || loading">
              @if (loading) {
                <mat-spinner diameter="20"></mat-spinner>
              } @else {
                Reset Password
              }
            </button>
          </form>
        }

        <div class="auth-card__footer mt-16">
          <a routerLink="/auth/login">Back to Sign In</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    mat-form-field { width: 100%; }
    .full-width { width: 100%; }
    .success-message { text-align: center; padding: 24px 0; }
    .success-icon { font-size: 48px; width: 48px; height: 48px; color: #66BB6A; margin-bottom: 16px; }
    .error-banner { background: #FFEBEE; color: #C62828; padding: 12px 16px; border-radius: 8px; font-size: 14px; }
  `]
})
export class ResetPasswordComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ApiService);
  private readonly notification = inject(NotificationService);

  resetForm!: FormGroup;
  hidePassword = true;
  hideConfirm = true;
  loading = false;
  success = false;
  error: string | null = null;
  private token = '';
  private email = '';

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('token') || '';
    this.email = this.route.snapshot.queryParamMap.get('email') || '';

    this.resetForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    const password = g.get('newPassword')?.value;
    const confirm = g.get('confirmPassword')?.value;
    return password === confirm ? null : { passwordMismatch: true };
  }

  onSubmit(): void {
    if (this.resetForm.invalid) return;
    this.loading = true;
    this.error = null;

    this.api.post<any>('auth/reset-password', {
      email: this.email,
      token: this.token,
      newPassword: this.resetForm.value.newPassword
    }).subscribe({
      next: () => {
        this.loading = false;
        this.success = true;
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message || 'Failed to reset password. The link may have expired.';
      }
    });
  }
}
