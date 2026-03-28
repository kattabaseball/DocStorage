import { Injectable, inject, NgZone } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, Observable } from 'rxjs';
import { SignalRService } from './signalr.service';

export interface AppNotification {
  id: string;
  title: string;
  message: string;
  type: 'info' | 'success' | 'warning' | 'error';
  isRead: boolean;
  createdAt: string;
  link?: string;
  entityType?: string;
  entityId?: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly snackBar = inject(MatSnackBar);
  private readonly signalRService = inject(SignalRService);
  private readonly ngZone = inject(NgZone);

  private readonly HUB_NAME = 'notifications';
  private notificationSubject = new Subject<AppNotification>();
  notification$: Observable<AppNotification> = this.notificationSubject.asObservable();

  async connect(): Promise<void> {
    this.signalRService.createConnection(this.HUB_NAME);
    this.signalRService.on<AppNotification>(this.HUB_NAME, 'ReceiveNotification', (notification) => {
      this.ngZone.run(() => {
        this.notificationSubject.next(notification);
        this.showToast(notification);
      });
    });
    await this.signalRService.startConnection(this.HUB_NAME);
  }

  async disconnect(): Promise<void> {
    await this.signalRService.stopConnection(this.HUB_NAME);
  }

  showSuccess(message: string, duration: number = 3000): void {
    this.snackBar.open(message, 'Close', {
      duration,
      panelClass: ['snackbar-success'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
  }

  showError(message: string, duration: number = 5000): void {
    this.snackBar.open(message, 'Close', {
      duration,
      panelClass: ['snackbar-error'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
  }

  showWarning(message: string, duration: number = 4000): void {
    this.snackBar.open(message, 'Close', {
      duration,
      panelClass: ['snackbar-warning'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
  }

  showInfo(message: string, duration: number = 3000): void {
    this.snackBar.open(message, 'Close', {
      duration,
      panelClass: ['snackbar-info'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
  }

  private showToast(notification: AppNotification): void {
    switch (notification.type) {
      case 'success':
        this.showSuccess(notification.message);
        break;
      case 'error':
        this.showError(notification.message);
        break;
      case 'warning':
        this.showWarning(notification.message);
        break;
      default:
        this.showInfo(notification.message);
        break;
    }
  }
}
