import type { VacationPlanModel } from "../types/vacationPlan";
import type { VacationPlanCatalogItem } from "../types/vacationPlanCatalogItem";
import apiClient from "./apiClient";

const vacationPlanService = {
    getCatalog: async (): Promise<VacationPlanCatalogItem[]> => {
        const response = await apiClient.get<VacationPlanCatalogItem[]>(
            "/api/vacationplan/catalog"
        );
        return response.data;
    },

    get: async (id: string): Promise<VacationPlanModel> => {
        const response = await apiClient.get<VacationPlanModel>(
            "/api/vacationplan/" + id
        );
        return response.data;
    }
}

export default vacationPlanService