import { Avatar, Group, Paper, Text, createStyles } from "@mantine/core";

const useStyles = createStyles((theme) => ({
  franchiseCard: {
    transition: "transform 160ms ease, box-shadow 160ms ease",
    height: "12.5rem",
    cursor: "pointer",
    borderColor:
      theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[2],

    "&:hover": {
      transform: "translateY(-3px)",
      boxShadow: theme.shadows.md,
      borderColor: theme.colors.brand[3],
    },
  },
}));

const FranchiseCard = ({ franchise, onFranchise }) => {
  const { name, description } = franchise;
  const { classes } = useStyles();
  const initials = (name || "?")
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((w) => w[0])
    .join("")
    .toUpperCase();

  return (
    <Paper
      shadow="sm"
      p="md"
      radius="md"
      withBorder
      className={classes.franchiseCard}
      onClick={() => onFranchise(franchise)}
    >
      <Group mb="md" noWrap>
        <Avatar size="lg" radius="md" color="brand">
          {initials || "?"}
        </Avatar>
        <Text size="lg" weight={600} lineClamp={1}>
          {name}
        </Text>
      </Group>

      <Text size="sm" color="dimmed" lineClamp={3}>
        {description}
      </Text>
    </Paper>
  );
};

export default FranchiseCard;
