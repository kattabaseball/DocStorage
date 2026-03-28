import { Routes } from '@angular/router';

export const DOCUMENT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./document-vault/document-vault.component').then(m => m.DocumentVaultComponent)
  },
  {
    path: 'pending',
    loadComponent: () => import('./document-vault/document-vault.component').then(m => m.DocumentVaultComponent),
    data: { filter: 'pending' }
  },
  {
    path: 'requests',
    loadComponent: () => import('./document-vault/document-vault.component').then(m => m.DocumentVaultComponent),
    data: { filter: 'requests' }
  },
  {
    path: 'expiry',
    loadComponent: () => import('./expiry-tracker/expiry-tracker.component').then(m => m.ExpiryTrackerComponent)
  }
];
