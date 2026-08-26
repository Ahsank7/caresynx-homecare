import { Navbar, ScrollArea, createStyles } from "@mantine/core";
import { LinksGroup } from "core/components";
import { useSidebar } from "../../context/SidebarContext";

const useStyles = createStyles((theme) => ({
  navbar: {
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[6] : theme.white,
    paddingBottom: 0,
    borderRight: 0,
    height: "92vh",
    transition: "width 0.2s ease, min-width 0.2s ease",
  },
}));

const COLLAPSED_WIDTH = 60;

export const Sidebar = ({ sidebarMenu, selectedMenu, onSidebarMenu }) => {
  const { classes } = useStyles();
  const { isCollapsed } = useSidebar();

  // For org portals, "Settings" is a generic cross-organization option.
  // Render it as a footer so it doesn't appear mixed with the organization list.
  const footerMenus = (sidebarMenu || []).filter(
    (m) => m?.id === -1 || m?.menuId === "organization-settings"
  );
  const mainMenus = (sidebarMenu || []).filter(
    (m) => !(m?.id === -1 || m?.menuId === "organization-settings")
  );

  return (
    <Navbar
      className={classes.navbar}
      width={isCollapsed ? { base: COLLAPSED_WIDTH } : { base: "100%" }}
    >
      <Navbar.Section grow component={ScrollArea}>
        {mainMenus.map((menu) => (
          <LinksGroup
            key={menu.id}
            menu={menu}
            selectedMenu={selectedMenu}
            onSidebarMenu={onSidebarMenu}
            isCollapsed={isCollapsed}
          />
        ))}
      </Navbar.Section>

      {footerMenus.length > 0 && (
        <Navbar.Section>
          {footerMenus.map((menu) => (
            <LinksGroup
              key={menu.id}
              menu={menu}
              selectedMenu={selectedMenu}
              onSidebarMenu={onSidebarMenu}
              isCollapsed={isCollapsed}
            />
          ))}
        </Navbar.Section>
      )}
    </Navbar>
  );
};
