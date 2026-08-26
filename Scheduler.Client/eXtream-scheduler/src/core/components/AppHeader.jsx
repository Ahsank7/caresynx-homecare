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
import { IconChevronRight, IconLogout, IconSun, IconMoon, IconLock } from "@tabler/icons";
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

const useStyles = createStyles((theme) => ({
  header: {
    paddingLeft: theme.spacing.xs,
    paddingRight: theme.spacing.xs,
    borderBottom: `1px solid ${theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[3]}`,
    minHeight: "56px",
    height: "8vh",
    [`@media (min-width: ${theme.breakpoints.sm}px)`]: {
      paddingLeft: theme.spacing.md,
      paddingRight: theme.spacing.md,
    },
  },

  headerInner: {
    height: "100%",
    width: "100%",
    flexWrap: "nowrap",
    overflow: "hidden",
  },

  leftSection: {
    flex: "1 1 0",
    minWidth: 0,
    gap: theme.spacing.xs,
    [`@media (min-width: ${theme.breakpoints.sm}px)`]: {
      gap: theme.spacing.sm,
    },
  },

  breadcrumbWrap: {
    minWidth: 0,
    overflow: "hidden",
  },

  breadcrumbs: {
    [`& .mantine-Breadcrumbs-separator`]: {
      margin: "0 4px",
      color: theme.colorScheme === "dark" ? theme.colors.dark[3] : theme.colors.gray[5],
      display: "flex",
      alignItems: "center",
    },
  },

  breadcrumbItem: {
    color: theme.colors.blue[6],
    fontWeight: 600,
    fontSize: theme.fontSizes.xs,
    textDecoration: "none",
    cursor: "pointer",
    whiteSpace: "nowrap",
    overflow: "hidden",
    textOverflow: "ellipsis",
    maxWidth: 140,
    padding: "4px 8px",
    borderRadius: theme.radius.sm,
    transition: "background-color 0.15s ease, color 0.15s ease",
    "&:hover": {
      backgroundColor: theme.colorScheme === "dark" ? theme.colors.dark[5] : theme.colors.blue[0],
      color: theme.colorScheme === "dark" ? theme.colors.blue[3] : theme.colors.blue[7],
    },
    [`@media (min-width: ${theme.breakpoints.sm}px)`]: {
      fontSize: theme.fontSizes.sm,
      maxWidth: 200,
    },
  },

  breadcrumbCurrent: {
    color: theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.colors.gray[8],
    fontWeight: 600,
    fontSize: theme.fontSizes.xs,
    whiteSpace: "nowrap",
    overflow: "hidden",
    textOverflow: "ellipsis",
    maxWidth: 140,
    padding: "4px 8px",
    borderRadius: theme.radius.sm,
    backgroundColor: theme.colorScheme === "dark" ? theme.colors.dark[5] : theme.colors.gray[1],
    cursor: "pointer",
    "&:hover": {
      backgroundColor: theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[2],
    },
    [`@media (min-width: ${theme.breakpoints.sm}px)`]: {
      fontSize: theme.fontSizes.sm,
      maxWidth: 200,
    },
  },

  headerMenu: {
    height: "100%",
    paddingRight: theme.spacing.xs,
    paddingLeft: theme.spacing.xs,
    flexShrink: 0,
    "&:hover": {
      backgroundColor:
        theme.colorScheme === "dark"
          ? theme.colors.dark[8]
          : theme.colors.gray[0],
    },
    [`@media (min-width: ${theme.breakpoints.sm}px)`]: {
      paddingRight: theme.spacing.md,
      paddingLeft: theme.spacing.md,
    },
  },

  userBlock: {
    gap: theme.spacing.xs,
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


  const showBreadcrumb = organizationName && franchiseName && userType == UserType.Staffs;
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
    <Header shadow="md" className={classes.header}>
      <Group position="apart" className={classes.headerInner}>
        <Group className={classes.leftSection} spacing="xs" noWrap>
          <Burger opened={!isCollapsed} onClick={toggleSidebar} size="sm" />
          <Box style={{ flexShrink: 1, minWidth: 0, maxWidth: 160 }}>
            <Logo width={180} style={{ maxWidth: "100%", height: "auto" }} />
          </Box>
          {showBreadcrumb && breadcrumbItems.length > 0 && (
            <Box className={classes.breadcrumbWrap}>
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
          )}
        </Group>

        <Group className={classes.headerMenu} spacing="xs">
          <NotificationBell theme={theme} />
          <Tooltip label={colorScheme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}>
            <ActionIcon
              variant="outline"
              color={colorScheme === 'dark' ? 'yellow' : 'blue'}
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
                    color={theme.colors.blue[6]}
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
