import { Card, createStyles, Box } from "@mantine/core";
import { AppHeader, Sidebar } from "core/components";
import { useSidebar } from "../../context/SidebarContext";

const useStyles = createStyles((theme) => ({
  layoutHeight: {
    height: "92vh",
  },

  layoutContainer: {
    padding: theme.spacing.md,
    backgroundColor: theme.colorScheme === 'dark' ? theme.colors.dark[8] : theme.colors.gray[0],
  },

  bodyContainer: {
    border: `1px solid ${theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[3]}`,
    height: "100%",
    borderRadius: "0.4rem",
    overflowY: "auto",
  },

  bodyRow: {
    display: "flex",
    width: "100%",
    minHeight: 0,
  },

  sidebarWrap: {
    flexShrink: 0,
    width: 60,
  },

  sidebarWrapExpanded: {
    width: "16.666667%",
    minWidth: 200,
    maxWidth: 280,
  },

  contentWrap: {
    flex: "1 1 0",
    minWidth: 0,
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
    <Box m={0} p={0}>
      <Box m={0} p={0}>
        <AppHeader franchiseName={franchiseName} />
      </Box>

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
          <section className={cx(classes.layoutHeight, classes.layoutContainer)}>
            <Card p="0" m="0" shadow="sm" className={classes.bodyContainer}>
              {children}
            </Card>
          </section>
        </Box>
      </Box>
    </Box>
  );
};
