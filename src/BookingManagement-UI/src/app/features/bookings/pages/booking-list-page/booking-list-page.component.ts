import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { BookingApiService } from '../../../../core/api/booking-api.service';
import {
  Booking,
  BookingSearchRequest,
  PagedResult,
  Resource
} from '../../../../core/models/booking.models';
import { getApiErrorMessage } from '../../../../core/utils/http-error.util';

@Component({
  selector: 'app-booking-list-page',
  templateUrl: './booking-list-page.component.html',
  styleUrls: ['./booking-list-page.component.scss']
})
export class BookingListPageComponent implements OnInit {
  resources: Resource[] = [];
  bookings: Booking[] = [];

  isLoading = false;
  isLoadingResources = true;

  errorMessage = '';

  cancellingBookingId: string | null = null;
  bookingToCancel: Booking | null = null;
  cancellationErrorMessage = '';

  search: BookingSearchRequest = {
    resourceId: '',
    includeCancelled: true,
    pageNumber: 1,
    pageSize: 10
  };

  page: PagedResult<Booking> = {
    items: [],
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0
  };

  constructor(
    private readonly bookingApi: BookingApiService,
    private readonly route: ActivatedRoute,
    private readonly router: Router
  ) { }

  ngOnInit(): void {
    this.loadResources();
  }

  applyFilters(search: BookingSearchRequest): void {
    this.search = {
      ...search,
      pageNumber: 1
    };

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { resourceId: this.search.resourceId },
      queryParamsHandling: ''
    });

    this.loadBookings();
  }

  changePage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.page.totalPages) {
      return;
    }

    this.search = {
      ...this.search,
      pageNumber
    };

    this.loadBookings();
  }

  openCancelDialog(booking: Booking): void {
    if (this.cancellingBookingId) {
      return;
    }

    this.bookingToCancel = booking;
    this.cancellationErrorMessage = '';
    this.errorMessage = '';
  }

  closeCancelDialog(): void {
    if (this.cancellingBookingId) {
      return;
    }

    this.bookingToCancel = null;
    this.cancellationErrorMessage = '';
  }

  confirmCancellation(): void {
    const booking = this.bookingToCancel;

    if (!booking || this.cancellingBookingId) {
      return;
    }

    this.cancellingBookingId = booking.id;
    this.cancellationErrorMessage = '';
    this.errorMessage = '';

    this.bookingApi
      .cancelBooking(booking.id)
      .pipe(
        finalize(() => {
          this.cancellingBookingId = null;
        })
      )
      .subscribe({
        next: () => {
          this.bookingToCancel = null;
          this.loadBookings();
        },
        error: (error: unknown) => {
          this.cancellationErrorMessage = getApiErrorMessage(error);
        }
      });
  }

  private loadResources(): void {
    this.isLoadingResources = true;
    this.errorMessage = '';

    this.bookingApi.getResources().subscribe({
      next: (resources) => {
        this.resources = resources;
        this.isLoadingResources = false;

        const requestedResourceId =
          this.route.snapshot.queryParamMap.get('resourceId');

        const selectedResourceId = resources.some(
          (resource) => resource.id === requestedResourceId
        )
          ? (requestedResourceId as string)
          : resources.length > 0
            ? resources[0].id
            : '';

        this.search = {
          ...this.search,
          resourceId: selectedResourceId
        };

        if (selectedResourceId) {
          this.loadBookings();
        }
      },
      error: (error: unknown) => {
        this.errorMessage = getApiErrorMessage(error);
        this.isLoadingResources = false;
      }
    });
  }

  private loadBookings(): void {
    if (!this.search.resourceId) {
      this.bookings = [];
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.bookingApi
      .getBookings(this.search)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: (page) => {
          this.page = page;
          this.bookings = page.items || [];
        },
        error: (error: unknown) => {
          this.errorMessage = getApiErrorMessage(error);
          this.bookings = [];
        }
      });
  }
}