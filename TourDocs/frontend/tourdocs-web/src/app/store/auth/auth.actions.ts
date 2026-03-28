import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { LoginRequest, RegisterRequest, TokenResponse, UserProfile } from '@core/auth/auth.models';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    'Login': props<{ request: LoginRequest }>(),
    'Login Success': props<{ tokenResponse: TokenResponse }>(),
    'Login Failure': props<{ error: string }>(),
    'Register': props<{ request: RegisterRequest }>(),
    'Register Success': props<{ tokenResponse: TokenResponse }>(),
    'Register Failure': props<{ error: string }>(),
    'Init Auth': emptyProps(),
    'Init Auth Complete': props<{ token: TokenResponse; user: UserProfile }>(),
    'Logout': emptyProps(),
    'Load Profile': emptyProps(),
    'Load Profile Success': props<{ user: UserProfile }>(),
    'Load Profile Failure': props<{ error: string }>(),
    'Clear Error': emptyProps(),
  }
});
