import { Component } from '@angular/core';

interface NavigationItem {
  label: string;
  route: string;
  icon: string;
  exact?: boolean;
}

@Component({
  selector: 'app-shell',
  templateUrl: './app-shell.component.html',
  styleUrls: ['./app-shell.component.scss']
})
export class AppShellComponent {
  isSidebarOpen = false;

  readonly navigationItems: NavigationItem[] = [
    { label: 'Dashboard', route: '/dashboard', icon: '▦', exact: true },
    { label: 'Bookings', route: '/bookings', icon: '▤', exact: true },
    { label: 'Create booking', route: '/bookings/new', icon: '+', exact: true },
    { label: 'Resources', route: '/resources', icon: '▣', exact: true },
    { label: 'Users', route: '/users', icon: '◉', exact: true }
  ];

  toggleSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  closeSidebar(): void {
    this.isSidebarOpen = false;
  }
}
