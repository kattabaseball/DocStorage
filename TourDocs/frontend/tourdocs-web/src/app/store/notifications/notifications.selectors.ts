import { createFeatureSelector, createSelector } from '@ngrx/store';
import { NotificationsState, notificationsAdapter } from './notifications.reducer';

export const selectNotificationsState = createFeatureSelector<NotificationsState>('notifications');

const { selectAll, selectTotal } = notificationsAdapter.getSelectors();

export const selectAllNotifications = createSelector(
  selectNotificationsState,
  selectAll
);

export const selectNotificationsTotal = createSelector(
  selectNotificationsState,
  selectTotal
);

export const selectUnreadCount = createSelector(
  selectAllNotifications,
  (notifications) => notifications.filter(n => !n.isRead).length
);

export const selectUnreadNotifications = createSelector(
  selectAllNotifications,
  (notifications) => notifications.filter(n => !n.isRead)
);

export const selectRecentNotifications = createSelector(
  selectAllNotifications,
  (notifications) => notifications.slice(0, 5)
);

export const selectNotificationsLoading = createSelector(
  selectNotificationsState,
  (state) => state.loading
);
