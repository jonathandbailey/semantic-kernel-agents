import { Alert, Card } from "antd";
import type { UiMessage } from "../../types/uiMessage";
import Markdown from "react-markdown";

interface AssistantMessageProps {
    message: UiMessage
}

const AssistantMessage = ({ message }: AssistantMessageProps) => {
    return (
        <Card
            style={{
                marginBottom: 16,
                marginLeft: 96,
                textAlign: "left",
                alignSelf: "center",
                maxWidth: 800,
                boxShadow: "0 0px 0px rgba(0, 0, 0, 0.1)",
            }}
            variant="borderless"
            loading={message.isLoading}
        >
            {message.hasError ? (
                <Alert
                    message={message.errorMessage}
                    description="There was an error processing your request. Please try again."
                    type="error"
                    showIcon
                />
            ) : (
                <Markdown>{message.text}</Markdown>
            )}
        </Card>
    );
};

export default AssistantMessage;