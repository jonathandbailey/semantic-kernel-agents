import { create } from "zustand";
import type { User } from "../types/user";

interface UserState {
    user: User;
    setUser: (user: User) => void;
    clearUser: () => void;
    updateUser: (updates: Partial<User>) => void;
}

const defaultUser: User = {
    id: "",
    sessionId: "",
    source: ""
};

export const useUserStore = create<UserState>((set, get) => ({
    user: defaultUser,
    setUser: (user: User) => set({ user }),
    clearUser: () => set({ user: defaultUser }),
    updateUser: (updates: Partial<User>) =>
        set({ user: { ...get().user, ...updates } })
}));