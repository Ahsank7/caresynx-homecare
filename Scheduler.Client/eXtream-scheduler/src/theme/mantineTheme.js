import { brandScale, fontFamily, HEADER_HEIGHT } from "./tokens";

const inputStyles = (theme) => ({
  label: {
    fontWeight: 500,
    marginBottom: 4,
    color: theme.colorScheme === "dark" ? theme.colors.dark[1] : theme.colors.gray[7],
  },
  input: {
    borderColor:
      theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[3],
  },
});

/**
 * Mantine v6 theme. Pass colorScheme from ThemeContext at the provider.
 */
export const createAppTheme = (colorScheme = "light") => ({
  colorScheme,
  primaryColor: "brand",
  fontFamily,
  headings: {
    fontFamily,
    fontWeight: 600,
  },
  defaultRadius: "md",
  colors: {
    brand: brandScale,
  },
  shadows: {
    xs: "0 1px 2px rgba(15, 23, 42, 0.04)",
    sm: "0 1px 3px rgba(15, 23, 42, 0.06), 0 1px 2px rgba(15, 23, 42, 0.04)",
    md: "0 4px 12px rgba(15, 23, 42, 0.08)",
    lg: "0 10px 24px rgba(15, 23, 42, 0.10)",
    xl: "0 20px 40px rgba(15, 23, 42, 0.12)",
  },
  components: {
    Button: {
      defaultProps: {
        radius: "md",
      },
      styles: (theme) => ({
        root: {
          fontWeight: 600,
          letterSpacing: "-0.01em",
        },
      }),
    },
    TextInput: {
      defaultProps: { radius: "md" },
      styles: inputStyles,
    },
    PasswordInput: {
      defaultProps: { radius: "md" },
      styles: inputStyles,
    },
    NumberInput: {
      defaultProps: { radius: "md" },
      styles: inputStyles,
    },
    Textarea: {
      defaultProps: { radius: "md" },
      styles: inputStyles,
    },
    Select: {
      defaultProps: { radius: "md" },
      styles: inputStyles,
    },
    MultiSelect: {
      defaultProps: { radius: "md" },
      styles: inputStyles,
    },
    DatePicker: {
      defaultProps: { radius: "md" },
    },
    Card: {
      defaultProps: {
        radius: "md",
        shadow: "sm",
        withBorder: true,
      },
      styles: (theme) => ({
        root: {
          borderColor:
            theme.colorScheme === "dark"
              ? theme.colors.dark[4]
              : theme.colors.gray[2],
        },
      }),
    },
    Paper: {
      defaultProps: {
        radius: "md",
      },
    },
    Modal: {
      defaultProps: {
        radius: "md",
        overlayBlur: 4,
        overlayOpacity: 0.45,
        centered: true,
        zIndex: 400,
      },
      styles: (theme) => ({
        title: {
          fontWeight: 600,
          fontSize: theme.fontSizes.lg,
        },
        header: {
          paddingBottom: theme.spacing.sm,
        },
        inner: {
          paddingTop: HEADER_HEIGHT + 16,
          paddingBottom: 24,
        },
        overlay: {
          top: HEADER_HEIGHT,
        },
      }),
    },
    Drawer: {
      defaultProps: {
        overlayBlur: 4,
        overlayOpacity: 0.45,
        zIndex: 400,
      },
      styles: {
        inner: {
          top: HEADER_HEIGHT,
          bottom: 0,
          height: "auto",
          padding: 0,
        },
        overlay: {
          top: HEADER_HEIGHT,
        },
        content: {
          borderRadius: 0,
          height: "100%",
        },
      },
    },
    Tabs: {
      defaultProps: {
        variant: "default",
      },
      styles: (theme) => ({
        tab: {
          fontWeight: 500,
          "&[data-active]": {
            fontWeight: 600,
          },
        },
      }),
    },
    Badge: {
      defaultProps: {
        radius: "sm",
        size: "sm",
      },
    },
    Tooltip: {
      defaultProps: {
        withArrow: true,
        radius: "sm",
        withinPortal: true,
      },
    },
    ActionIcon: {
      defaultProps: {
        radius: "md",
      },
    },
    Checkbox: {
      defaultProps: {
        radius: "sm",
      },
    },
    Notification: {
      defaultProps: {
        radius: "md",
      },
    },
    Pagination: {
      defaultProps: {
        radius: "md",
        size: "sm",
      },
    },
    Table: {
      styles: (theme) => ({
        root: {
          "& thead th": {
            fontWeight: 600,
            fontSize: theme.fontSizes.xs,
            textTransform: "uppercase",
            letterSpacing: "0.04em",
            color:
              theme.colorScheme === "dark"
                ? theme.colors.dark[1]
                : theme.colors.gray[6],
            backgroundColor:
              theme.colorScheme === "dark"
                ? theme.colors.dark[6]
                : theme.colors.gray[0],
          },
          "& tbody tr:hover": {
            backgroundColor:
              theme.colorScheme === "dark"
                ? theme.colors.dark[5]
                : theme.colors.gray[0],
          },
        },
      }),
    },
    Navbar: {
      styles: (theme) => ({
        root: {
          backgroundColor:
            theme.colorScheme === "dark" ? theme.colors.dark[7] : theme.white,
          borderRight: `1px solid ${
            theme.colorScheme === "dark"
              ? theme.colors.dark[4]
              : theme.colors.gray[2]
          }`,
        },
      }),
    },
    Header: {
      styles: (theme) => ({
        root: {
          overflow: "visible",
          backgroundColor:
            theme.colorScheme === "dark" ? theme.colors.dark[7] : theme.white,
          borderBottom: `1px solid ${
            theme.colorScheme === "dark"
              ? theme.colors.dark[4]
              : theme.colors.gray[2]
          }`,
          boxShadow:
            theme.colorScheme === "dark"
              ? "none"
              : "0 1px 0 rgba(15, 23, 42, 0.04)",
        },
      }),
    },
  },
});
