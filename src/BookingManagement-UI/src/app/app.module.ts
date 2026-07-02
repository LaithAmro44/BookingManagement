import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AppShellComponent } from './layout/app-shell/app-shell.component';
import { PageHeaderComponent } from './shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from './shared/components/status-badge/status-badge.component';
import { EmptyStateComponent } from './shared/components/empty-state/empty-state.component';
import { PaginationComponent } from './shared/components/pagination/pagination.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { BookingCreatePageComponent } from './features/bookings/pages/booking-create-page/booking-create-page.component';
import { BookingListPageComponent } from './features/bookings/pages/booking-list-page/booking-list-page.component';
import { BookingFormComponent } from './features/bookings/components/booking-form/booking-form.component';
import { BookingFiltersComponent } from './features/bookings/components/booking-filters/booking-filters.component';
import { BookingTableComponent } from './features/bookings/components/booking-table/booking-table.component';
import { ResourcesPageComponent } from './features/reference-data/pages/resources-page/resources-page.component';
import { UsersPageComponent } from './features/reference-data/pages/users-page/users-page.component';
import { NotFoundComponent } from './features/not-found/not-found.component';
import { ConfirmationDialogComponent } from './shared/components/confirmation-dialog/confirmation-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    AppShellComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
    EmptyStateComponent,
    PaginationComponent,
    DashboardComponent,
    BookingCreatePageComponent,
    BookingListPageComponent,
    BookingFormComponent,
    BookingFiltersComponent,
    BookingTableComponent,
    ResourcesPageComponent,
    UsersPageComponent,
    NotFoundComponent,
    ConfirmationDialogComponent,
  ],
  imports: [BrowserModule, HttpClientModule, ReactiveFormsModule, AppRoutingModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
