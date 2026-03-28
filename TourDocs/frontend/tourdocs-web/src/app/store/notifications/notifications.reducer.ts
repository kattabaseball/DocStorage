import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { AppNotification } from '@core/services/notification.service';
import { NotificationActions } from './notifications.actions';

export interface NotificationsState extends EntityState<AppNotification> {
  loading: boolean;
  error: string | null;
}

export const notificationsAdapter: EntityAdapter<AppNotification> = createEntityAdapter<AppNotification>({
  selectId: (notification) => notification.id,
  sortComparer: (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
});

export const initialNotificationsState: NotificationsState = notificationsAdapter.getInitialState({
  loading: false,
  error: null
});

export const notificationsReducer = createReducer(
  initialNotificationsState,
  on(NotificationActions.loadNotifications, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  on(NotificationActions.loadNotificationsSuccess, (state, { notifications }) =>
    notificationsAdapter.setAll(notifications, {
      ...state,
      loading: false
    })
  ),
  on(NotificationActions.loadNotificationsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(NotificationActions.markAsReadSuccess, (state, { id }) =>
    notificationsAdapter.updateOne({ id, changes: { isRead: true } }, state)
  ),
  on(NotificationActions.markAllAsReadSuccess, (state) =>
    notificationsAdapter.map(
      (notification) => ({ ...notification, isRead: true }),
      state
    )
  ),
  on(NotificationActions.newNotification, (state, { notification }) =>
    notificationsAdapter.addOne(notification, state)
  )
);
