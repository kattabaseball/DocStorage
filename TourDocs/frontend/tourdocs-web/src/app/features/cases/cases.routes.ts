import { Routes } from '@angular/router';

export const CASE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./case-list/case-list.component').then(m => m.CaseListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./case-form/case-form.component').then(m => m.CaseFormComponent),
    data: { mode: 'create' }
  },
  {
    path: ':id',
    loadComponent: () => import('./case-detail/case-detail.component').then(m => m.CaseDetailComponent)
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./case-form/case-form.component').then(m => m.CaseFormComponent),
    data: { mode: 'edit' }
  }
];
