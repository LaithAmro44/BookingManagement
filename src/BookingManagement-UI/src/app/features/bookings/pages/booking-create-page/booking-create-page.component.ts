import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { BookingApiService } from '../../../../core/api/booking-api.service';
import { BookingUser, CreateBookingRequest, Resource } from '../../../../core/models/booking.models';
import { getApiErrorMessage } from '../../../../core/utils/http-error.util';
import { BookingFormComponent } from '../../components/booking-form/booking-form.component';

@Component({
  selector: 'app-booking-create-page',
  templateUrl: './booking-create-page.component.html',
  styleUrls: ['./booking-create-page.component.scss']
})
export class BookingCreatePageComponent implements OnInit {
  @ViewChild(BookingFormComponent) bookingForm?: BookingFormComponent;

  resources: Resource[] = [];
  users: BookingUser[] = [];
  isLoadingReferenceData = true;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private readonly bookingApi: BookingApiService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loadReferenceData();
  }

  createBooking(request: CreateBookingRequest): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isSubmitting = true;

    this.bookingApi.createBooking(request).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.successMessage = 'Booking created successfully.';
        this.bookingForm?.reset();

        window.setTimeout(() => {
          this.router.navigate(['/bookings'], { queryParams: { resourceId: request.resourceId } });
        }, 650);
      },
      error: (error: unknown) => {
        this.isSubmitting = false;
        this.errorMessage = getApiErrorMessage(error);
      }
    });
  }

  private loadReferenceData(): void {
    this.isLoadingReferenceData = true;

    forkJoin({
      resources: this.bookingApi.getResources(),
      users: this.bookingApi.getUsers()
    }).subscribe({
      next: ({ resources, users }) => {
        this.resources = resources;
        this.users = users;
        this.isLoadingReferenceData = false;
      },
      error: (error: unknown) => {
        this.errorMessage = getApiErrorMessage(error);
        this.isLoadingReferenceData = false;
      }
    });
  }
}
