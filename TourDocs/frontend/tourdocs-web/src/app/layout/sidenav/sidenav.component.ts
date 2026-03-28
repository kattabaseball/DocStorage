import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppState } from '@store/app.state';
import { selectOrganizationName } from '@store/auth/auth.selectors';
import { selectUnreadCount } from '@store/notifications/notifications.selectors';
import { NavItemComponent } from './nav-item/nav-item.component';
import { NavGroupComponent } from './nav-group/nav-group.component';

@Component({
  selector: 'td-sidenav',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatDividerModule, NavItemComponent, NavGroupComponent],
  template: `
    <div class="td-sidebar">
      <div class="td-sidebar__brand">
        <mat-icon class="td-sidebar__logo-icon">description</mat-icon>
        <div class="td-sidebar__brand-text">
          <span class="td-sidebar__app-name">TourDocs</span>
          <span class="td-sidebar__org-name">{{ orgName$ | async }}</span>
        </div>
      </div>

      <nav class="td-sidebar__nav">
        <td-nav-item icon="dashboard" label="Dashboard" link="/dashboard"></td-nav-item>

        <td-nav-group icon="people" label="Members">
          <td-nav-item icon="groups" label="All Members" link="/members" [isChild]="true"></td-nav-item>
          <td-nav-item icon="person_add" label="Invite Member" link="/members/invite" [isChild]="true"></td-nav-item>
        </td-nav-group>

        <td-nav-group icon="folder" label="Documents" [expanded]="true">
          <td-nav-item icon="inventory_2" label="Document Vault" link="/documents" [isChild]="true"></td-nav-item>
          <td-nav-item icon="pending_actions" label="Pending Review" link="/documents/pending" [isChild]="true"></td-nav-item>
          <td-nav-item icon="assignment" label="Requests" link="/documents/requests" [isChild]="true"></td-nav-item>
          <td-nav-item icon="event_busy" label="Expiry Tracker" link="/documents/expiry" [isChild]="true"></td-nav-item>
        </td-nav-group>

        <td-nav-group icon="work" label="Cases">
          <td-nav-item icon="list_alt" label="All Cases" link="/cases" [isChild]="true"></td-nav-item>
          <td-nav-item icon="add_box" label="Create Case" link="/cases/create" [isChild]="true"></td-nav-item>
        </td-nav-group>

        <td-nav-item icon="checklist" label="Checklists" link="/checklists"></td-nav-item>
        <td-nav-item icon="inventory" label="Hard Copies" link="/hard-copies"></td-nav-item>
        <td-nav-item icon="history" label="Audit Log" link="/audit"></td-nav-item>

        <mat-divider class="td-sidebar__divider"></mat-divider>

        <td-nav-group icon="business" label="Organization">
          <td-nav-item icon="settings" label="Settings" link="/organization/settings" [isChild]="true"></td-nav-item>
          <td-nav-item icon="group_work" label="Team" link="/organization/team" [isChild]="true"></td-nav-item>
          <td-nav-item icon="credit_card" label="Subscription" link="/organization/subscription" [isChild]="true"></td-nav-item>
        </td-nav-group>

        <td-nav-item icon="notifications" label="Notifications" link="/notifications"
                     [badgeCount]="(unreadCount$ | async) ?? 0"></td-nav-item>
      </nav>
    </div>
  `,
  styles: [`
    .td-sidebar {
      height: 100%;
      background: #263238;
      display: flex;
      flex-direction: column;
      overflow-y: auto;
      overflow-x: hidden;

      &::-webkit-scrollbar {
        width: 4px;
      }

      &::-webkit-scrollbar-thumb {
        background: #455A64;
        border-radius: 2px;
      }
    }

    .td-sidebar__brand {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 20px;
      border-bottom: 1px solid #37474F;
      flex-shrink: 0;
    }

    .td-sidebar__logo-icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
      color: #42A5F5;
    }

    .td-sidebar__brand-text {
      display: flex;
      flex-direction: column;
    }

    .td-sidebar__app-name {
      font-size: 18px;
      font-weight: 700;
      color: #FFFFFF;
      letter-spacing: 0.5px;
    }

    .td-sidebar__org-name {
      font-size: 12px;
      color: #78909C;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      max-width: 160px;
    }

    .td-sidebar__nav {
      flex: 1;
      padding: 8px 0;
    }

    .td-sidebar__divider {
      border-color: #37474F;
      margin: 8px 16px;
    }
  `]
})
export class SidenavComponent {
  private readonly store = inject(Store<AppState>);
  orgName$: Observable<string> = this.store.select(selectOrganizationName);
  unreadCount$: Observable<number> = this.store.select(selectUnreadCount);
}
