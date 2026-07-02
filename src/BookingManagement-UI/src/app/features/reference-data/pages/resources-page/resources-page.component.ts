import { Component, OnInit } from '@angular/core';
import { BookingApiService } from '../../../../core/api/booking-api.service';
import { Resource } from '../../../../core/models/booking.models';
import { getApiErrorMessage } from '../../../../core/utils/http-error.util';

@Component({
  selector: 'app-resources-page',
  templateUrl: './resources-page.component.html',
  styleUrls: ['./resources-page.component.scss']
})
export class ResourcesPageComponent implements OnInit {
  resources: Resource[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(private readonly bookingApi: BookingApiService) {}

  ngOnInit(): void {
    this.bookingApi.getResources().subscribe({
      next: (resources) => {
        this.resources = resources;
        this.isLoading = false;
      },
      error: (error: unknown) => {
        this.errorMessage = getApiErrorMessage(error);
        this.isLoading = false;
      }
    });
  }
}
