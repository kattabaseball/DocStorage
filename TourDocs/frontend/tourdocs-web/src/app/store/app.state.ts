import { AuthState } from './auth/auth.reducer';
import { UiState } from './ui/ui.reducer';
import { NotificationsState } from './notifications/notifications.reducer';

export interface AppState {
  auth: AuthState;
  ui: UiState;
  notifications: NotificationsState;
}
