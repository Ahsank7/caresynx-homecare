import {
  Card,
  Group,
  Stack,
  Badge,
  Grid,
  Text,
  Button,
  Switch,
  Tooltip,
  ActionIcon,
  Divider,
  createStyles,
  useMantineTheme,
} from "@mantine/core";
import {
  IconBuilding,
  IconBuildingStore,
  IconPlus,
  IconCheck,
  IconX,
  IconEdit,
} from "@tabler/icons";
import { stringhelper } from "shared/utils";

const franchiseAdminTooltip = (franchiseName) => {
  const { username, password } =
    stringhelper.getFranchiseDefaultAdminCredentials(franchiseName);
  return (
    <Stack spacing={4}>
      <Text size="xs" weight={600}>
        Default franchise admin (at creation)
      </Text>
      <Text size="xs">Username: {username || "—"}</Text>
      <Text size="xs">Password: {password || "—"}</Text>
      <Text size="xs" color="dimmed" mt={4}>
        Matches server defaults. Wrong if this admin&apos;s password was changed.
      </Text>
    </Stack>
  );
};

const useStyles = createStyles((theme) => ({
  orgCard: {
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[7] : theme.white,
    border: `1px solid ${
      theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[3]
    }`,
    borderRadius: theme.radius.md,
    boxShadow: theme.shadows.sm,
    transition: "all 0.2s ease",
    "&:hover": {
      boxShadow: theme.shadows.md,
      transform: "translateY(-2px)",
    },
  },
  orgCardDisabled: {
    opacity: 0.6,
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[8] : theme.colors.gray[1],
  },
  statusBadge: {
    textTransform: "capitalize",
  },
  infoRow: {
    display: "flex",
    alignItems: "center",
    gap: theme.spacing.xs,
    marginBottom: theme.spacing.xs,
  },
  infoLabel: {
    fontWeight: 500,
    color:
      theme.colorScheme === "dark"
        ? theme.colors.dark[2]
        : theme.colors.gray[7],
    fontSize: theme.fontSizes.sm,
    minWidth: 100,
  },
  infoValue: {
    fontWeight: 600,
    color:
      theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.colors.gray[9],
    fontSize: theme.fontSizes.sm,
  },
  emptyState: {
    textAlign: "center",
    padding: theme.spacing.xl * 2,
  },
}));

const AdminOrganizationList = ({
  organizations,
  onAddFranchise,
  onToggleOrganization,
  onOrganizationClick,
  onToggleFranchise,
}) => {
  const { classes } = useStyles();
  const theme = useMantineTheme();

  if (organizations.length === 0) {
    return (
      <Card className={classes.orgCard} p="xl" mt="lg">
        <Stack align="center" spacing="md">
          <IconBuilding size={64} color={theme.colors.gray[4]} />
          <Text size="lg" weight={500} color="dimmed">
            No organizations found
          </Text>
          <Text color="dimmed" size="sm">
            Click "Add New Organization" to create your first organization.
          </Text>
        </Stack>
      </Card>
    );
  }

  return (
    <Grid gutter="md" mt="lg">
      {organizations.map((org) => (
        <Grid.Col key={org.id} span={12} md={6} lg={4}>
          <Card
            className={`${classes.orgCard} ${
              !org.isActive ? classes.orgCardDisabled : ""
            }`}
            p="md"
            style={{ cursor: "pointer" }}
            onClick={() => onOrganizationClick && onOrganizationClick(org)}
          >
            <Stack spacing="sm">
              {/* Organization Header */}
              <Group position="apart" align="flex-start">
                <div>
                  <Text weight={600} size="lg">
                    {org.name}
                  </Text>
                  <Text size="sm" color="dimmed">
                    ID: {org.id}
                  </Text>
                </div>
                <Badge
                  color={org.isActive ? "green" : "red"}
                  variant="filled"
                  className={classes.statusBadge}
                  leftSection={
                    org.isActive ? (
                      <IconCheck size={14} />
                    ) : (
                      <IconX size={14} />
                    )
                  }
                >
                  {org.isActive ? "Active" : "Disabled"}
                </Badge>
              </Group>

              <Divider />

              {/* Organization Info */}
              {org.description && (
                <div className={classes.infoRow}>
                  <Text size="sm" color="dimmed">
                    {org.description}
                  </Text>
                </div>
              )}

              {org.email && (
                <div className={classes.infoRow}>
                  <Text size="sm" className={classes.infoLabel}>
                    Email:
                  </Text>
                  <Text size="sm" className={classes.infoValue}>
                    {org.email}
                  </Text>
                </div>
              )}

              {org.contactNo && (
                <div className={classes.infoRow}>
                  <Text size="sm" className={classes.infoLabel}>
                    Contact:
                  </Text>
                  <Text size="sm" className={classes.infoValue}>
                    {org.contactNo}
                  </Text>
                </div>
              )}

              <Divider />

              {/* Franchises List */}
              <Text size="sm" weight={500} mt="xs">
                Franchises ({org.franchises?.length ?? 0}):
              </Text>
              {org.franchises && org.franchises.length > 0 ? (
                <Stack spacing="xs">
                  {org.franchises.map((franchise) => (
                    <Tooltip
                      key={franchise.id}
                      label={franchiseAdminTooltip(franchise.name)}
                      multiline
                      width={300}
                      position="top-start"
                      withArrow
                      withinPortal
                    >
                      <Group
                        position="apart"
                        p="xs"
                        style={{
                          cursor: "help",
                          backgroundColor:
                            theme.colorScheme === "dark"
                              ? theme.colors.dark[6]
                              : theme.colors.gray[0],
                          borderRadius: theme.radius.sm,
                        }}
                        onClick={(e) => {
                          e.stopPropagation();
                        }}
                      >
                        <Group spacing="xs" noWrap>
                          <IconBuildingStore size={16} />
                          <Text size="sm">{franchise.name}</Text>
                          <Badge
                            size="xs"
                            color={franchise.isActive ? "green" : "red"}
                            variant="light"
                          >
                            {franchise.isActive ? "Active" : "Disabled"}
                          </Badge>
                        </Group>
                        <Switch
                          checked={franchise.isActive}
                          onChange={() =>
                            onToggleFranchise &&
                            onToggleFranchise(franchise, org)
                          }
                          color={franchise.isActive ? "red" : "green"}
                          size="sm"
                          onClick={(e) => e.stopPropagation()}
                        />
                      </Group>
                    </Tooltip>
                  ))}
                </Stack>
              ) : (
                <Text size="sm" color="dimmed" py="xs">
                  No franchises yet. Use Add Franchise below.
                </Text>
              )}

              <Divider />

              {/* Actions */}
              <Group position="apart" mt="xs">
                <Group spacing="xs">
                  <Tooltip label={org.isActive ? "Disable" : "Enable"}>
                    <Switch
                      checked={org.isActive}
                      onChange={(e) => {
                        e.stopPropagation();
                        onToggleOrganization(org);
                      }}
                      color={org.isActive ? "red" : "green"}
                      size="md"
                    />
                  </Tooltip>
                  <Text size="xs" color="dimmed">
                    {org.isActive ? "Enabled" : "Disabled"}
                  </Text>
                </Group>
                <Button
                  size="sm"
                  variant="light"
                  color="brand"
                  leftIcon={<IconPlus size={16} />}
                  onClick={(e) => {
                    e.stopPropagation();
                    onAddFranchise(org);
                  }}
                >
                  Add Franchise
                </Button>
              </Group>
            </Stack>
          </Card>
        </Grid.Col>
      ))}
    </Grid>
  );
};

export default AdminOrganizationList;

