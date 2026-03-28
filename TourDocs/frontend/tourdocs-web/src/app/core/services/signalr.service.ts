import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '@env/environment';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private readonly authService = inject(AuthService);
  private connections = new Map<string, signalR.HubConnection>();

  createConnection(hubName: string): signalR.HubConnection {
    const existingConnection = this.connections.get(hubName);
    if (existingConnection) {
      return existingConnection;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.signalRUrl}/${hubName}`, {
        accessTokenFactory: () => this.authService.getToken() || ''
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.connections.set(hubName, connection);
    return connection;
  }

  async startConnection(hubName: string): Promise<void> {
    const connection = this.connections.get(hubName);
    if (!connection) {
      throw new Error(`No connection found for hub: ${hubName}`);
    }
    if (connection.state === signalR.HubConnectionState.Disconnected) {
      try {
        await connection.start();
      } catch (err) {
        console.error(`Error starting SignalR connection for ${hubName}:`, err);
        throw err;
      }
    }
  }

  async stopConnection(hubName: string): Promise<void> {
    const connection = this.connections.get(hubName);
    if (connection && connection.state !== signalR.HubConnectionState.Disconnected) {
      await connection.stop();
    }
    this.connections.delete(hubName);
  }

  on<T>(hubName: string, methodName: string, callback: (data: T) => void): void {
    const connection = this.connections.get(hubName);
    if (connection) {
      connection.on(methodName, callback);
    }
  }

  off(hubName: string, methodName: string): void {
    const connection = this.connections.get(hubName);
    if (connection) {
      connection.off(methodName);
    }
  }

  async invoke<T>(hubName: string, methodName: string, ...args: unknown[]): Promise<T> {
    const connection = this.connections.get(hubName);
    if (!connection) {
      throw new Error(`No connection found for hub: ${hubName}`);
    }
    return connection.invoke<T>(methodName, ...args);
  }

  async stopAll(): Promise<void> {
    const stopPromises = Array.from(this.connections.keys()).map(hub => this.stopConnection(hub));
    await Promise.all(stopPromises);
  }
}
