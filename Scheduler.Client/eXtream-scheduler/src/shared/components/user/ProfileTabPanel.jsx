import { Paper, Stack, Title, Text, Group } from "@mantine/core";

/**
 * Consistent header + bordered container for profile detail tabs
 * (matches Preferences / Complaints: title, subtitle, withBorder Paper).
 */
export function ProfileTabPanel({ title, description, headerActions, children }) {
  return (
    <Paper p="md" withBorder>
      <Stack spacing="md">
        {(title || description || headerActions) && (
          <Group position="apart" align="flex-start" noWrap>
            <div style={{ minWidth: 0 }}>
              {title && <Title order={4}>{title}</Title>}
              {description && (
                <Text size="sm" color="dimmed" mt={title ? 4 : 0}>
                  {description}
                </Text>
              )}
            </div>
            {headerActions ? (
              <div style={{ flexShrink: 0 }}>{headerActions}</div>
            ) : null}
          </Group>
        )}
        {children}
      </Stack>
    </Paper>
  );
}
