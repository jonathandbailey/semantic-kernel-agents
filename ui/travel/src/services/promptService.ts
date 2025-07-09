import type { SendUserRequest } from "../types/sendUserRequest";
import type { SendUserResponse } from "../types/sendUserResponse";
import apiClient from "./apiClient";

const promptService = {
    generate: async (promptRequest: SendUserRequest): Promise<SendUserResponse> => {
        const response = await apiClient.post<SendUserResponse>(
            "/api/todo/send",
            promptRequest
        );
        return response.data;
    },
};

export default promptService;