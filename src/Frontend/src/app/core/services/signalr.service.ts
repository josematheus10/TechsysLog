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
      console.warn('Não foi possível iniciar a conexão SignalR: token não encontrado');
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
        console.log('Conexão SignalR estabelecida com sucesso');
        this.connectionState.next(signalR.HubConnectionState.Connected);
      })
      .catch(err => {
        console.error('Erro ao iniciar conexão SignalR:', err);
        this.connectionState.next(signalR.HubConnectionState.Disconnected);
      });

    this.hubConnection.onreconnecting(() => {
      console.log('Reconectando SignalR...');
      this.connectionState.next(signalR.HubConnectionState.Reconnecting);
    });

    this.hubConnection.onreconnected(() => {
      console.log('SignalR reconectado');
      this.connectionState.next(signalR.HubConnectionState.Connected);
    });

    this.hubConnection.onclose(() => {
      console.log('Conexão SignalR fechada');
      this.connectionState.next(signalR.HubConnectionState.Disconnected);
    });
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection
        .stop()
        .then(() => console.log('Conexão SignalR encerrada'))
        .catch(err => console.error('Erro ao encerrar conexão SignalR:', err));
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
          console.log(`Evento SignalR recebido: ${eventName}`, data);
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
