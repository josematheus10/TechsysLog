import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

export interface DeliveryAddress {
  cep: string;
  street: string;
  number: string;
  neighborhood: string;
  city: string;
  state: string;
}

export interface CreateOrderRequest {
  orderNumber: string;
  description: string;
  value: number;
  deliveryAddress: DeliveryAddress;
}

export interface OrderResponse {
  id: string;
  orderNumber: string;
  description: string;
  value: number;
  deliveryAddress: DeliveryAddress;
  status: string;
  userId: string;
  userName?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface UpdateOrderStatusRequest {
  status: 'novo' | 'entregue';
}

@Injectable({
  providedIn: 'root',
})
export class OrdersService {
  protected readonly http = inject(HttpClient);

  createOrder(order: CreateOrderRequest): Observable<OrderResponse> {
    return this.http.post<OrderResponse>('/api/Orders', order);
  }

  getOrders(): Observable<OrderResponse[]> {
    return this.http.get<OrderResponse[]>('/api/Orders');
  }

  getOrderById(id: string): Observable<OrderResponse> {
    return this.http.get<OrderResponse>(`/api/Orders/${id}`);
  }

  updateOrderStatus(id: string, status: 'novo' | 'entregue'): Observable<OrderResponse> {
    return this.http.patch<OrderResponse>(`/api/Orders/${id}/status`, { status });
  }
}
