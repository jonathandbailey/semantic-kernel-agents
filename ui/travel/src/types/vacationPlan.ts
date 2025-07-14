import type { VacationPlanStageModel } from "./vacationPlanStageModel";

export interface VacationPlanModel {
    id: string;
    title: string;
    stages: VacationPlanStageModel[];
}