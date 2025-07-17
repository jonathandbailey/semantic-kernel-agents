import ChatInput from "../components/ChatInput/ChatInput";
import promptService from "../services/promptService";
import { useConversationStore } from "../store/useConversationStore";
import { useMutation } from "@tanstack/react-query";
import type { SendUserResponse } from "../types/sendUserResponse";
import signalRService from "../services/streamingService";
import { useUserStore } from "../store/useUserStore";
import type { Interaction } from "../types/interaction";
import { useEffect, useRef, useState } from "react";
import UserMessage from "../components/UserMessage/UserMessage";
import AssistantMessage from "../components/AssistantMessage/AssistantMessage";
import { useVacationPlanStore } from "../store/useVacationPlanStore";
import { List } from "antd";




const ConversationPage = () => {
    const [uiInteractions, setUiInteractions] = useState<Interaction[]>([]);


    const addMessage = useConversationStore(state => state.addMessage);

    const user = useUserStore(state => state.user);
    const updateUser = useUserStore(state => state.updateUser);
    const vacationPlanModel = useVacationPlanStore(state => state.vacationPlan);

    const lastMessageRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        if (lastMessageRef.current) {
            lastMessageRef.current.scrollIntoView({ behavior: "smooth" });
        }
    }, [uiInteractions]);

    signalRService.on("user", (response: SendUserResponse) => {

        console.log("Received response from SignalR:", response);
        console.log("Current UI Interactions:", uiInteractions);
        setUiInteractions((prev) =>
            prev.map((interaction) => {
                if (interaction.id === response.id) {
                    console.log("Updating interaction:", interaction);
                    return {
                        ...interaction,
                        assistantMessage: {
                            ...interaction.assistantMessage,
                            text: interaction.assistantMessage.text + response.message,
                            isLoading: false,
                            hasError: false,
                            errorMessage: "",
                        },
                    };
                }

                return interaction;
            })
        );
    });

    const promptMutation = useMutation({
        mutationFn: async ({ id, message }: { id: string; message: string }) => {
            const request = { id, message, sessionId: user.sessionId, source: user.source, vacationPlanId: vacationPlanModel.id } as SendUserResponse;
            console.log(request)
            return await promptService.generate(request);
        },
        onSuccess: (response: SendUserResponse) => {
            updateUser({ sessionId: response.sessionId, source: response.source });
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
        const newInteraction: Interaction = {
            id: crypto.randomUUID(),
            userMessage: {
                id: crypto.randomUUID(),
                sender: "user" as "user",
                text: value
            },
            assistantMessage: {
                id: crypto.randomUUID(),
                text: "",
                isLoading: true,
                hasError: false,
                errorMessage: "",
            },
        }

        setUiInteractions((prev) => [...prev, newInteraction]);
        promptMutation.mutate({ message: value, id: newInteraction.id });
    };

    return (
        <div style={{ display: "flex", flexDirection: "column", height: "95vh" }}>

            <List style={{ flex: 1, overflowY: "auto", padding: "48px" }} split={false}>
                {uiInteractions.map((interaction, index) => {
                    const isLast = index === uiInteractions.length - 1;
                    return (
                        <List.Item key={interaction.id} style={{ width: "100%", display: "flex", flexDirection: "column" }} ref={isLast ? lastMessageRef : null}>
                            <UserMessage message={interaction.userMessage} />
                            <AssistantMessage message={interaction.assistantMessage} />
                        </List.Item>
                    );
                })}
            </List>


            <div style={{ position: "sticky", bottom: 0, background: "#fff", zIndex: 10, paddingRight: 48, paddingLeft: 48 }}>
                <ChatInput onEnter={handleOnEnter} />
            </div>
        </div>
    );
}

export default ConversationPage;