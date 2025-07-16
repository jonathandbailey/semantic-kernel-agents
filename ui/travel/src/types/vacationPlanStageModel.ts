import type { StageTaskModel } from "./stageTaskModel";

export interface VacationPlanStageModel {
    id: string;
    title: string;
    description: string;
    tasks: StageTaskModel[];
}