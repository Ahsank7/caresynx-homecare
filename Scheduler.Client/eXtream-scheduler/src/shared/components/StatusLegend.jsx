import { Badge, Group, Text } from "@mantine/core";

export const STATUS_LEGEND_ITEMS = [
  { color: "violet", label: "Scheduled" },
  { color: "teal", label: "In Progress" },
  { color: "yellow", label: "Cancelled" },
  { color: "brand", label: "Completed" },
  { color: "red", label: "Delayed" },
  { color: "gray", label: "Unassigned" },
];

export function StatusLegend({
  showLabel = true,
  position,
  ...groupProps
}) {
  return (
    <Group
      className="app-status-legend"
      spacing="sm"
      position={position}
      sx={{ flexWrap: "wrap", rowGap: 8 }}
      {...groupProps}
    >
      {showLabel ? (
        <Text className="app-status-legend-label" weight={700} color="dimmed" mr={4}>
          Status
        </Text>
      ) : null}
      {STATUS_LEGEND_ITEMS.map((item) => (
        <Badge
          key={item.label}
          className="app-status-legend-badge"
          color={item.color}
          variant="light"
          size="lg"
          radius="md"
        >
          {item.label}
        </Badge>
      ))}
    </Group>
  );
}
