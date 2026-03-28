import { Routes } from '@angular/router';

export const ORGANIZATION_ROUTES: Routes = [
  {
    path: 'settings',
    loadComponent: () => import('./org-settings/org-settings.component').then(m => m.OrgSettingsComponent)
  },
  {
    path: 'team',
    loadComponent: () => import('./team-management/team-management.component').then(m => m.TeamManagementComponent)
  },
  {
    path: 'subscription',
    loadComponent: () => import('./subscription/subscription.component').then(m => m.SubscriptionComponent)
  },
  {
    path: '',
    redirectTo: 'settings',
    pathMatch: 'full'
  }
];
