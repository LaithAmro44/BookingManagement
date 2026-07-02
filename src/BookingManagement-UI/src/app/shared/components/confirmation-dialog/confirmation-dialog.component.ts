import {
    ChangeDetectionStrategy,
    Component,
    EventEmitter,
    HostListener,
    Input,
    Output
} from '@angular/core';

@Component({
    selector: 'app-confirmation-dialog',
    templateUrl: './confirmation-dialog.component.html',
    styleUrls: ['./confirmation-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ConfirmationDialogComponent {
    @Input() title = 'Confirm action';
    @Input() confirmText = 'Confirm';
    @Input() cancelText = 'Cancel';
    @Input() loading = false;
    @Input() destructive = false;
    @Input() errorMessage = '';

    @Output() confirm = new EventEmitter<void>();
    @Output() cancel = new EventEmitter<void>();

    @HostListener('document:keydown.escape')
    onEscapePressed(): void {
        this.closeIfPossible();
    }

    onBackdropClicked(): void {
        this.closeIfPossible();
    }

    private closeIfPossible(): void {
        if (!this.loading) {
            this.cancel.emit();
        }
    }
}