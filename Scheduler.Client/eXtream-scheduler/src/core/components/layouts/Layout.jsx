import { createStyles, Box } from "@mantine/core";
import { AppHeader, Sidebar } from "core/components";
import { useSidebar } from "../../context/SidebarContext";
import { HEADER_HEIGHT } from "theme";

const useStyles = createStyles((theme) => ({
  shell: {
    display: "flex",
    flexDirection: "column",
    height: "100vh",
    overflow: "hidden",
    "--cs-header-height": `${HEADER_HEIGHT}px`,
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[8] : "#f8fafc",
  },

  bodyRow: {
    display: "flex",
    flex: 1,
    minHeight: 0,
    width: "100%",
  },

  sidebarWrap: {
    flexShrink: 0,
    width: 64,
    transition: "width 0.2s ease",
  },

  sidebarWrapExpanded: {
    width: 240,
    minWidth: 200,
    maxWidth: 280,
  },

  contentWrap: {
    flex: "1 1 0",
    minWidth: 0,
    display: "flex",
    flexDirection: "column",
    minHeight: 0,
  },

  pageCanvas: {
    flex: 1,
    minHeight: 0,
    paddingTop: 8,
    paddingLeft: theme.spacing.md,
    paddingRight: theme.spacing.md,
    paddingBottom: theme.spacing.md,
    overflow: "auto",
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[8] : "#f8fafc",
  },

  pageInner: {
    minHeight: "100%",
    height: "100%",
  },
}));

export const Layout = ({
  sidebarMenu,
  selectedMenu,
  onSidebarMenu,
  children,
  franchiseName
}) => {
  const { classes, cx } = useStyles();
  const { isCollapsed } = useSidebar();

  return (
    <Box className={classes.shell}>
      <AppHeader franchiseName={franchiseName} />

      <Box className={classes.bodyRow}>
        <Box
          className={cx(classes.sidebarWrap, !isCollapsed && classes.sidebarWrapExpanded)}
        >
          <Sidebar
            sidebarMenu={sidebarMenu}
            selectedMenu={selectedMenu}
            onSidebarMenu={onSidebarMenu}
          />
        </Box>

        <Box className={classes.contentWrap}>
          <Box className={cx(classes.pageCanvas, "app-page-canvas")}>
            <Box className={cx(classes.pageInner, "app-page-content")}>
              {children}
            </Box>
          </Box>
        </Box>
      </Box>
    </Box>
  );
};
