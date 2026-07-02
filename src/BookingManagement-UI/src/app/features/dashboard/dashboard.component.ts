import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { BookingApiService } from '../../core/api/booking-api.service';
import { getApiErrorMessage } from '../../core/utils/http-error.util';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  resourceCount = 0;
  userCount = 0;
  isLoading = true;
  errorMessage = '';

  constructor(private readonly bookingApi: BookingApiService) {}

  ngOnInit(): void {
    this.loadSummary();
  }

  private loadSummary(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      resources: this.bookingApi.getResources(),
      users: this.bookingApi.getUsers()
    }).subscribe({
      next: ({ resources, users }) => {
        this.resourceCount = resources.length;
        this.userCount = users.length;
        this.isLoading = false;
      },
      error: (error: unknown) => {
        this.errorMessage = getApiErrorMessage(error);
        this.isLoading = false;
      }
    });
  }
}
