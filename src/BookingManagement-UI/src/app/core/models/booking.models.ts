export type BookingStatus = 1 | 2 | 'Active' | 'Cancelled';

export interface Resource {
  id: string;
  name: string;
}

export interface BookingUser {
  id: string;
  displayName: string;
}

export interface CreateBookingRequest {
  resourceId: string;
  userId: string;
  startDateTime: string;
  endDateTime: string;
}

export interface Booking {
  id: string;
  resourceId: string;
  resourceName: string;
  userId: string;
  userDisplayName: string;
  startDateTimeUtc: string;
  endDateTimeUtc: string;
  status: BookingStatus;
  createdAtUtc: string;
  cancelledAtUtc: string | null;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
}

export interface BookingSearchRequest {
  resourceId: string;
  fromUtc?: string;
  toUtc?: string;
  includeCancelled: boolean;
  pageNumber: number;
  pageSize: number;
}

export function isActiveBooking(status: BookingStatus): boolean {
  return status === 1 || status === 'Active';
}

export function getBookingStatusLabel(status: BookingStatus): string {
  return isActiveBooking(status) ? 'Active' : 'Cancelled';
}
