import { create } from "zustand";
import type { VacationPlanModel } from "../types/vacationPlan";


interface VacationPlanState {
    vacationPlan: VacationPlanModel;
    setVacationPlan: (vacationPlan: VacationPlanModel) => void;
}

const defaultVacationPlan: VacationPlanModel = {
    id: "",
    title: "",
    stages: []
}

export const useVacationPlanStore = create<VacationPlanState>((set, get) => ({
    vacationPlan: defaultVacationPlan,
    setVacationPlan: (vacationPlan: VacationPlanModel) => set({ vacationPlan })
}));