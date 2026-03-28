import { Routes } from '@angular/router';

export const CHECKLIST_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./checklist-list/checklist-list.component').then(m => m.ChecklistListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./checklist-editor/checklist-editor.component').then(m => m.ChecklistEditorComponent),
    data: { mode: 'create' }
  },
  {
    path: ':id',
    loadComponent: () => import('./checklist-detail/checklist-detail.component').then(m => m.ChecklistDetailComponent)
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./checklist-editor/checklist-editor.component').then(m => m.ChecklistEditorComponent),
    data: { mode: 'edit' }
  }
];
