import { createFeatureSelector, createSelector } from '@ngrx/store';
import { UiState } from './ui.reducer';

export const selectUiState = createFeatureSelector<UiState>('ui');

export const selectSidebarOpen = createSelector(
  selectUiState,
  (state) => state.sidebarOpen
);

export const selectTheme = createSelector(
  selectUiState,
  (state) => state.theme
);

export const selectGlobalLoading = createSelector(
  selectUiState,
  (state) => state.globalLoading
);
