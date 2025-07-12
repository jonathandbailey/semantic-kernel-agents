import type { Message } from "./message";
import type { UiMessage } from "./uiMessage";

export interface Interaction {
    id: string;
    userMessage: Message;
    assistantMessage: UiMessage;
}

