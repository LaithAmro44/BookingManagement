import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails } from '../models/booking.models';

export function getApiErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as ProblemDetails | string | null;

    if (body && typeof body === 'object' && body.detail) {
      return body.detail;
    }

    if (typeof body === 'string' && body.trim()) {
      return body;
    }

    if (error.status === 0) {
      return 'Could not reach the API. Start the backend and check proxy.conf.json.';
    }
  }

  return 'Something went wrong. Please try again.';
}
