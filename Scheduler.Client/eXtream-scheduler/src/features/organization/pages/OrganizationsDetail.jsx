import { Tabs, createStyles, Center, Text, Alert } from "@mantine/core";
import { AppContainer } from "shared/components";
import BasicSetting from "../components/BasicSetting";

import Lookup from "../components/Lookup";
import { useParams } from "react-router-dom";
import { useEffect, useState, useMemo } from "react";
import { organizationService } from "core/services";
import { notifications } from "@mantine/notifications";
import Services from '../components/Services';
import OrganizationPayers from '../components/OrganizationPayers';
import OrganizationFunding from '../components/OrganizationFunding';
import RolePermissionManagement from '../components/RolePermissionManagement';
import RatesAndBillingSettings from '../components/RatesAndBillingSettings';
import { LoginHistory } from "features/loginHistory";
import { usePermissions } from "core/context/PermissionContext";

/** Labels must match MenuName in tblMenu where possible; menuId matches tblMenu.MenuId for RBAC. */
const ORGANIZATION_SETTINGS_TABS = [
    { label: "Basic Settings", menuId: "basic-settings" },
    { label: "Rates & Billing", menuId: "rates-billing" },
    { label: "Lookups", menuId: "lookups" },
    { label: "Services", menuId: "services" },
    { label: "Payers", menuId: "org-payers" },
    { label: "Organization funding", menuId: "org-funding" },
    { label: "Access Control", menuId: "access-control" },
    { label: "Login History", menuId: "login-history" },
];

const useStyles = createStyles(() => ({
    tab: {
        padding: "1rem 1rem"
    },
    panel: {
        height: "100%",
        paddingTop: "1.25rem",
        overflow: "auto"
    },
    label: {
        color: "green",
        "&:focus": {
            borderColor: "#ced4da !important"
        }
    }
}));

const OrganizationDetail = () => {
    const [org, setOrg] = useState();
    const [activeTab, setActiveTab] = useState(null);

    const { classes } = useStyles();
    const { organizationID } = useParams();
    const { canView, loading: permissionsLoading, initialized, permissions } = usePermissions();

    const visibleTabs = useMemo(() => {
        return ORGANIZATION_SETTINGS_TABS.filter((tab) => canView(tab.menuId));
    }, [permissions, initialized, canView]);

    useEffect(() => {
        organizationService.getOrganizationById(organizationID)
            .then((response) => {
                if (response) {
                    setOrg(response);
                } else {
                    notifications.show({
                        title: "Error",
                        message: "Failed to fetch organization details",
                        color: "red",
                    });
                }
            })
            .catch((error) => {
                console.error("Failed to fetch organization details:", error);
                notifications.show({
                    title: "Error",
                    message: "Failed to fetch organization details",
                    color: "red",
                });
            });
    }, [organizationID]);

    useEffect(() => {
        if (!initialized || permissionsLoading) return;
        if (visibleTabs.length === 0) {
            setActiveTab(null);
            return;
        }
        setActiveTab((prev) => {
            if (prev && visibleTabs.some((t) => t.label === prev)) return prev;
            return visibleTabs[0].label;
        });
    }, [initialized, permissionsLoading, visibleTabs]);

    const renderTabPanel = (tabLabel) => {
        switch (tabLabel) {
            case "Basic Settings":
                return <BasicSetting organization={org} />;
            case "Rates & Billing":
                return <RatesAndBillingSettings organizationId={organizationID} organizationName={org?.name} />;
            case "Lookups":
                return <Lookup organizationid={organizationID} />;
            case "Services":
                return <Services organizationId={organizationID} />;
            case "Payers":
                return <OrganizationPayers organizationId={organizationID} />;
            case "Organization funding":
                return <OrganizationFunding organizationId={organizationID} />;
            case "Access Control":
                return <RolePermissionManagement organizationId={organizationID} />;
            case "Login History":
                return <LoginHistory />;
            default:
                return null;
        }
    };

    if (!initialized || permissionsLoading) {
        return (
            <AppContainer title={org?.name ? `${org.name} Settings` : "Organization Settings"}>
                <Center p="xl">
                    <Text color="dimmed">Loading permissions…</Text>
                </Center>
            </AppContainer>
        );
    }

    if (visibleTabs.length === 0) {
        return (
            <AppContainer title={`${org?.name ?? ""} Settings`}>
                <Alert color="yellow" title="No access">
                    You do not have permission to view any organization settings sections for this organization.
                </Alert>
            </AppContainer>
        );
    }

    return (
        <AppContainer title={org?.name ? `${org.name} Settings` : "Organization Settings"}>
            <Tabs
                value={activeTab}
                onTabChange={setActiveTab}
                variant="outline"
            >
                <Tabs.List>
                    {visibleTabs.map((tab) => (
                        <Tabs.Tab className={classes.tab} value={tab.label} key={tab.label}>
                            {tab.label}
                        </Tabs.Tab>
                    ))}
                </Tabs.List>

                {visibleTabs.map((tab) => (
                    <Tabs.Panel value={tab.label} key={tab.label} className={classes.panel}>
                        {renderTabPanel(tab.label)}
                    </Tabs.Panel>
                ))}
            </Tabs>
        </AppContainer>
    );
};

export default OrganizationDetail;
