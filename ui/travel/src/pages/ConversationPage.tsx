import ChatHistory from "../components/ChatHistory/ChatHistory";
import ChatInput from "../components/ChatInput/ChatInput";
import promptService from "../services/promptService";
import { useConversationStore } from "../store/useConversationStore";
import { useMutation } from "@tanstack/react-query";


const ConversationPage = () => {
    const messages = useConversationStore(state => state.messages);
    const addMessage = useConversationStore(state => state.addMessage);

    const promptMutation = useMutation({
        mutationFn: async (message: string) => {

            const request = { message: message, sessionId: "" };
            console.log(request)
            return await promptService.generate(request);
        },
        onSuccess: (response) => {
            addMessage({
                id: crypto.randomUUID(),
                sender: "assistant" as "assistant",
                text: response.message,
            });
        },
        onError: () => {

            addMessage({
                id: crypto.randomUUID(),
                sender: "assistant" as "assistant",
                text: "Sorry, something went wrong.",
            });
        }
    });

    const handleOnEnter = (value: string) => {
        const newMessage = {
            id: crypto.randomUUID(),
            sender: "user" as "user",
            text: value
        };
        addMessage(newMessage);

        promptMutation.mutate(value);
    };

    return (
        <>
            <ChatHistory messages={messages} />
            <ChatInput onEnter={handleOnEnter} />
        </>
    );
}

export default ConversationPage;