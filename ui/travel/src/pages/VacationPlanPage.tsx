import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import vacationPlanService from "../services/vacationPlanService";
import VacationPlan from "../components/VacationPlan/VacationPlan";

const VacationPlanPage = () => {
    const { id } = useParams();

    const { data } = useQuery({
        queryKey: ["vacationPlan", id],
        queryFn: () => {
            if (!id) throw new Error("Vacation plan id is required");
            return vacationPlanService.get(id);
        },
        enabled: !!id
    });

    return (
        <>
            {data && <VacationPlan item={data} />}
        </>
    );
}

export default VacationPlanPage;