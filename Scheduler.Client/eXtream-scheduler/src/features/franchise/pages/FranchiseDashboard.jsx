import React, { useState, useEffect, useCallback } from "react";
import {
  Box,
  Grid,
  Group,
  Stack,
  Text,
  ThemeIcon,
  Button,
  Paper,
  TextInput,
  LoadingOverlay,
  Alert,
  Badge,
  Divider,
  createStyles,
} from "@mantine/core";
import { AppContainer } from "shared/components";
import { 
  IconNotes, 
  IconUsers, 
  IconUserCheck, 
  IconCashBanknote, 
  IconReceipt, 
  IconRefresh,
  IconCalendar,
  IconChartBar,
  IconAlertCircle,
  IconFilter,
} from "@tabler/icons";
import { franchiseDashboardService } from "core/services/franchiseDashboardService";
import { useParams } from "react-router-dom";
import { usePermissions } from "core/context/PermissionContext";
import { useFranchise } from "core/context/FranchiseContext";
import { brand, chartColors } from "theme";

import { Line, Bar, Pie, Doughnut } from "react-chartjs-2";
import { 
  Chart, 
  CategoryScale, 
  LinearScale, 
  PointElement, 
  LineElement, 
  BarElement, 
  Title as ChartTitle, 
  Tooltip, 
  Legend,
  ArcElement
} from "chart.js";

Chart.register(
  CategoryScale, 
  LinearScale, 
  PointElement, 
  LineElement, 
  BarElement, 
  ChartTitle, 
  Tooltip, 
  Legend,
  ArcElement
);

