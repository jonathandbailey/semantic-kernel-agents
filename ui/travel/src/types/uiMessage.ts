export interface UiMessage {
    id: string;
    text: string;
    isLoading: boolean;
    hasError: boolean;
    errorMessage: string;
}