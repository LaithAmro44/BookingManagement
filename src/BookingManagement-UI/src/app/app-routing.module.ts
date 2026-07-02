import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { BookingCreatePageComponent } from './features/bookings/pages/booking-create-page/booking-create-page.component';
import { BookingListPageComponent } from './features/bookings/pages/booking-list-page/booking-list-page.component';
import { ResourcesPageComponent } from './features/reference-data/pages/resources-page/resources-page.component';
import { UsersPageComponent } from './features/reference-data/pages/users-page/users-page.component';
import { NotFoundComponent } from './features/not-found/not-found.component';
import { AppShellComponent } from './layout/app-shell/app-shell.component';

const routes: Routes = [
  {
    path: '',
    component: AppShellComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent, title: 'Dashboard | Booking Management' },
      { path: 'bookings', component: BookingListPageComponent, title: 'Bookings | Booking Management' },
      { path: 'bookings/new', component: BookingCreatePageComponent, title: 'Create Booking | Booking Management' },
      { path: 'resources', component: ResourcesPageComponent, title: 'Resources | Booking Management' },
      { path: 'users', component: UsersPageComponent, title: 'Users | Booking Management' },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: '**', component: NotFoundComponent, title: 'Not Found | Booking Management' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
