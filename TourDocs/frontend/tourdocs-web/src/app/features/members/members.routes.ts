import { Routes } from '@angular/router';

export const MEMBER_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./member-list/member-list.component').then(m => m.MemberListComponent)
  },
  {
    path: 'invite',
    loadComponent: () => import('./member-form/member-form.component').then(m => m.MemberFormComponent),
    data: { mode: 'invite' }
  },
  {
    path: 'new',
    loadComponent: () => import('./member-form/member-form.component').then(m => m.MemberFormComponent),
    data: { mode: 'create' }
  },
  {
    path: ':id',
    loadComponent: () => import('./member-detail/member-detail.component').then(m => m.MemberDetailComponent)
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./member-form/member-form.component').then(m => m.MemberFormComponent),
    data: { mode: 'edit' }
  }
];
