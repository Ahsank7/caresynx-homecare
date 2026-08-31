import {
  createStyles,
  Header,
  Group,
  Burger,
  Menu,
  UnstyledButton,
  Avatar,
  Text,
  Stack,
  ActionIcon,
  Tooltip,
  Breadcrumbs,
  Box,
  MediaQuery,
} from "@mantine/core";
import { IconChevronRight, IconLogout, IconSun, IconMoon, IconLock, IconBuilding } from "@tabler/icons";
import { useNavigate } from "react-router-dom";
import { localStoreService, loginHistoryService, organizationService } from "core/services";
import { useTheme } from "../context/ThemeContext";
import { useSidebar } from "../context/SidebarContext";
import { buildImageUrl } from "../utils/urlHelper";
import { ChangePassword } from "shared/components/user/ChangePassword";
import { AppTable, AppModal } from "shared/components";
import React, { useState, useEffect } from "react";
import { UserType } from "core/enum";
import { NotificationBell } from "./NotificationBell";
import { HEADER_HEIGHT } from "theme";

const useStyles = createStyles((theme) => ({
  header: {
    paddingLeft: theme.spacing.xs,
    paddingRight: theme.spacing.xs,
    borderBottom: `1px solid ${theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[2]}`,
    height: HEADER_HEIGHT,
    minHeight: HEADER_HEIGHT,
    maxHeight: HEADER_HEIGHT,
    flexShrink: 0,
    boxSizing: "border-box",
    zIndex: 200,
    overflow: "visible",
    backgroundColor: theme.colorScheme === "dark" ? theme.colors.dark[7] : theme.white,
    boxShadow: theme.colorScheme === "dark" ? "none" : "0 1px 0 rgba(15, 23, 42, 0.04)",
    [`@media (min-width: ${theme.breakpoints.sm}px)`]: {
      paddingLeft: theme.spacing.md,
      paddingRight: theme.spacing.md,
    },
  },

  headerInner: {
    height: "100%",
    width: "100%",
    flexWrap: "nowrap",
    overflow: "visible",
  },

  leftSection: {
    flex: "0 1 auto",
    minWidth: 0,
    gap: theme.spacing.xs,
    [`@media (min-width: ${theme.breakpoints.sm}px)`]: {
      gap: theme.spacing.sm,
    },
  },

  contextSection: {
    flex: "1 1 0",
    minWidth: 0,
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    paddingLeft: theme.spacing.sm,
    paddingRight: theme.spacing.sm,
  },

  contextChip: {
    display: "inline-flex",
    alignItems: "center",
    gap: 8,
    maxWidth: "100%",
    padding: "6px 12px",
    borderRadius: 999,
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[5] : theme.colors.gray[0],
    border: `1px solid ${
      theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[2]
    }`,
  },

  breadcrumbWrap: {
    minWidth: 0,
    overflow: "hidden",
  },

  breadcrumbs: {
    [`& .mantine-Breadcrumbs-separator`]: {
      margin: "0 6px",
      color: theme.colorScheme === "dark" ? theme.colors.dark[3] : theme.colors.gray[5],
      display: "flex",
      alignItems: "center",
    },
  },

  breadcrumbItem: {
    color: theme.colors.brand[6],
    fontWeight: 600,
    fontSize: theme.fontSizes.sm,
    textDecoration: "none",
    cursor: "pointer",
    whiteSpace: "nowrap",
    overflow: "hidden",
    textOverflow: "ellipsis",
    maxWidth: 180,
    padding: "2px 4px",
    borderRadius: theme.radius.sm,
    transition: "background-color 0.15s ease, color 0.15s ease",
    "&:hover": {
      color: theme.colorScheme === "dark" ? theme.colors.brand[3] : theme.colors.brand[8],
    },
  },

  breadcrumbCurrent: {
    color: theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.colors.gray[8],
    fontWeight: 600,
    fontSize: theme.fontSizes.sm,
    whiteSpace: "nowrap",
    overflow: "hidden",
    textOverflow: "ellipsis",
    maxWidth: 200,
    padding: "2px 4px",
    cursor: "pointer",
  },

  headerMenu: {
    height: "100%",
    paddingRight: theme.spacing.xs,
    paddingLeft: theme.spacing.xs,
    flexShrink: 0,
    [`@media (min-width: ${theme.breakpoints.sm}px)`]: {
      paddingRight: theme.spacing.md,
      paddingLeft: theme.spacing.md,
    },
  },

  userBlock: {
    gap: theme.spacing.xs,
    padding: "4px 8px",
    borderRadius: theme.radius.md,
    transition: "background-color 0.15s ease",
    "&:hover": {
      backgroundColor:
        theme.colorScheme === "dark"
          ? theme.colors.dark[6]
          : theme.colors.gray[0],
    },
    [`@media (min-width: ${theme.breakpoints.md}px)`]: {
      gap: theme.spacing.sm,
    },
  },

  userInfo: {
    display: "none",
    [`@media (min-width: ${theme.breakpoints.md}px)`]: {
      display: "flex",
    },
  },
}));

