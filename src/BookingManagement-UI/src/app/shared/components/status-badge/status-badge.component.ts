import { Component, Input } from '@angular/core';
import { BookingStatus, getBookingStatusLabel, isActiveBooking } from '../../../core/models/booking.models';

@Component({
  selector: 'app-status-badge',
  templateUrl: './status-badge.component.html',
  styleUrls: ['./status-badge.component.scss']
})
export class StatusBadgeComponent {
  @Input() status!: BookingStatus;

  get label(): string {
    return getBookingStatusLabel(this.status);
  }

  get isActive(): boolean {
    return isActiveBooking(this.status);
  }
}
