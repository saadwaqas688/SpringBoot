import * as signalR from "@microsoft/signalr";

class SignalRService {
  constructor() {
    this.connection = null;
  }

  startConnection(token) {
    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      return Promise.resolve();
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`http://localhost:5000/chathub?access_token=${token}`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.elapsedMilliseconds < 60000) {
            return 1000;
          }
          return 5000;
        },
      })
      .build();

    return this.connection.start();
  }

  stopConnection() {
    if (this.connection) {
      return this.connection.stop();
    }
    return Promise.resolve();
  }

  on(event, callback) {
    if (this.connection) {
      this.connection.on(event, callback);
      console.log(`✅ Registered SignalR event handler: ${event}`);
    } else {
      console.warn(
        `⚠️ Cannot register event handler ${event}: connection not available`
      );
    }
  }

  off(event, callback) {
    if (this.connection) {
      this.connection.off(event, callback);
      console.log(`❌ Unregistered SignalR event handler: ${event}`);
    }
  }

  invoke(method, ...args) {
    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      return this.connection.invoke(method, ...args);
    }
    // Return a resolved promise instead of rejecting to prevent errors during cleanup
    // This is especially important when components unmount during logout
    console.warn(`⚠️ Cannot invoke ${method}: connection not established`);
    return Promise.resolve();
  }

  getConnection() {
    return this.connection;
  }

  isConnected() {
    return (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    );
  }
}

export default new SignalRService();
