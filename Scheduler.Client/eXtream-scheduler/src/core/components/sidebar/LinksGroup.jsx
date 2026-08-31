import { useState } from "react";
import {
  Group,
  Box,
  Collapse,
  ThemeIcon,
  Text,
  UnstyledButton,
  createStyles,
  Tooltip,
} from "@mantine/core";
import { IconChevronLeft, IconChevronRight } from "@tabler/icons";

const useStyles = createStyles((theme) => {
  const brand = theme.colors.brand;
  const isDark = theme.colorScheme === "dark";

  return {
    control: {
      fontWeight: 500,
      display: "block",
      width: "100%",
      padding: `8px 10px`,
      marginBottom: 2,
      borderRadius: theme.radius.md,
      color: isDark ? theme.colors.dark[0] : theme.colors.gray[7],
      fontSize: theme.fontSizes.sm,
      transition: "background-color 0.15s ease, color 0.15s ease",

      "&:hover": {
        backgroundColor: isDark ? theme.colors.dark[6] : theme.colors.gray[0],
        color: isDark ? theme.white : theme.black,
      },
    },

    link: {
      fontWeight: 500,
      display: "block",
      textDecoration: "none",
      padding: `8px 10px 8px 14px`,
      marginLeft: 28,
      marginBottom: 2,
      borderRadius: theme.radius.sm,
      fontSize: theme.fontSizes.sm,
      cursor: "pointer",
      color: isDark ? theme.colors.dark[0] : theme.colors.gray[6],
      borderLeft: `2px solid ${isDark ? theme.colors.dark[4] : theme.colors.gray[2]}`,

      "&:hover": {
        backgroundColor: isDark ? theme.colors.dark[6] : theme.colors.gray[0],
        color: isDark ? theme.white : theme.black,
      },
    },

    chevron: {
      transition: "transform 200ms ease",
      color: isDark ? theme.colors.dark[2] : theme.colors.gray[5],
    },

    active: {
      backgroundColor: isDark ? `${brand[9]}55` : brand[0],
      color: isDark ? brand[3] : brand[7],
      fontWeight: 600,
      "&:hover": {
        backgroundColor: isDark ? `${brand[9]}70` : brand[1],
        color: isDark ? brand[2] : brand[8],
      },
    },

    activeCollapsed: {
      backgroundColor: isDark ? `${brand[9]}55` : brand[0],
    },
  };
});

export const LinksGroup = ({ menu, selectedMenu, onSidebarMenu, isCollapsed = false }) => {
  const { icon: Icon, label, initiallyOpened = false, childrenLinks } = menu;

  const { classes, theme, cx } = useStyles();
  const ChevronIcon = theme.dir === "ltr" ? IconChevronRight : IconChevronLeft;

  const hasLinks = Array.isArray(childrenLinks);
  const [opened, setOpened] = useState(initiallyOpened || false);

  const isActive = menu?.id === selectedMenu?.id;
  const buttonContent = (
    <UnstyledButton
      onClick={() => (hasLinks ? setOpened((o) => !o) : onSidebarMenu(menu))}
      className={cx(
        classes.control,
        isActive && !isCollapsed && classes.active,
        isActive && isCollapsed && classes.activeCollapsed
      )}
      style={{
        padding: isCollapsed ? `8px 6px` : undefined,
        justifyContent: isCollapsed ? "center" : "flex-start",
      }}
    >
      <Group position="apart" spacing={0} style={{ width: "100%" }}>
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: isCollapsed ? "center" : "flex-start" }}>
          <ThemeIcon
            variant="light"
            size={30}
            radius="md"
            color={isActive ? "brand" : "gray"}
          >
            <Icon size={18} />
          </ThemeIcon>
          {!isCollapsed && <Box ml="sm">{label}</Box>}
        </Box>
        {hasLinks && !isCollapsed && (
          <ChevronIcon
            className={classes.chevron}
            size={14}
            stroke={1.5}
            style={{
              transform: opened
                ? `rotate(${theme.dir === "rtl" ? -90 : 90}deg)`
                : "none",
            }}
          />
        )}
      </Group>
    </UnstyledButton>
  );

  return (
    <>
      {isCollapsed ? (
        <Tooltip label={label} position="right" withArrow>
          {buttonContent}
        </Tooltip>
      ) : (
        buttonContent
      )}

      {hasLinks && !isCollapsed && (
        <Collapse in={opened}>
          {childrenLinks.map((link) => (
            <Text
              component="a"
              className={cx(
                classes.link,
                link.id === selectedMenu?.id ? classes.active : ""
              )}
              key={link.label}
              onClick={() => onSidebarMenu(link)}
            >
              {link.label}
            </Text>
          ))}
        </Collapse>
      )}
    </>
  );
};
