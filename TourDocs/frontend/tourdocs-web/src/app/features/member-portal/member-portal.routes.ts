import { Routes } from '@angular/router';

export const MEMBER_PORTAL_ROUTES: Routes = [
  {
    path: 'my-documents',
    loadComponent: () => import('./my-documents/my-documents.component').then(m => m.MyDocumentsComponent)
  },
  {
    path: 'my-cases',
    loadComponent: () => import('./my-cases/my-cases.component').then(m => m.MyCasesComponent)
  },
  {
    path: '',
    redirectTo: 'my-documents',
    pathMatch: 'full'
  }
];
