import ChatHistory from "../components/ChatHistory/ChatHistory";
import ChatInput from "../components/ChatInput/ChatInput";
import { useConversationStore } from "../store/useConversationStore";

const ConverstionPage = () => {

    const messages = useConversationStore(state => state.messages);
    const addMessage = useConversationStore(state => state.addMessage);

    const handleOnEnter = (value: string) => {



        const newMessage = {
            id: crypto.randomUUID(),
            sender: "user" as "user",
            text: value
        };

        addMessage(newMessage);
    };


    return (
        <>
            <ChatHistory messages={messages} />
            <ChatInput onEnter={handleOnEnter} />
        </>
    );
}

export default ConverstionPage;