import React from "react";
import { Text, Tooltip } from "@mantine/core";

export function TruncatedTooltipText({
  value,
  maxWidth = 220,
  tooltipWidth = 320,
  color,
  size,
  weight,
}) {
  const displayValue = value || "N/A";

  return (
    <Tooltip
      label={displayValue}
      multiline
      width={tooltipWidth}
      withArrow
      position="top-start"
    >
      <Text
        color={color}
        size={size}
        weight={weight}
        style={{
          maxWidth,
          whiteSpace: "nowrap",
          overflow: "hidden",
          textOverflow: "ellipsis",
          lineHeight: 1.4,
          cursor: "pointer",
        }}
      >
        {displayValue}
      </Text>
    </Tooltip>
  );
}
