import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { authGuard } from '@core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES)
      },
      {
        path: 'members',
        loadChildren: () => import('./features/members/members.routes').then(m => m.MEMBER_ROUTES)
      },
      {
        path: 'documents',
        loadChildren: () => import('./features/documents/documents.routes').then(m => m.DOCUMENT_ROUTES)
      },
      {
        path: 'cases',
        loadChildren: () => import('./features/cases/cases.routes').then(m => m.CASE_ROUTES)
      },
      {
        path: 'hard-copies',
        loadChildren: () => import('./features/hard-copies/hard-copies.routes').then(m => m.HARD_COPY_ROUTES)
      },
      {
        path: 'checklists',
        loadChildren: () => import('./features/checklists/checklists.routes').then(m => m.CHECKLIST_ROUTES)
      },
      {
        path: 'organization',
        loadChildren: () => import('./features/organization/organization.routes').then(m => m.ORGANIZATION_ROUTES)
      },
      {
        path: 'audit',
        loadChildren: () => import('./features/audit/audit.routes').then(m => m.AUDIT_ROUTES)
      },
      {
        path: 'notifications',
        loadChildren: () => import('./features/notifications/notifications.routes').then(m => m.NOTIFICATION_ROUTES)
      },
      {
        path: 'portal',
        loadChildren: () => import('./features/member-portal/member-portal.routes').then(m => m.MEMBER_PORTAL_ROUTES)
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
