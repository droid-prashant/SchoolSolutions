import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';

import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ConfirmationService, MessageService } from 'primeng/api';
import { tokenInterceptor } from './shared/tokenInterceptor.service';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes), 
              provideAnimations(),
              provideHttpClient(withInterceptors([tokenInterceptor]),withFetch()), MessageService, ConfirmationService]
};