export function AppHeader({ franchiseName }) {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [organizationName, setOrganizationName] = useState(null);
  const { classes, theme } = useStyles();
  const naviagte = useNavigate();
  const { colorScheme, toggleColorScheme } = useTheme();
  const { isCollapsed, toggleSidebar } = useSidebar();

  const userInfo = localStoreService.getUserInfo();
  const userType = localStoreService.getUserType();


  useEffect(() => {
    fetchOrganizationName();
  }, []);

  const fetchOrganizationName = async () => {
    try {
      const organizationId = localStoreService.getOrganizationID();
      if (organizationId) {
        const response = await organizationService.getOrganizationById(organizationId);
        if (response && response.name) {
          setOrganizationName(response.name);
        }
      }
    } catch (error) {
      console.error('Error fetching organization name:', error);
    }
  };

  const handleLogout = async () => {
    try {
      // Log logout history before clearing storage
      const userInfo = localStoreService.getUserInfo();
      if (userInfo?.UserID) {
        await loginHistoryService.updateLogoutTime(userInfo.UserID);
      }
    } catch (error) {
      console.error('Failed to log logout history:', error);
      // Don't block logout if logging fails
    }
    
    localStoreService.clearLocalStorage();
    naviagte("/login");
  };

  const handleChangePassword = () => {
    setIsModalOpen(true);
  };

  const closeChangePasswordModal = () => {
    setIsModalOpen(false);
  };


  const showBreadcrumb = !!organizationName && userType == UserType.Staffs;
  const breadcrumbItems = [];
  if (organizationName) {
    breadcrumbItems.push({
      label: organizationName,
      onClick: () => naviagte("/organizations"),
    });
  }
  if (franchiseName) {
    breadcrumbItems.push({
      label: franchiseName,
      onClick: () => naviagte(`/franchises/${franchiseName}/dashboard`),
    });
  }

  return (
    <Header height={HEADER_HEIGHT} zIndex={200} className={classes.header}>
      <Group position="apart" className={classes.headerInner} noWrap>
        <Group className={classes.leftSection} spacing="xs" noWrap>
          <Burger opened={!isCollapsed} onClick={toggleSidebar} size="sm" />
          <Box style={{ flexShrink: 1, minWidth: 0, maxWidth: 160 }}>
            <Logo width={180} style={{ maxWidth: "100%", height: "auto" }} />
          </Box>
        </Group>

        {showBreadcrumb && breadcrumbItems.length > 0 ? (
          <Box className={classes.contextSection}>
            <Box className={classes.contextChip}>
              <IconBuilding size={16} stroke={1.75} color={theme.colors.brand[6]} />
              <Breadcrumbs
                className={classes.breadcrumbs}
                separator={<IconChevronRight size={14} stroke={2} />}
              >
                {breadcrumbItems.map((item, i) => {
                  const isLast = i === breadcrumbItems.length - 1;
                  return (
                    <Text
                      key={i}
                      className={isLast ? classes.breadcrumbCurrent : classes.breadcrumbItem}
                      component="span"
                      role="button"
                      tabIndex={0}
                      onClick={item.onClick}
                      onKeyDown={(e) => e.key === "Enter" && item.onClick()}
                      title={item.label}
                    >
                      {item.label}
                    </Text>
                  );
                })}
              </Breadcrumbs>
            </Box>
          </Box>
        ) : (
          <Box className={classes.contextSection} />
        )}

        <Group className={classes.headerMenu} spacing="xs">
          <NotificationBell theme={theme} />
          <Tooltip label={colorScheme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'} position="bottom">
            <ActionIcon
              variant="subtle"
              color={colorScheme === 'dark' ? 'yellow' : 'brand'}
              onClick={toggleColorScheme}
              size="md"
            >
              {colorScheme === 'dark' ? (
                <IconSun size="1rem" />
              ) : (
                <IconMoon size="1rem" />
              )}
            </ActionIcon>
          </Tooltip>
          <Menu withArrow width={220} position="bottom-end">
            <Menu.Target>
              <UnstyledButton className={classes.userBlock}>
                <Group spacing="xs">
                  <Avatar
                    src={buildImageUrl(userInfo?.ProfileImagePath)}
                    radius="xl"
                    size="md"
                  />
                  <MediaQuery smallerThan="md" styles={{ display: "none" }}>
                    <Stack spacing={0} className={classes.userInfo}>
                      <Text size="sm" weight={500} lineClamp={1}>
                        {userInfo?.FullName || "-"}
                      </Text>
                      <Text color="dimmed" size="xs" lineClamp={1}>
                        {userInfo?.Email || "-"}
                      </Text>
                    </Stack>
                  </MediaQuery>
                  <IconChevronRight size={16} style={{ flexShrink: 0 }} />
                </Group>
              </UnstyledButton>
            </Menu.Target>
            <Menu.Dropdown mt="0.4rem">
              <Menu.Item
                onClick={handleChangePassword}
                icon={
                  <IconLock
                    size="0.9rem"
                    stroke={1.5}
                    color={theme.colors.brand[6]}
                  />
                }
              >
                Change Password
              </Menu.Item>
              <Menu.Item
                onClick={handleLogout}
                icon={
                  <IconLogout
                    size="0.9rem"
                    stroke={1.5}
                    color={theme.colors.red[6]}
                  />
                }
              >
                Logout
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        </Group>
      </Group>


      <AppModal
        opened={isModalOpen}
        onClose={closeChangePasswordModal}
        title="Change Password"
      >
        <ChangePassword
          userId={userInfo?.UserID}
          onSuccess={closeChangePasswordModal}
        />
      </AppModal>

    </Header>
  );
}

export function Logo({ width = 180, ...props }) {
  return (
    <img
      src="/caresyncHC-Logo.PNG"
      alt="caresynX Scheduler"
      width={width}
      height="auto"
      style={{ maxHeight: "40px", objectFit: "contain" }}
      {...props}
    />
  );
}
