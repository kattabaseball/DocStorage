import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { AppNotification } from '@core/services/notification.service';

export const NotificationActions = createActionGroup({
  source: 'Notifications',
  events: {
    'Load Notifications': emptyProps(),
    'Load Notifications Success': props<{ notifications: AppNotification[] }>(),
    'Load Notifications Failure': props<{ error: string }>(),
    'Mark As Read': props<{ id: string }>(),
    'Mark As Read Success': props<{ id: string }>(),
    'Mark All As Read': emptyProps(),
    'Mark All As Read Success': emptyProps(),
    'New Notification': props<{ notification: AppNotification }>(),
  }
});
