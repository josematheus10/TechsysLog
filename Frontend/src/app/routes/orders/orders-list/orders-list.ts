import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { OrdersService, OrderResponse } from '../orders.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-orders-list',
  templateUrl: './orders-list.html',
  styleUrl: './orders-list.scss',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatChipsModule,
    MatIconModule,
    MatTableModule,
    MatSnackBarModule,
  ],
})
export class OrdersList implements OnInit {
  private readonly ordersService = inject(OrdersService);
  private readonly snackBar = inject(MatSnackBar);

  orders: OrderResponse[] = [];
  displayedColumns: string[] = ['orderNumber', 'description', 'value', 'status', 'actions'];
  isLoading = false;
  updatingOrderId: string | null = null;

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.ordersService
      .getOrders()
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: orders => {
          this.orders = orders;
        },
        error: error => {
          console.error('Erro ao carregar pedidos:', error);
          this.snackBar.open('Erro ao carregar pedidos', 'Fechar', { duration: 3000 });
        },
      });
  }

  updateStatus(orderId: string, newStatus: 'novo' | 'entregue'): void {
    this.updatingOrderId = orderId;
    this.ordersService
      .updateOrderStatus(orderId, newStatus)
      .pipe(finalize(() => (this.updatingOrderId = null)))
      .subscribe({
        next: updatedOrder => {
          const index = this.orders.findIndex(o => o.id === orderId);
          if (index !== -1) {
            this.orders[index] = updatedOrder;
          }
          this.snackBar.open('Status atualizado com sucesso!', 'Fechar', { duration: 3000 });
        },
        error: error => {
          console.error('Erro ao atualizar status:', error);
          this.snackBar.open('Erro ao atualizar status', 'Fechar', { duration: 3000 });
        },
      });
  }

  getStatusColor(status: string): string {
    return status === 'novo' ? 'primary' : 'accent';
  }

  getStatusIcon(status: string): string {
    return status === 'novo' ? 'new_releases' : 'check_circle';
  }

  canMarkAsDelivered(status: string): boolean {
    return status === 'novo';
  }

  canMarkAsNew(status: string): boolean {
    return status === 'entregue';
  }
}
