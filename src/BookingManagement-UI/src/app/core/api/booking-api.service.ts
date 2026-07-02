import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Booking,
  BookingSearchRequest,
  BookingUser,
  CreateBookingRequest,
  PagedResult,
  Resource
} from '../models/booking.models';

@Injectable({ providedIn: 'root' })
export class BookingApiService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  getResources(): Observable<Resource[]> {
    return this.http.get<Resource[]>(`${this.baseUrl}/resources`);
  }

  getUsers(): Observable<BookingUser[]> {
    return this.http.get<BookingUser[]>(`${this.baseUrl}/users`);
  }

  createBooking(request: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(`${this.baseUrl}/bookings`, request);
  }

  getBookings(request: BookingSearchRequest): Observable<PagedResult<Booking>> {
    let params = new HttpParams()
      .set('includeCancelled', String(request.includeCancelled))
      .set('pageNumber', String(request.pageNumber))
      .set('pageSize', String(request.pageSize));

    if (request.fromUtc) {
      params = params.set('fromUtc', request.fromUtc);
    }

    if (request.toUtc) {
      params = params.set('toUtc', request.toUtc);
    }

    return this.http.get<PagedResult<Booking>>(
      `${this.baseUrl}/bookings/resource/${encodeURIComponent(request.resourceId)}`,
      { params }
    );
  }

  cancelBooking(bookingId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/bookings/${bookingId}/cancel`, {});
  }
}
