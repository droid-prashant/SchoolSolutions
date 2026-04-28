import { HttpErrorResponse } from '@angular/common/http';

export function extractApiErrorMessage(error: HttpErrorResponse): string {
  const payload = error.error;

  if (typeof payload === 'string') {
    const trimmedPayload = payload.trim();
    if (trimmedPayload && !looksLikeHtml(trimmedPayload)) {
      return trimmedPayload;
    }
  }

  if (payload && typeof payload === 'object') {
    const messageFromPayload = firstNonEmptyString([
      payload.message,
      payload.title,
      payload.detail,
      payload.error?.message,
      typeof payload.error === 'string' ? payload.error : ''
    ]);

    if (messageFromPayload) {
      return messageFromPayload;
    }

    const validationMessage = extractValidationMessage(payload.errors);
    if (validationMessage) {
      return validationMessage;
    }
  }

  return firstNonEmptyString([
    error.message,
    error.statusText,
    'Something went wrong while processing the request.'
  ])!;
}

function extractValidationMessage(errors: unknown): string {
  if (!errors || typeof errors !== 'object') {
    return '';
  }

  const allMessages = Object.values(errors as Record<string, unknown>)
    .flatMap(value => Array.isArray(value) ? value : [value])
    .filter((value): value is string => typeof value === 'string' && value.trim().length > 0);

  return allMessages[0] ?? '';
}

function firstNonEmptyString(values: Array<string | undefined | null>): string {
  return values.find((value): value is string => typeof value === 'string' && value.trim().length > 0)?.trim() ?? '';
}

function looksLikeHtml(value: string): boolean {
  return value.startsWith('<!DOCTYPE') || value.startsWith('<html');
}
