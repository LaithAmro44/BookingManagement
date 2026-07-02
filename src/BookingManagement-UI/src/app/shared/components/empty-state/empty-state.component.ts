import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  templateUrl: './empty-state.component.html',
  styleUrls: ['./empty-state.component.scss']
})
export class EmptyStateComponent {
  @Input() icon = '⌁';
  @Input() title = 'No data found';
  @Input() description = 'Try changing your filters or create a new record.';
  @Input() actionText?: string;
  @Input() actionRoute?: string;
}
