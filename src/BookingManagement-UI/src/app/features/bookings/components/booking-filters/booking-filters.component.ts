import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BookingSearchRequest, Resource } from '../../../../core/models/booking.models';

@Component({
  selector: 'app-booking-filters',
  templateUrl: './booking-filters.component.html',
  styleUrls: ['./booking-filters.component.scss']
})
export class BookingFiltersComponent implements OnChanges {
  @Input() resources: Resource[] = [];
  @Input() selectedResourceId = '';
  @Input() isLoading = false;
  @Output() filtersApplied = new EventEmitter<BookingSearchRequest>();

  formError = '';

  readonly form = this.formBuilder.group({
    resourceId: [''],
    fromLocal: [''],
    toLocal: [''],
    includeCancelled: [true]
  });

  constructor(private readonly formBuilder: FormBuilder) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedResourceId'] && this.selectedResourceId) {
      this.form.patchValue({ resourceId: this.selectedResourceId }, { emitEvent: false });
    }
  }

  apply(): void {
    this.formError = '';
    const value = this.form.getRawValue();

    if (!value.resourceId) {
      this.formError = 'Please select a resource.';
      return;
    }

    const fromUtc = this.toUtc(value.fromLocal);
    const toUtc = this.toUtc(value.toLocal);

    if (value.fromLocal && !fromUtc || value.toLocal && !toUtc) {
      this.formError = 'Please enter valid filter dates.';
      return;
    }

    if (fromUtc && toUtc && new Date(fromUtc) >= new Date(toUtc)) {
      this.formError = 'The To date must be later than the From date.';
      return;
    }

    this.filtersApplied.emit({
      resourceId: value.resourceId,
      fromUtc,
      toUtc,
      includeCancelled: !!value.includeCancelled,
      pageNumber: 1,
      pageSize: 10
    });
  }

  clearDates(): void {
    this.form.patchValue({ fromLocal: '', toLocal: '' });
    this.apply();
  }

  private toUtc(value: string | null): string | undefined {
    if (!value) {
      return undefined;
    }

    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? undefined : date.toISOString();
  }
}
