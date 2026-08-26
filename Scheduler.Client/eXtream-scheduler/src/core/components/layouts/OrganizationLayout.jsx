import { useEffect, useState } from "react";
import { Outlet, useLocation, useNavigate, useParams } from "react-router-dom";
import { IconNotes, IconSitemap, IconSettings } from "@tabler/icons";
import { Layout } from "core/components";
import { localStoreService, organizationService } from "core/services";
import { stringhelper } from "shared/utils";
import { usePermissions } from "core/context/PermissionContext";

export const OrganizationLayout = () => {
  const [organizationSidebarMenu, setOrganizationSidebarMenu] = useState([]);
  const [selectedMenu, setSelectedMenu] = useState(null);
  const { canView, loading: permissionsLoading, initialized } = usePermissions();
  const navigate = useNavigate();
  const { orgName, organizationID } = useParams();
  const location = useLocation();

  useEffect(() => {
    if (initialized) {
      createOrganizationSideBarMenus();
    }
  }, [canView, initialized]);

  const createOrganizationSideBarMenus = async () => {
    const userID = localStoreService.getUserID();
    const response = await organizationService.getOrganizationList(userID);
    // Build organization menus first, then append cross-organization settings at the end.
    // This guarantees "Settings" renders below the organization list.
    let organizationMenus = [];
    let settingsMenus = [];
    
    // Check if user has permission to view organizations
    if (canView('organizations')) {
      if (response && Array.isArray(response)) {
        const orgs = response;
        organizationMenus = orgs?.map((org) => {
          return {
            id: org.id,
            label: org.name,
            icon: IconSitemap,
            link: `/organizations/${stringhelper.removeSpaceFromString(org.name)}`,
            menuId: 'organizations'
          };
        });
      }
    }

    // Organization Settings entry: show if user can open org settings OR can browse organizations.
    // (API may omit parent rows; strict permission mode denies missing keys — org list + Settings stay aligned.)
    if (canView('organization-settings') || canView('organizations')) {
      settingsMenus.push({
        id: -1,
        label: 'Settings',
        icon: IconSettings,
        link: '',
        menuId: 'organization-settings'
      });
    }

    const menus = [...organizationMenus, ...settingsMenus];
    setOrganizationSidebarMenu(menus);

    // Selection logic:
    // - If route is /organization-settings, select Settings menu item (id: -1).
    // - Otherwise select org item by orgName param (or redirect to first item).
    const isOnOrganizationSettingsRoute = location.pathname.includes("organization-settings");

    if (isOnOrganizationSettingsRoute) {
      const settingsMenu = menus.find((m) => m?.id === -1);
      if (settingsMenu) {
        setSelectedMenu(settingsMenu);
      }
      return;
    }

    if (orgName) {
      const matchedMenu = menus?.find(
        (x) => stringhelper.removeSpaceFromString(x.label) === orgName
      );

      if (matchedMenu) {
        setSelectedMenu(matchedMenu);
      } else {
        // If no organization matches, redirect to first available menu
        const firstAvailableMenu = getFirstAvailableMenu(menus);
        if (firstAvailableMenu) {
          console.log("Redirecting to first available organization menu:", firstAvailableMenu);
          fallbackRedirection(firstAvailableMenu);
        }
      }
    } else if (menus.length > 0) {
      const firstAvailableMenu = getFirstAvailableMenu(menus);
      if (firstAvailableMenu) {
        console.log("Redirecting to first available organization menu:", firstAvailableMenu);
        fallbackRedirection(firstAvailableMenu);
      }
    }
  };

  const getFirstAvailableMenu = (menus) => {
    // Find the first menu item that has a link (not just a parent menu)
    for (const menu of menus) {
      if (menu.link && menu.link !== '') {
        return menu;
      }
      // Check children if this is a parent menu
      if (menu.childrenLinks && menu.childrenLinks.length > 0) {
        for (const child of menu.childrenLinks) {
          if (child.link && child.link !== '') {
            return child;
          }
        }
      }
    }
    return null;
  };

  const fallbackRedirection = (menu) => {
    setSelectedMenu(menu);
    navigate(menu.link);
  };

  const handleOrganizationSidebarMenu = (sidebarMenu) => {
    if (sidebarMenu.id === -1) {
      // selectedMenu.id is the org id when on an organization route; for safety fall back to route param.
      const selectedOrganizationID =
        selectedMenu?.id !== -1 ? selectedMenu?.id : organizationID;

      const link = `/organizations/${selectedOrganizationID}/organization-settings`;
      setSelectedMenu({ ...sidebarMenu, link });
      navigate(link);
    }
    else {
      setSelectedMenu(sidebarMenu);
      navigate(sidebarMenu.link);
    }
  };

  // Show loading state while permissions are being loaded
  if (permissionsLoading || !initialized) {
    return (
      <Layout
        sidebarMenu={[]}
        selectedMenu={null}
        onSidebarMenu={() => {}}
      >
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
          <div>Loading permissions...</div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout
      sidebarMenu={organizationSidebarMenu}
      selectedMenu={selectedMenu}
      onSidebarMenu={handleOrganizationSidebarMenu}
    >
      <Outlet context={{ selectedMenu }} />
    </Layout>
  );
};