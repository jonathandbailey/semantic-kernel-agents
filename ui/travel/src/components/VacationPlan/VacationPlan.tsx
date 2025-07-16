import { Tabs, Typography } from "antd";
import type { VacationPlanModel } from "../../types/vacationPlan";
import { useVacationPlanStore } from "../../store/useVacationPlanStore";
import { useEffect } from "react";

const { Title } = Typography;

interface VacationPlanProps {
    item: VacationPlanModel;
}

const VacationPlan = ({ item }: VacationPlanProps) => {
    const setVacationPlan = useVacationPlanStore((state) => state.setVacationPlan);

    useEffect(() => {
        setVacationPlan(item);
    }, [item, setVacationPlan]);

    return (
        <>
            <Title>
                {item.title}
            </Title>

            <Tabs>
                {item.stages?.map((stage, idx) => (
                    <Tabs.TabPane tab={stage.title} key={stage.id ?? idx}>
                        {stage.description}
                    </Tabs.TabPane>
                ))}
            </Tabs>
        </>
    );
}

export default VacationPlan;