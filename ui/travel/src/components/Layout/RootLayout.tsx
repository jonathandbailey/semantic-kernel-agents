import { Layout, Menu, Splitter } from "antd";
import vacationPlanService from "../../services/vacationPlanService";
import { useQuery } from "@tanstack/react-query";
import type { VacationPlanCatalogItem } from "../../types/vacationPlanCatalogItem";
import { Link, BrowserRouter, Routes, Route } from "react-router-dom";
import VacationPlanPage from "../../pages/VacationPlanPage";
import ConversationPage from "../../pages/ConversationPage";


const { Content, Sider } = Layout;


const RootLayout = () => {



    const { data } = useQuery({
        queryKey: ["vacationPlanCatalog"],
        queryFn: () => vacationPlanService.getCatalog()
    });

    const handleClick = () => { };

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
                        <Menu.ItemGroup key="g3" title={<div style={{ color: "gray", paddingLeft: 12 }}>Plans</div>}>
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
                    <Splitter style={{ boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)' }}>
                        <Splitter.Panel defaultSize="50%" min="20%" max="70%">
                            <Content style={{
                                display: "flex",
                                flexDirection: "column",

                                overflow: "hidden",
                                backgroundColor: "white",
                            }}>
                                <div style={{ padding: "32px" }}>
                                    <Routes>
                                        <Route path="/plan/:id" element={<VacationPlanPage />} />
                                    </Routes>
                                </div>

                            </Content>
                        </Splitter.Panel>
                        <Splitter.Panel>
                            <ConversationPage />
                        </Splitter.Panel>
                    </Splitter>


                </Layout>
            </Layout >
        </BrowserRouter >
    </>);
}

export default RootLayout;