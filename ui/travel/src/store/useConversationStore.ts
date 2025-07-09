import { create } from 'zustand';
import type { Message } from '../types/message';



interface ConversationState {
    messages: Message[];
    addMessage: (msg: Message) => void;
    clearMessages: () => void;
}

export const useConversationStore = create<ConversationState>((set) => ({
    messages: [],
    addMessage: (msg) => set(state => ({ messages: [...state.messages, msg] })),
    clearMessages: () => set({ messages: [] }),
}));
