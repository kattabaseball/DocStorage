import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '@core/auth/auth.service';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'td-forgot-password',
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
          <h1>TourDocs</h1>
        </div>

        @if (!emailSent) {
          <h2 class="auth-card__title">Forgot Password</h2>
          <p class="auth-card__description">
            Enter your email address and we'll send you a link to reset your password.
          </p>

          <form [formGroup]="forgotForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" type="email">
              <mat-icon matIconPrefix>email</mat-icon>
              @if (forgotForm.get('email')?.hasError('required') && forgotForm.get('email')?.touched) {
                <mat-error>Email is required</mat-error>
              }
              @if (forgotForm.get('email')?.hasError('email') && forgotForm.get('email')?.touched) {
                <mat-error>Please enter a valid email</mat-error>
              }
            </mat-form-field>

            <button mat-flat-button color="primary" type="submit"
                    class="auth-form__submit"
                    [disabled]="forgotForm.invalid || loading">
              @if (loading) {
                <mat-spinner diameter="20"></mat-spinner>
              } @else {
                Send Reset Link
              }
            </button>
          </form>
        } @else {
          <div class="auth-card__success">
            <mat-icon class="success-icon">mark_email_read</mat-icon>
            <h2 class="auth-card__title">Check Your Email</h2>
            <p class="auth-card__description">
              We've sent a password reset link to <strong>{{ forgotForm.get('email')?.value }}</strong>.
              Please check your inbox and follow the instructions.
            </p>
          </div>
        }

        <div class="auth-card__footer">
          <a routerLink="/auth/login">Back to Sign In</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .auth-card__description {
      font-size: 14px;
      color: #546E7A;
      text-align: center;
      margin: 0 0 24px;
      line-height: 1.6;
    }

    mat-form-field { width: 100%; }

    .auth-form__submit {
      width: 100%;
      height: 48px;
      font-size: 16px;
      font-weight: 600;
    }

    .auth-card__success {
      text-align: center;
    }

    .success-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: #66BB6A;
      margin-bottom: 16px;
    }
  `]
})
export class ForgotPasswordComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);

  forgotForm!: FormGroup;
  loading = false;
  emailSent = false;

  ngOnInit(): void {
    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotForm.valid) {
      this.loading = true;
      this.authService.forgotPassword(this.forgotForm.value).subscribe({
        next: () => {
          this.emailSent = true;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.notificationService.showError('Failed to send reset link. Please try again.');
        }
      });
    }
  }
}
