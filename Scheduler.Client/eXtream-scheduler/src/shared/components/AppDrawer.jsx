import {
  Button,
  Drawer,
  Group,
  Text,
  Stack,
  ScrollArea,
  Box,
  Paper,
  ThemeIcon,
} from "@mantine/core";
import { IconFilter } from "@tabler/icons-react";

/**
 * Groups filter fields with a subtle card and section label.
 */
export function FilterSection({ title, children, ...paperProps }) {
  return (
    <Paper
      p="md"
      radius="md"
      withBorder
      shadow="xs"
      sx={(theme) => ({
        backgroundColor:
          theme.colorScheme === "dark"
            ? theme.colors.dark[6]
            : theme.colors.gray[0],
      })}
      {...paperProps}
    >
      {title ? (
        <Text
          size="xs"
          weight={700}
          color="dimmed"
          tt="uppercase"
          letterSpacing="0.06em"
          mb="sm"
        >
          {title}
        </Text>
      ) : null}
      <Stack spacing="sm">{children}</Stack>
    </Paper>
  );
}

export const AppDrawer = ({
  opened,
  close,
  onFilter,
  children,
  onReset,
  title = "Filters",
  description = "Refine the list with the options below.",
}) => {
  return (
    <Drawer
      opened={opened}
      position="right"
      size={420}
      onClose={close}
      padding="lg"
      shadow="xl"
      overlayProps={{ opacity: 0.5, blur: 5 }}
      transitionProps={{
        duration: 220,
        timingFunction: "cubic-bezier(0.4, 0, 0.2, 1)",
      }}
      styles={{
        header: {
          marginBottom: 4,
        },
        body: {
          paddingTop: 8,
          display: "flex",
          flexDirection: "column",
          flex: 1,
          minHeight: 0,
          height: "100%",
        },
        content: {
          display: "flex",
          flexDirection: "column",
          height: "100%",
        },
      }}
      title={
        <Group spacing="sm" noWrap align="flex-start">
          <ThemeIcon size="lg" radius="md" variant="light" color="blue">
            <IconFilter size={18} stroke={1.75} />
          </ThemeIcon>
          <Box>
            <Text weight={700} size="lg" lh={1.25}>
              {title}
            </Text>
            {description ? (
              <Text size="xs" color="dimmed" mt={6} maw={320}>
                {description}
              </Text>
            ) : null}
          </Box>
        </Group>
      }
    >
      <ScrollArea
        type="hover"
        offsetScrollbars
        scrollbarSize={8}
        style={{ flex: 1, minHeight: 0 }}
        styles={{
          viewport: { paddingRight: 4 },
          root: { height: "100%" },
        }}
      >
        <Stack spacing="lg" pb="md">
          {children}
        </Stack>
      </ScrollArea>
      <Box
        pt="lg"
        mt="md"
        sx={(theme) => ({
          borderTop: `1px solid ${
            theme.colorScheme === "dark"
              ? theme.colors.dark[4]
              : theme.colors.gray[3]
          }`,
          backgroundColor:
            theme.colorScheme === "dark" ? theme.colors.dark[7] : theme.white,
        })}
      >
        <Button.Group>
          <Button fullWidth size="sm" onClick={onFilter}>
            Apply
          </Button>
          {onReset && (
            <Button variant="default" fullWidth size="sm" onClick={onReset}>
              Reset
            </Button>
          )}
        </Button.Group>
      </Box>
    </Drawer>
  );
};
