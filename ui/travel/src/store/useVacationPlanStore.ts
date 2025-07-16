import { create } from "zustand";
import type { VacationPlanModel } from "../types/vacationPlan";


interface VacationPlanState {
    vacationPlan: VacationPlanModel;
    vacationPlans: VacationPlanModel[];
    addVacationPlan: (vacationPlan: VacationPlanModel) => void;
    setVacationPlan: (vacationPlan: VacationPlanModel) => void;
}

const defaultVacationPlan: VacationPlanModel = {
    id: "",
    title: "",
    stages: []
}

export const useVacationPlanStore = create<VacationPlanState>((set, get) => ({
    vacationPlans: [],
    vacationPlan: defaultVacationPlan,
    addVacationPlan: (vacationPlan: VacationPlanModel) => set(state => ({
        vacationPlans: [...state.vacationPlans, vacationPlan]
    })),
    setVacationPlan: (vacationPlan: VacationPlanModel) => set({ vacationPlan })
}));