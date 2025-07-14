
import { Tabs, Typography } from "antd";
import type { VacationPlanModel } from "../../types/vacationPlan";

const { Title } = Typography;

interface VacationPlanProps {
    item: VacationPlanModel;
}

const VacationPlan = ({ item }: VacationPlanProps) => {
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