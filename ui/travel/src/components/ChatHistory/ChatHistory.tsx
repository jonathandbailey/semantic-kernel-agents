import type { Message } from "../../types/message";

interface ChatHistoryProps {
    messages: Message[];
}

const ChatHistory = ({ messages }: ChatHistoryProps) => {
    return (
        <div className="chat-history">
            {messages.map((msg) => (
                <div key={msg.id} className={`message ${msg.sender}`}>
                    <span className="sender">{msg.sender === 'user' ? 'You' : 'Assistant'}:</span>
                    <p className="text">{msg.text}</p>
                </div>
            ))}
        </div>
    );
}

export default ChatHistory;