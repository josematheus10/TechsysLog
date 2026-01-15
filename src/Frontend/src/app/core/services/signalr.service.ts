import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { TokenService } from '../authentication/token.service';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private readonly tokenService = inject(TokenService);
  private hubConnection: signalR.HubConnection | null = null;
  private connectionState = new BehaviorSubject<signalR.HubConnectionState>(
    signalR.HubConnectionState.Disconnected
  );

  public connectionState$ = this.connectionState.asObservable();

  startConnection(): void {
    const token = this.tokenService.getBearerToken();
    
    if (!token) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/orders', {
        accessTokenFactory: () => token.replace('Bearer ', ''),
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.connectionState.next(signalR.HubConnectionState.Connected);
      })
      .catch(err => {
        this.connectionState.next(signalR.HubConnectionState.Disconnected);
      });

    this.hubConnection.onreconnecting(() => {
      this.connectionState.next(signalR.HubConnectionState.Reconnecting);
    });

    this.hubConnection.onreconnected(() => {
      this.connectionState.next(signalR.HubConnectionState.Connected);
    });

    this.hubConnection.onclose(() => {
      this.connectionState.next(signalR.HubConnectionState.Disconnected);
    });
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection
        .stop()
        .then(() => {})
        .catch(err => {});
    }
  }

  on(eventName: string): Observable<any> {
    return new Observable(observer => {
      const registerHandler = () => {
        if (!this.hubConnection) {
          observer.error('Conexão SignalR não estabelecida');
          return;
        }

        this.hubConnection.on(eventName, (data: any) => {
          observer.next(data);
        });
      };

      // Se já está conectado, registrar imediatamente
      if (this.isConnected()) {
        registerHandler();
      } else {
        // Aguardar a conexão ser estabelecida
        const connectionSub = this.connectionState$.subscribe(state => {
          if (state === signalR.HubConnectionState.Connected) {
            registerHandler();
            connectionSub.unsubscribe();
          }
        });
      }

      return () => {
        if (this.hubConnection) {
          this.hubConnection.off(eventName);
        }
      };
    });
  }

  isConnected(): boolean {
    return this.hubConnection?.state === signalR.HubConnectionState.Connected;
  }
}
