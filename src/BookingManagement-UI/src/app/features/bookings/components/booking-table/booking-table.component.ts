import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Booking, isActiveBooking } from '../../../../core/models/booking.models';

@Component({
  selector: 'app-booking-table',
  templateUrl: './booking-table.component.html',
  styleUrls: ['./booking-table.component.scss']
})
export class BookingTableComponent {
  @Input() bookings: Booking[] = [];
  @Input() isLoading = false;
  @Input() cancellingBookingId: string | null = null;
  @Output() cancelRequested = new EventEmitter<Booking>();

  isActive(booking: Booking): boolean {
    return isActiveBooking(booking.status);
  }

  requestCancel(booking: Booking): void {
    this.cancelRequested.emit(booking);
  }
}
