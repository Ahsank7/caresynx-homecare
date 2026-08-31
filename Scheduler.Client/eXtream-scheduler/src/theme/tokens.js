/**
 * CareSync brand tokens — shared between the Mantine theme and
 * screens that cannot pick up theme (Chart.js, hardcoded status colors).
 * Palette matches the public landing site.
 */

export const brand = {
  indigo: "#6366f1",
  violet: "#8b5cf6",
  cyan: "#06b6d4",
  amber: "#f59e0b",
  emerald: "#10b981",
  rose: "#f43f5e",
  slate: "#64748b",
  canvas: "#f8fafc",
  canvasDark: "#1a1b1e",
};

/** Mantine 10-shade scale. Index 5 is the primary brand indigo. */
export const brandScale = [
  "#eef2ff",
  "#e0e7ff",
  "#c7d2fe",
  "#a5b4fc",
  "#818cf8",
  "#6366f1",
  "#4f46e5",
  "#4338ca",
  "#3730a3",
  "#312e81",
];

export const chartColors = [
  brand.indigo,
  brand.violet,
  brand.cyan,
  brand.emerald,
  brand.amber,
  brand.rose,
];

export const statusColors = {
  pending: { color: "violet", hex: brand.violet },
  confirmed: { color: "teal", hex: brand.emerald },
  cancelled: { color: "red", hex: brand.rose },
  completed: { color: "indigo", hex: brand.indigo },
  inProgress: { color: "cyan", hex: brand.cyan },
  default: { color: "gray", hex: brand.slate },
};

export const fontFamily =
  "Inter, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif";

/** Logged-in app chrome. Keep header, drawers, and overlays in sync. */
export const HEADER_HEIGHT = 56;
