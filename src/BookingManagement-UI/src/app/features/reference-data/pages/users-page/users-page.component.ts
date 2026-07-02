import { Component, OnInit } from '@angular/core';
import { BookingApiService } from '../../../../core/api/booking-api.service';
import { BookingUser } from '../../../../core/models/booking.models';
import { getApiErrorMessage } from '../../../../core/utils/http-error.util';

@Component({
  selector: 'app-users-page',
  templateUrl: './users-page.component.html',
  styleUrls: ['./users-page.component.scss']
})
export class UsersPageComponent implements OnInit {
  users: BookingUser[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(private readonly bookingApi: BookingApiService) {}

  ngOnInit(): void {
    this.bookingApi.getUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.isLoading = false;
      },
      error: (error: unknown) => {
        this.errorMessage = getApiErrorMessage(error);
        this.isLoading = false;
      }
    });
  }
}
