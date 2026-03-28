import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppState } from '@store/app.state';
import { selectUser } from '@store/auth/auth.selectors';
import { AuthActions } from '@store/auth/auth.actions';
import { UserProfile } from '@core/auth/auth.models';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';

@Component({
  selector: 'td-user-menu',
  standalone: true,
  imports: [CommonModule, RouterModule, MatMenuModule, MatIconModule, MatButtonModule, MatDividerModule, AvatarComponent],
  template: `
    @if (user$ | async; as user) {
      <button mat-button [matMenuTriggerFor]="userMenu" class="user-menu-trigger">
        <td-avatar [name]="user.fullName" [imageUrl]="user.avatarUrl" size="sm"></td-avatar>
        <span class="user-menu-trigger__name">{{ user.fullName }}</span>
        <mat-icon>arrow_drop_down</mat-icon>
      </button>

      <mat-menu #userMenu="matMenu" xPosition="before" class="user-menu-panel">
        <div class="user-menu__header" (click)="$event.stopPropagation()">
          <td-avatar [name]="user.fullName" [imageUrl]="user.avatarUrl" size="lg"></td-avatar>
          <div class="user-menu__info">
            <span class="user-menu__name">{{ user.fullName }}</span>
            <span class="user-menu__email">{{ user.email }}</span>
          </div>
        </div>
        <mat-divider></mat-divider>
        <button mat-menu-item routerLink="/profile">
          <mat-icon>person</mat-icon>
          <span>My Profile</span>
        </button>
        <button mat-menu-item routerLink="/organization/settings">
          <mat-icon>settings</mat-icon>
          <span>Settings</span>
        </button>
        <mat-divider></mat-divider>
        <button mat-menu-item (click)="logout()">
          <mat-icon>logout</mat-icon>
          <span>Logout</span>
        </button>
      </mat-menu>
    }
  `,
  styles: [`
    .user-menu-trigger {
      display: flex;
      align-items: center;
      gap: 8px;
      text-transform: none;
      font-weight: 500;
    }

    .user-menu-trigger__name {
      max-width: 120px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;

      @media (max-width: 768px) {
        display: none;
      }
    }

    .user-menu__header {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 16px;
    }

    .user-menu__info {
      display: flex;
      flex-direction: column;
    }

    .user-menu__name {
      font-size: 14px;
      font-weight: 600;
      color: #263238;
    }

    .user-menu__email {
      font-size: 12px;
      color: #78909C;
    }
  `]
})
export class UserMenuComponent {
  private readonly store = inject(Store<AppState>);
  user$: Observable<UserProfile | null> = this.store.select(selectUser);

  logout(): void {
    this.store.dispatch(AuthActions.logout());
  }
}
