import { createStyles } from "@mantine/core";

const useStyles = createStyles((theme) => ({
  tableHeader: {
    position: "sticky",
    top: 0,
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[6] : theme.colors.gray[0],
    transition: "box-shadow 150ms ease",
    zIndex: 2,

    "&::after": {
      content: '""',
      position: "absolute",
      left: 0,
      right: 0,
      bottom: 0,
      borderBottom: `1px solid ${theme.colorScheme === "dark"
          ? theme.colors.dark[4]
          : theme.colors.gray[2]
        }`,
    },
  },

  scrolled: {
    boxShadow: theme.shadows.sm,
  },

  headerHeaderRow: {
    backgroundColor: theme.colorScheme === "dark" ? theme.colors.dark[6] : theme.colors.gray[0],
  },

  headerHeaderColumn: {
    paddingTop: "0.85rem !important",
    paddingBottom: "0.85rem !important",
    whiteSpace: "nowrap",
    fontWeight: 600,
    fontSize: theme.fontSizes.xs,
    textTransform: "uppercase",
    letterSpacing: "0.04em",
    color: theme.colorScheme === "dark" ? theme.colors.dark[1] : theme.colors.gray[6],
  },
}));

export const AppTableHeader = ({ thead, scrolled }) => {
  const { classes, cx } = useStyles();

  return (
    <thead
      className={cx(classes.tableHeader, {
        [classes.scrolled]: scrolled,
      })}
    >
      <tr className={classes.headerHeaderRow}>
        {thead.map((head, index) => (
          <th className={classes.headerHeaderColumn} key={index}>
            {head}
          </th>
        ))}
      </tr>
    </thead>
  );
};