const toDateString = (date) => {
  if (!date) return '';
  const d = new Date(date);
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

const STAT_CARDS_LIGHT_BG = {
  brand:  "linear-gradient(135deg, #eef2ff 0%, #e0e7ff 100%)",
  teal:   "linear-gradient(135deg, #e6fcf5 0%, #c3fae8 100%)",
  violet: "linear-gradient(135deg, #f3f0ff 0%, #e5dbff 100%)",
  orange: "linear-gradient(135deg, #fff4e6 0%, #ffe8cc 100%)",
  cyan:   "linear-gradient(135deg, #e3fafc 0%, #c5f6fa 100%)",
  pink:   "linear-gradient(135deg, #fff0f6 0%, #ffdeeb 100%)",
};

const STAT_CARDS = [
  { key: "totalClients",          title: "Total Clients",     subtitle: "Active Clients",  icon: IconUsers,        color: "brand" },
  { key: "totalServiceProviders",  title: "Service Providers", subtitle: "Active Providers", icon: IconUserCheck,     color: "teal" },
  { key: "totalStaff",            title: "Total Staff",       subtitle: "Active Staff",    icon: IconNotes,        color: "violet" },
  { key: "totalTasks",            title: "Total Tasks",       subtitle: "This Period",     icon: IconChartBar,     color: "orange" },
  { key: "totalBillingInvoices",   title: "Billing Invoices",  subtitle: "This Period",     icon: IconReceipt,      color: "cyan" },
  { key: "totalWages",            title: "Total Wages",       subtitle: "This Period",     icon: IconCashBanknote, color: "pink" },
];

const useStyles = createStyles((theme) => ({
  filterPaper: {
    backgroundColor: theme.colorScheme === "dark" ? theme.colors.dark[6] : theme.colors.gray[0],
    borderColor: theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[3],
  },
  statCard: {
    height: "100%",
    minHeight: 140,
    display: "flex",
    flexDirection: "column",
    justifyContent: "space-between",
    borderColor: theme.colorScheme === "dark" ? theme.colors.dark[4] : "rgba(222, 226, 230, 0.64)",
  },
  statCardDark: {
    backgroundColor: theme.colors.dark[6],
    borderLeftWidth: 3,
    borderLeftStyle: "solid",
  },
  chartPaper: {
    borderColor: theme.colorScheme === "dark" ? theme.colors.dark[4] : "rgba(222, 226, 230, 0.64)",
  },
}));

const FranchiseDashboard = () => {
  const { franchiseName } = useParams();
  const { franchiseId, loading: franchiseLoading } = useFranchise();
  const { loading: permissionsLoading, initialized } = usePermissions();
  const { classes, cx, theme } = useStyles();
  const isDark = theme.colorScheme === "dark";
  const [dashboardData, setDashboardData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [dateError, setDateError] = useState(null);

  const getDefaultStartDate = () => new Date(new Date().getFullYear(), new Date().getMonth(), 1);
  const getDefaultEndDate   = () => new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);

  const [appliedFilters, setAppliedFilters] = useState({
    startDate: getDefaultStartDate(),
    endDate:   getDefaultEndDate(),
  });

  const [draftStartDate, setDraftStartDate] = useState(toDateString(getDefaultStartDate()));
  const [draftEndDate,   setDraftEndDate]   = useState(toDateString(getDefaultEndDate()));

  const validateDates = useCallback((start, end) => {
    if (!start || !end) {
      return null;
    }
    const s = new Date(start);
    const e = new Date(end);
    if (isNaN(s.getTime()) || isNaN(e.getTime())) {
      return "Please enter valid dates.";
    }
    if (s > e) {
      return "Start date cannot be after end date.";
    }
    const diffMs = e - s;
    const diffDays = diffMs / (1000 * 60 * 60 * 24);
    if (diffDays > 365) {
      return "Date range cannot exceed one year.";
    }
    return null;
  }, []);

  const fetchDashboardData = useCallback(async (startDate, endDate) => {
    if (!franchiseId || franchiseLoading) return;

    try {
      setLoading(true);
      setError(null);
      
      const response = await franchiseDashboardService.getFranchiseDashboardData(
        franchiseId,
        startDate,
        endDate
      );

      if (response) {
        setDashboardData(response);
      } else {
        setError("Failed to fetch dashboard data");
      }
    } catch (err) {
      console.error("Error fetching dashboard data:", err);
      if (err.response?.status === 401) {
        setError("Authentication failed. Please log in again.");
      } else if (err.response?.status === 403) {
        setError("Access denied. You don't have permission to view this dashboard.");
      } else {
        setError("An error occurred while fetching dashboard data");
      }
    } finally {
      setLoading(false);
    }
  }, [franchiseId, franchiseLoading]);

  useEffect(() => {
    if (!permissionsLoading && initialized && franchiseId && !franchiseLoading) {
      fetchDashboardData(appliedFilters.startDate, appliedFilters.endDate);
    }
  }, [appliedFilters, permissionsLoading, initialized, franchiseId, franchiseLoading, fetchDashboardData]);

  const applyFilters = () => {
    const validationError = validateDates(draftStartDate, draftEndDate);
    if (validationError) {
      setDateError(validationError);
      return;
    }
    setDateError(null);
    setAppliedFilters({
      startDate: new Date(draftStartDate),
      endDate:   new Date(draftEndDate),
    });
  };

  const handleResetFilters = () => {
    const start = getDefaultStartDate();
    const end   = getDefaultEndDate();
    setDraftStartDate(toDateString(start));
    setDraftEndDate(toDateString(end));
    setDateError(null);
    setAppliedFilters({ startDate: start, endDate: end });
  };

  useEffect(() => {
    setDateError(validateDates(draftStartDate, draftEndDate));
  }, [draftStartDate, draftEndDate, validateDates]);

  // ── Chart data ──
  const popularServicesChartData = {
    labels: dashboardData?.popularServices?.map(s => s.serviceType) || [],
    datasets: [{
      label: "Service Requests",
      data: dashboardData?.popularServices?.map(s => s.count) || [],
      backgroundColor: chartColors,
      borderRadius: 6,
    }],
  };

  const taskStatusDistributionChartData = {
    labels: dashboardData?.serviceTaskStatuses?.map(s => s.taskStatus) || [],
    datasets: [{
      label: "Task Count",
      data: dashboardData?.serviceTaskStatuses?.map(s => s.count) || [],
      backgroundColor: dashboardData?.serviceTaskStatuses?.map(s => s.color) || chartColors,
      borderRadius: 6,
    }],
  };

  const taskStatusChartData = {
    labels: dashboardData?.taskStatusDistribution?.map(s => s.status) || [],
    datasets: [{
      data: dashboardData?.taskStatusDistribution?.map(s => s.count) || [],
      backgroundColor: dashboardData?.taskStatusDistribution?.map(s => s.color) || [],
      borderWidth: 2,
      borderColor: "#fff",
    }],
  };

  const billingTrendChartData = {
    labels: dashboardData?.billingTrend?.map(b => b.month) || [],
    datasets: [{
      label: "Billing Amount",
      data: dashboardData?.billingTrend?.map(b => b.amount) || [],
      fill: true,
      backgroundColor: "rgba(99, 102, 241, 0.10)",
      borderColor: brand.indigo,
      tension: 0.4,
      pointBackgroundColor: brand.indigo,
      pointRadius: 4,
    }],
  };

  const wageTrendChartData = {
    labels: dashboardData?.wageTrend?.map(w => w.month) || [],
    datasets: [{
      label: "Wage Amount",
      data: dashboardData?.wageTrend?.map(w => w.amount) || [],
      fill: true,
      backgroundColor: "rgba(16, 185, 129, 0.10)",
      borderColor: brand.emerald,
      tension: 0.4,
      pointBackgroundColor: brand.emerald,
      pointRadius: 4,
    }],
  };

  const billingSummaryChartData = {
    labels: ["Paid", "Unpaid"],
    datasets: [{
      data: [
        dashboardData?.billingSummary?.paidCount || 0,
        dashboardData?.billingSummary?.unpaidCount || 0
      ],
      backgroundColor: [brand.emerald, brand.rose],
      borderWidth: 2,
      borderColor: "#fff",
    }],
  };

  const wageSummaryChartData = {
    labels: ["Paid", "Unpaid"],
    datasets: [{
      data: [
        dashboardData?.wageSummary?.paidCount || 0,
        dashboardData?.wageSummary?.unpaidCount || 0
      ],
      backgroundColor: [brand.emerald, brand.rose],
      borderWidth: 2,
      borderColor: "#fff",
    }],
  };

  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: true, position: "top" },
    },
    scales: {
      y: { beginAtZero: true, grid: { color: "rgba(0,0,0,0.04)" } },
      x: { grid: { display: false } },
    },
  };

  const pieChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: true, position: "bottom" },
    },
  };

  if (permissionsLoading || (loading && !dashboardData)) {
    return (
      <AppContainer title="Dashboard" showDivider="true" margionTop="2rem">
        <LoadingOverlay visible={true} />
      </AppContainer>
    );
  }

  if (error && !dashboardData) {
    return (
      <AppContainer title="Dashboard" showDivider="true" margionTop="2rem">
        <Alert color="red" title="Error" mb="md">
          {error}
        </Alert>
        <Button onClick={() => fetchDashboardData(appliedFilters.startDate, appliedFilters.endDate)} leftIcon={<IconRefresh size={16} />}>
          Retry
        </Button>
      </AppContainer>
    );
  }

  return (
    <AppContainer title="Dashboard" showDivider="true" margionTop="2rem">
      {/* ── Filters ── */}
      <Paper
        p="lg"
        mb="xl"
        radius="md"
        withBorder
        className={classes.filterPaper}
      >
        <Group position="apart" mb="sm">
          <Group spacing="xs">
            <ThemeIcon variant="light" color="brand" size="md">
              <IconFilter size={16} />
            </ThemeIcon>
            <Text weight={600} size="md">Dashboard Filters</Text>
          </Group>
          <Group spacing="xs">
            <Button 
              variant="default" 
              size="sm"
              onClick={handleResetFilters}
              leftIcon={<IconCalendar size={14} />}
            >
              Reset to Current Month
            </Button>
            <Button 
              size="sm"
              onClick={applyFilters}
              leftIcon={<IconRefresh size={14} />}
              loading={loading}
              disabled={!!dateError}
            >
              Apply &amp; Refresh
            </Button>
          </Group>
        </Group>
        
        <Grid align="flex-end">
          <Grid.Col md={3} sm={6}>
            <TextInput
              label="Start Date"
              type="date"
              size="sm"
              value={draftStartDate}
              max={draftEndDate || undefined}
              onChange={(e) => setDraftStartDate(e.target.value)}
              error={dateError && draftStartDate > draftEndDate ? true : false}
            />
          </Grid.Col>
          <Grid.Col md={3} sm={6}>
            <TextInput
              label="End Date"
              type="date"
              size="sm"
              value={draftEndDate}
              min={draftStartDate || undefined}
              onChange={(e) => setDraftEndDate(e.target.value)}
              error={dateError && draftStartDate > draftEndDate ? true : false}
            />
          </Grid.Col>
          <Grid.Col md={3} sm={6}>
            {dateError ? (
              <Group spacing={6}>
                <IconAlertCircle size={14} color="#fa5252" />
                <Text size="xs" color="red">{dateError}</Text>
              </Group>
            ) : (
              <Text size="xs" color="dimmed">
                Showing data from{" "}
                <Text span weight={500}>
                  {appliedFilters.startDate?.toLocaleDateString()}
                </Text>
                {" "}to{" "}
                <Text span weight={500}>
                  {appliedFilters.endDate?.toLocaleDateString()}
                </Text>
              </Text>
            )}
          </Grid.Col>
        </Grid>
      </Paper>

      {/* ── Stat cards ── */}
      <Grid mb="xl" gutter="md">
        {STAT_CARDS.map((card) => {
          const Icon = card.icon;
          return (
            <Grid.Col key={card.key} lg={2} md={4} sm={6}>
              <Paper
                p="lg"
                radius="md"
                withBorder
                className={cx(classes.statCard, isDark && classes.statCardDark)}
                style={
                  isDark
                    ? { borderLeftColor: theme.colors[card.color]?.[5] ?? theme.colors.brand[5] }
                    : { background: STAT_CARDS_LIGHT_BG[card.color] }
                }
              >
                <Text size="sm" weight={600} color="dimmed" mb="sm" lineClamp={1}>
                  {card.title}
                </Text>
                <Group spacing="sm">
                  <ThemeIcon variant="light" size={44} radius="md" color={card.color}>
                    <Icon size={22} />
                  </ThemeIcon>
                  <Stack spacing={0}>
                    <Text size={26} weight={700} lh={1.1}>
                      {dashboardData?.stats?.[card.key] ?? 0}
                    </Text>
                    <Text size="xs" color="dimmed">{card.subtitle}</Text>
                  </Stack>
                </Group>
              </Paper>
            </Grid.Col>
          );
        })}
      </Grid>

      {/* ── Loading overlay for refresh ── */}
      {loading && dashboardData && (
        <LoadingOverlay visible overlayOpacity={0.15} />
      )}

      {/* ── Charts ── */}
      <Grid gutter="lg">
        <Grid.Col lg={6}>
          <Paper p="lg" radius="md" withBorder className={classes.chartPaper}>
            <Text weight={600} mb="md">Popular Services</Text>
            <Box style={{ height: 300 }}>
              <Bar
                data={popularServicesChartData}
                options={{
                  ...chartOptions,
                  plugins: {
                    ...chartOptions.plugins,
                    title: { display: true, text: "Most Requested Services", font: { size: 13 } },
                  },
                }}
              />
            </Box>
          </Paper>
        </Grid.Col>

        <Grid.Col lg={6}>
          <Paper p="lg" radius="md" withBorder className={classes.chartPaper}>
            <Text weight={600} mb="md">Task Status Distribution</Text>
            <Box style={{ height: 300 }}>
              <Bar
                data={taskStatusDistributionChartData}
                options={{
                  ...chartOptions,
                  plugins: {
                    ...chartOptions.plugins,
                    title: { display: true, text: "Task Status Breakdown", font: { size: 13 } },
                  },
                }}
              />
            </Box>
          </Paper>
        </Grid.Col>

        <Grid.Col lg={6}>
          <Paper p="lg" radius="md" withBorder className={classes.chartPaper}>
            <Text weight={600} mb="md">Billing Summary</Text>
            <Box style={{ height: 280 }}>
              <Pie data={billingSummaryChartData} options={pieChartOptions} />
            </Box>
            <Divider my="sm" />
            <Group position="apart">
              <Badge size="lg" variant="outline" color="dark">
                Total: ${dashboardData?.billingSummary?.totalAmount?.toFixed(2) || '0.00'}
              </Badge>
              <Badge size="lg" variant="light" color="green">
                Paid: ${dashboardData?.billingSummary?.paidAmount?.toFixed(2) || '0.00'}
              </Badge>
              <Badge size="lg" variant="light" color="red">
                Unpaid: ${dashboardData?.billingSummary?.unpaidAmount?.toFixed(2) || '0.00'}
              </Badge>
            </Group>
          </Paper>
        </Grid.Col>

        <Grid.Col lg={6}>
          <Paper p="lg" radius="md" withBorder className={classes.chartPaper}>
            <Text weight={600} mb="md">Wage Summary</Text>
            <Box style={{ height: 280 }}>
              <Pie data={wageSummaryChartData} options={pieChartOptions} />
            </Box>
            <Divider my="sm" />
            <Group position="apart">
              <Badge size="lg" variant="outline" color="dark">
                Total: ${dashboardData?.wageSummary?.totalAmount?.toFixed(2) || '0.00'}
              </Badge>
              <Badge size="lg" variant="light" color="green">
                Paid: ${dashboardData?.wageSummary?.paidAmount?.toFixed(2) || '0.00'}
              </Badge>
              <Badge size="lg" variant="light" color="red">
                Unpaid: ${dashboardData?.wageSummary?.unpaidAmount?.toFixed(2) || '0.00'}
              </Badge>
            </Group>
          </Paper>
        </Grid.Col>

        <Grid.Col lg={6}>
          <Paper p="lg" radius="md" withBorder className={classes.chartPaper}>
            <Text weight={600} mb="md">Billing Trend (6 Months)</Text>
            <Box style={{ height: 300 }}>
              <Line
                data={billingTrendChartData}
                options={{
                  ...chartOptions,
                  plugins: {
                    ...chartOptions.plugins,
                    title: { display: true, text: "Monthly Billing Amounts", font: { size: 13 } },
                  },
                }}
              />
            </Box>
          </Paper>
        </Grid.Col>

        <Grid.Col lg={6}>
          <Paper p="lg" radius="md" withBorder className={classes.chartPaper}>
            <Text weight={600} mb="md">Wage Trend (6 Months)</Text>
            <Box style={{ height: 300 }}>
              <Line
                data={wageTrendChartData}
                options={{
                  ...chartOptions,
                  plugins: {
                    ...chartOptions.plugins,
                    title: { display: true, text: "Monthly Wage Amounts", font: { size: 13 } },
                  },
                }}
              />
            </Box>
          </Paper>
        </Grid.Col>

        <Grid.Col lg={6}>
          <Paper p="lg" radius="md" withBorder className={classes.chartPaper}>
            <Text weight={600} mb="md">Task Confirmation Distribution</Text>
            <Box style={{ height: 300 }}>
              <Doughnut data={taskStatusChartData} options={pieChartOptions} />
            </Box>
          </Paper>
        </Grid.Col>
      </Grid>
    </AppContainer>
  );
};

export default FranchiseDashboard;
