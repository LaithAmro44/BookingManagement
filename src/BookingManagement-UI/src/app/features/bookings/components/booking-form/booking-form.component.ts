import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { BookingUser, CreateBookingRequest, Resource } from '../../../../core/models/booking.models';

@Component({
  selector: 'app-booking-form',
  templateUrl: './booking-form.component.html',
  styleUrls: ['./booking-form.component.scss']
})
export class BookingFormComponent {
  @Input() resources: Resource[] = [];
  @Input() users: BookingUser[] = [];
  @Input() isSubmitting = false;
  @Input() errorMessage = '';
  @Output() bookingSubmitted = new EventEmitter<CreateBookingRequest>();

  formError = '';

  readonly form = this.formBuilder.group({
    resourceId: ['', Validators.required],
    userId: ['', Validators.required],
    startDateTime: ['', Validators.required],
    endDateTime: ['', Validators.required]
  });

  constructor(private readonly formBuilder: FormBuilder) {}

  submit(): void {
    this.formError = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.formError = 'Please complete all required fields.';
      return;
    }

    const value = this.form.getRawValue();
    const start = new Date(value.startDateTime as string);
    const end = new Date(value.endDateTime as string);

    if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) {
      this.formError = 'Please enter valid dates and times.';
      return;
    }

    if (start >= end) {
      this.formError = 'The end time must be later than the start time.';
      return;
    }

    this.bookingSubmitted.emit({
      resourceId: value.resourceId as string,
      userId: value.userId as string,
      startDateTime: start.toISOString(),
      endDateTime: end.toISOString()
    });
  }

  reset(): void {
    this.form.reset();
    this.formError = '';
  }

  showFieldError(controlName: string): boolean {
    const control = this.form.get(controlName);
    return !!control && control.invalid && (control.dirty || control.touched);
  }
}
