import { createReducer, on } from '@ngrx/store';
import { UiActions } from './ui.actions';

export interface UiState {
  sidebarOpen: boolean;
  theme: 'light' | 'dark';
  globalLoading: boolean;
}

export const initialUiState: UiState = {
  sidebarOpen: true,
  theme: 'light',
  globalLoading: false
};

export const uiReducer = createReducer(
  initialUiState,
  on(UiActions.toggleSidebar, (state) => ({
    ...state,
    sidebarOpen: !state.sidebarOpen
  })),
  on(UiActions.setSidebarOpen, (state, { open }) => ({
    ...state,
    sidebarOpen: open
  })),
  on(UiActions.setTheme, (state, { theme }) => ({
    ...state,
    theme
  })),
  on(UiActions.setLoading, (state, { loading }) => ({
    ...state,
    globalLoading: loading
  }))
);
