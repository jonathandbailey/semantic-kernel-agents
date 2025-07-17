import { Card } from "antd";
import type { Message } from "../../types/message";

interface UserMessageProps {
    message: Message;
}

const UserMessage = ({ message }: UserMessageProps) => {
    return (
        <>
            <Card
                style={{
                    width: 400,
                    marginLeft: "auto",
                    marginBottom: 16,
                    marginRight: 0,
                    textAlign: "left",
                    backgroundColor: "#f5f5f5",
                }}
            >
                {message.text}
            </Card>
        </>
    );
};

export default UserMessage;

