import { Card, Input } from "antd";
import { useState } from "react";

interface ChatInputProps {
    onEnter: (value: string) => void;
}

const ChatInput = ({ onEnter }: ChatInputProps) => {
    const [inputValue, setInputValue] = useState<string>("");

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key === "Enter") {
            onEnter(inputValue);
            setInputValue("");
        }
    };
    return (
        <Card
            style={{

                alignSelf: "center",
                marginBottom: 48,
                marginTop: 48,
                boxShadow: "0 4px 8px rgba(0, 0, 0, 0.1)",
            }}
        >
            <Input
                placeholder="Where would you like to go?"
                variant="borderless"
                value={inputValue}
                onChange={(e) => setInputValue(e.target.value)}
                onKeyDown={handleKeyDown}
                style={{ flex: 1, width: "100%" }}
            />
        </Card>
    );
}

export default ChatInput;