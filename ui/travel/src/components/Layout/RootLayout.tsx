import { Layout, Menu, Splitter } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import vacationPlanService from "../../services/vacationPlanService";
import { useMutation, useQuery } from "@tanstack/react-query";
import type { VacationPlanCatalogItem } from "../../types/vacationPlanCatalogItem";
import { Link, BrowserRouter, Routes, Route } from "react-router-dom";
import VacationPlanPage from "../../pages/VacationPlanPage";
import ConversationPage from "../../pages/ConversationPage";
import type { VacationPlanModel } from "../../types/vacationPlan";
import signalRService from "../../services/streamingService";
import type { AssistantCommand } from "../../types/assistantCommand";


const { Content, Sider } = Layout;


const RootLayout = () => {


    const { data, refetch } = useQuery({
        queryKey: ["vacationPlanCatalog"],
        queryFn: () => vacationPlanService.getCatalog()
    });

    const createVacationPlan = useMutation({
        mutationFn: async () => {
            return await vacationPlanService.create();
        },
        onSuccess: (response: VacationPlanModel) => {
            console.log("Vacation plan created successfully:", response);
            refetch();
        },
        onError: () => {
            console.error("Error creating vacation plan");
        }
    });

    const handleClick = () => { };

    const handleAddPlanClick = (e: React.MouseEvent) => {
        e.stopPropagation();

        createVacationPlan.mutate();
    };

    signalRService.on("command", (response: AssistantCommand) => {

        console.log("SignalR Assistent Command:", response);
    });

    return (<>
        <BrowserRouter>
            <Layout style={{ height: "100vh" }}>
                <Sider breakpoint="lg"
                    style={{ margin: "0px", height: "100vh", overflow: "auto" }}
                    width={200}
                    theme="light" >
                    <Menu
                        style={{ paddingTop: "32px", paddingLeft: "8px" }}
                        onClick={handleClick}
                    >
                        <Menu.ItemGroup
                            key="g3"
                            title={
                                <div style={{ color: "gray", paddingLeft: 12, display: "flex", alignItems: "center", justifyContent: "space-between", gap: 8 }}>
                                    <span>Plans</span>
                                    <PlusOutlined style={{ cursor: "pointer" }} onClick={handleAddPlanClick} />
                                </div>
                            }
                        >
                            {data?.map((catalogItem: VacationPlanCatalogItem) => (
                                <Menu.Item key={catalogItem.id}>
                                    <Link to={`/plan/${catalogItem.id}`}>
                                        <span style={{ textOverflow: "ellipsis", overflow: "hidden", whiteSpace: "nowrap", display: "block", width: "24ch" }}>
                                            {catalogItem.title}
                                        </span>
                                    </Link>
                                </Menu.Item>
                            ))}
                        </Menu.ItemGroup>
                    </Menu>
                </Sider>
                <Layout>
                    <Content style={{
                        display: "flex",
                        flexDirection: "column",

                        overflow: "hidden",
                        backgroundColor: "white",
                    }}>
                        <Splitter style={{ boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)' }}>
                            <Splitter.Panel defaultSize="50%" min="20%" max="70%" style={{ padding: 48 }}>


                                <Routes>
                                    <Route path="/plan/:id" element={<VacationPlanPage />} />
                                </Routes>



                            </Splitter.Panel>
                            <Splitter.Panel style={{ padding: 0 }}>
                                <ConversationPage />
                            </Splitter.Panel>
                        </Splitter>

                    </Content>
                </Layout>
            </Layout >
        </BrowserRouter >
    </>);
}

export default RootLayout;