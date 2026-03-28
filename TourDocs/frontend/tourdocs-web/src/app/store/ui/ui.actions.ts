import { createActionGroup, emptyProps, props } from '@ngrx/store';

export const UiActions = createActionGroup({
  source: 'UI',
  events: {
    'Toggle Sidebar': emptyProps(),
    'Set Sidebar Open': props<{ open: boolean }>(),
    'Set Theme': props<{ theme: 'light' | 'dark' }>(),
    'Set Loading': props<{ loading: boolean }>(),
  }
});
