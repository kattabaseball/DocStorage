import { Routes } from '@angular/router';

export const HARD_COPY_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./hard-copy-list/hard-copy-list.component').then(m => m.HardCopyListComponent)
  },
  {
    path: ':id/timeline',
    loadComponent: () => import('./hard-copy-timeline/hard-copy-timeline.component').then(m => m.HardCopyTimelineComponent)
  }
];
