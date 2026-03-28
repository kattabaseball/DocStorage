import { ApplicationConfig, importProvidersFrom, isDevMode } from '@angular/core';
import { provideRouter, withComponentInputBinding, withRouterConfig } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatNativeDateModule } from '@angular/material/core';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

import { routes } from './app.routes';
import { jwtInterceptor } from '@core/auth/jwt.interceptor';
import { errorInterceptor } from '@core/interceptors/error.interceptor';
import { loadingInterceptor } from '@core/interceptors/loading.interceptor';
import { authReducer } from '@store/auth/auth.reducer';
import { uiReducer } from '@store/ui/ui.reducer';
import { notificationsReducer } from '@store/notifications/notifications.reducer';
import { AuthEffects } from '@store/auth/auth.effects';
import { NotificationsEffects } from '@store/notifications/notifications.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(
      routes,
      withComponentInputBinding(),
      withRouterConfig({ onSameUrlNavigation: 'reload' })
    ),
    provideHttpClient(
      withInterceptors([jwtInterceptor, errorInterceptor, loadingInterceptor])
    ),
    provideAnimations(),
    provideStore({
      auth: authReducer,
      ui: uiReducer,
      notifications: notificationsReducer
    }),
    provideEffects([AuthEffects, NotificationsEffects]),
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
      autoPause: true
    }),
    provideCharts(withDefaultRegisterables()),
    importProvidersFrom(MatSnackBarModule, MatNativeDateModule)
  ]
};
