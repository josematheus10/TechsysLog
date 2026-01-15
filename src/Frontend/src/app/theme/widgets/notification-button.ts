import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { OrdersService } from '../../routes/orders/orders.service';
import { Subscription } from 'rxjs';
import { SignalRService } from '@core';

@Component({
selector: 'app-notification',
template: `
  <button mat-icon-button [matMenuTriggerFor]="menu">
    <mat-icon [matBadge]="notificationCount" matBadgeColor="warn" aria-hidden="false">notifications</mat-icon>
  </button>

  <mat-menu #menu="matMenu">
    <mat-nav-list>
      @if (messages.length === 0) {
        <mat-list-item>
          <span matListItemTitle>Nenhuma notificação</span>
        </mat-list-item>
      }
      @for (message of messages; track message) {
        <mat-list-item>
          <mat-icon class="m-x-16" matListItemIcon>info</mat-icon>
          <a matListItemTitle href="#">{{ message }}</a>
        </mat-list-item>
      }
    </mat-nav-list>
  </mat-menu>
`,
  styles: `
    :host ::ng-deep .mat-badge-content {
      --mat-badge-background-color: #ef0000;
      --mat-badge-text-color: #fff;
    }
  `,
  imports: [MatBadgeModule, MatButtonModule, MatIconModule, MatListModule, MatMenuModule],
})
export class NotificationButton implements OnInit, OnDestroy {
  private readonly signalRService = inject(SignalRService);
  private subscription = new Subscription();

  messages: string[] = [];
  notificationCount = 0;

  ngOnInit(): void {
    // Inscrever-se no evento de novas ordens
    this.subscription.add(
      this.signalRService.on('new-order-notify').subscribe({
        next: (orderNumber: string) => {
          this.messages.unshift(`Novo pedido: ${orderNumber}`);
          this.notificationCount = this.messages.length;
        },
        error: (err) => {}
      })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
