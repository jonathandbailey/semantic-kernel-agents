import * as signalR from "@microsoft/signalr";

type CallbackFn = (...args: any[]) => void;

class SignalRService {
    private connection: signalR.HubConnection | null = null;
    private handlers: Record<string, CallbackFn> = {};

    async initialize(): Promise<void> {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(import.meta.env.VITE_API_BASE_URL + "/hub")
            .withAutomaticReconnect()
            .build();

        try {
            await this.connection.start();
            console.log("SignalR connected");
        } catch (err) {
            console.error("SignalR connection error: ", err);
        }
    }

    on(event: string, callback: CallbackFn): void {
        this.handlers[event] = callback;
        if (this.connection) {
            this.connection.off(event);
            this.connection.on(event, callback);
        }
    }

    stop(): void {
        if (this.connection) {
            this.connection.stop();
            this.connection = null;
            this.handlers = {};
        }
    }
}

const signalRService = new SignalRService();
export default signalRService;