import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { extractApiErrorMessage } from './http-error.util';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse)) {
        return throwError(() => error);
      }

      const normalizedMessage = extractApiErrorMessage(error);
      const currentMessage = typeof error.error === 'object' && error.error !== null
        ? error.error.message
        : '';

      if (currentMessage === normalizedMessage) {
        return throwError(() => error);
      }

      const normalizedPayload = typeof error.error === 'object' && error.error !== null
        ? { ...error.error, message: normalizedMessage }
        : { message: normalizedMessage };

      const normalizedError = new HttpErrorResponse({
        error: normalizedPayload,
        headers: error.headers,
        status: error.status,
        statusText: error.statusText,
        url: error.url ?? undefined
      });

      return throwError(() => normalizedError);
    })
  );
};
