import React, { useEffect, useState } from "react";
import {
  LoadingOverlay,
  TextInput,
  Select,
  Button,
  Group,
  Stack,
  Text,
  Badge,
  Paper,
  Divider,
  Alert,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { DataTable } from "mantine-datatable";
import { IconCalendar, IconDownload, IconFilter, IconRefresh } from "@tabler/icons";
import Moment from "moment";
import { notifications } from "@mantine/notifications";
import { AppContainer, AppDrawer } from "shared/components";
import { localStoreService, loginHistoryService } from "core/services";

const LoginHistory = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [loginHistory, setLoginHistory] = useState([]);
  const [totalRecords, setTotalRecords] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(25);
  
  // Filter states
  const [userId, setUserId] = useState("");
  const [userType, setUserType] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [loginStatus, setLoginStatus] = useState("");
  const [ipAddress, setIpAddress] = useState("");
  
  const [opened, { open, close }] = useDisclosure(false);

  const userTypeOptions = [
    { value: "1", label: "Client" },
    { value: "2", label: "Service Provider" },
    { value: "3", label: "Staff" },
  ];

  const loginStatusOptions = [
    { value: "Success", label: "Success" },
    { value: "Failed", label: "Failed" },
    { value: "Blocked", label: "Blocked" },
  ];

  useEffect(() => {
    getLoginHistory();
  }, [pageNumber]);

  const getLoginHistory = async (filterOverrides = {}) => {
    try {
      setIsLoading(true);
      
      const request = {
        organizationId: localStoreService.getOrganizationID(),
        userId: filterOverrides.userId !== undefined ? filterOverrides.userId || null : userId || null,
        userType:
          filterOverrides.userType !== undefined
            ? filterOverrides.userType
              ? parseInt(filterOverrides.userType)
              : null
            : userType
              ? parseInt(userType)
              : null,
        startDate:
          filterOverrides.startDate !== undefined
            ? filterOverrides.startDate
              ? new Date(filterOverrides.startDate).toISOString()
              : null
            : startDate
              ? new Date(startDate).toISOString()
              : null,
        endDate:
          filterOverrides.endDate !== undefined
            ? filterOverrides.endDate
              ? new Date(filterOverrides.endDate).toISOString()
              : null
            : endDate
              ? new Date(endDate).toISOString()
              : null,
        loginStatus:
          filterOverrides.loginStatus !== undefined
            ? filterOverrides.loginStatus || null
            : loginStatus || null,
        ipAddress:
          filterOverrides.ipAddress !== undefined
            ? filterOverrides.ipAddress || null
            : ipAddress || null,
        pageNumber: filterOverrides.pageNumber ?? pageNumber,
        pageSize,
        sortColumn: "LoginTime",
        sortDirection: "DESC"
      };

      const response = await loginHistoryService.getLoginHistory(request);
      
      if (response) {
        setLoginHistory(response.entries || []);
        setTotalRecords(response.totalRecords || 0);
      } else {
        notifications.show({
          title: "Error",
          message: "Failed to fetch login history",
          color: "red",
        });
      }
    } catch (error) {
      console.error("Error fetching login history:", error);
      notifications.show({
        title: "Error",
        message: "An error occurred while fetching login history",
        color: "red",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleFilter = async () => {
    close();
    if (pageNumber !== 1) {
      setPageNumber(1);
      return;
    }
    await getLoginHistory({ pageNumber: 1 });
  };

  const handleReset = async () => {
    setUserId("");
    setUserType("");
    setStartDate("");
    setEndDate("");
    setLoginStatus("");
    setIpAddress("");
    close();

    if (pageNumber !== 1) {
      setPageNumber(1);
      return;
    }

    await getLoginHistory({
      userId: "",
      userType: "",
      startDate: "",
      endDate: "",
      loginStatus: "",
      ipAddress: "",
      pageNumber: 1,
    });
  };

  const handlePagination = (pageNumber) => {
    setPageNumber(pageNumber);
  };

  const handleDownloadExcel = () => {
    if (!loginHistory || loginHistory.length === 0) return;

    // Prepare data for Excel export
    const excelData = loginHistory.map((record, index) => ({
      'Sr No': index + 1,
      'User Name': record.userName || '-',
      'User Email': record.userEmail || '-',
      'User Type': record.userTypeName || '-',
      'Login Time': record.loginTime ? Moment(record.loginTime).format("YYYY-MM-DD HH:mm:ss") : '-',
      'Logout Time': record.logoutTime ? Moment(record.logoutTime).format("YYYY-MM-DD HH:mm:ss") : '-',
      'Session Duration (min)': record.calculatedSessionDuration || '-',
      'IP Address': record.ipAddress || '-',
      'Browser': record.browserName || '-',
      'Operating System': record.operatingSystem || '-',
      'Device Type': record.deviceType || '-',
      'Login Status': record.loginStatus || '-',
      'Franchise': record.franchiseName || '-'
    }));

    // Convert to CSV format
    const headers = Object.keys(excelData[0]);
    const csvContent = [
      headers.join(','),
      ...excelData.map(row => 
        headers.map(header => {
          const value = row[header];
          // Escape commas and quotes in CSV
          if (typeof value === 'string' && (value.includes(',') || value.includes('"'))) {
            return `"${value.replace(/"/g, '""')}"`;
          }
          return value;
        }).join(',')
      )
    ].join('\n');

    // Create and download file
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    
    // Create filename with date range if available
    let filename = `Login_History_${Moment().format('YYYY-MM-DD')}`;
    if (startDate && endDate) {
      filename = `Login_History_${startDate}_to_${endDate}`;
    } else if (startDate) {
      filename = `Login_History_from_${startDate}`;
    } else if (endDate) {
      filename = `Login_History_until_${endDate}`;
    }
    
    link.setAttribute('download', `${filename}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const getStatusColor = (status) => {
    switch (status?.toLowerCase()) {
      case 'success': return 'green';
      case 'failed': return 'red';
      case 'blocked': return 'orange';
      default: return 'gray';
    }
  };

  const tableColumns = [
    {
      accessor: "userName",
      title: "User Name",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "userEmail",
      title: "User Email",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "userTypeName",
      title: "User Type",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "loginTime",
      title: "Login Time",
      textAlignment: "left",
      render: (record) => Moment(record.loginTime).format("YYYY-MM-DD HH:mm:ss"),
      noWrap: true,
    },
    {
      accessor: "logoutTime",
      title: "Logout Time",
      textAlignment: "left",
      render: (record) => record.logoutTime ? Moment(record.logoutTime).format("YYYY-MM-DD HH:mm:ss") : "Active",
      noWrap: true,
    },
    {
      accessor: "calculatedSessionDuration",
      title: "Session Duration",
      textAlignment: "left",
      render: (record) => {
        if (record.calculatedSessionDuration !== null) {
          const hours = Math.floor(record.calculatedSessionDuration / 60);
          const minutes = record.calculatedSessionDuration % 60;
          if (hours > 0) {
            return `${hours}h ${minutes}m`;
          }
          return `${minutes}m`;
        }
        return "Active";
      },
      noWrap: true,
    },
    {
      accessor: "ipAddress",
      title: "IP Address",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "browserName",
      title: "Browser",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "operatingSystem",
      title: "OS",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "deviceType",
      title: "Device",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "loginStatus",
      title: "Status",
      textAlignment: "left",
      render: (record) => (
        <Badge color={getStatusColor(record.loginStatus)} variant="filled">
          {record.loginStatus}
        </Badge>
      ),
      noWrap: true,
    },
    {
      accessor: "franchiseName",
      title: "Franchise",
      textAlignment: "left",
      noWrap: true,
    },
  ];

  return (
    <>
      <AppContainer
        title="Login History"
        button={
          <Group spacing="xs">
            <Button
              variant="light"
              size="sm"
              onClick={open}
              leftIcon={<IconFilter size={16} />}
            >
              Filter
            </Button>
            <Button
              variant="filled"
              color="teal"
              size="sm"
              leftIcon={<IconDownload size={16} />}
              onClick={handleDownloadExcel}
              disabled={!loginHistory || loginHistory.length === 0}
            >
              Download Excel
            </Button>
            <Button
              variant="default"
              size="sm"
              leftIcon={<IconRefresh size={16} />}
              onClick={getLoginHistory}
            >
              Refresh
            </Button>
          </Group>
        }
        showDivider="true"
      >
        <LoadingOverlay visible={isLoading} />

        {loginHistory.length === 0 && !isLoading && (
          <Alert
            icon={<IconCalendar size={16} />}
            title="No Login History"
            color="brand"
            variant="light"
            mt="md"
          >
            No login history records found for the selected criteria.
          </Alert>
        )}

        <DataTable
          height="70vh"
          striped
          highlightOnHover
          columns={tableColumns}
          records={loginHistory}
          noRecordsText={isLoading ? 'Loading Login History...' : 'No login history found'}
          totalRecords={totalRecords}
          recordsPerPage={pageSize}
          page={pageNumber}
          onPageChange={(p) => handlePagination(p)}
          paginationSize="lg"
          idAccessor="id"
        />
      </AppContainer>

      <AppDrawer
        opened={opened}
        close={close}
        onFilter={handleFilter}
        onReset={handleReset}
      >
        <form>
          <TextInput
            label="User ID"
            placeholder="Enter user ID"
            size="md"
            mt="md"
            value={userId}
            onChange={(e) => setUserId(e.target.value)}
          />
          
          <Select
            label="User Type"
            placeholder="Select user type"
            size="md"
            mt="md"
            value={userType}
            onChange={setUserType}
            data={userTypeOptions}
            clearable
          />

          <Stack spacing="xs" mt="md">
            <Text size="sm" weight={500} color="dimmed">
              Date Range
            </Text>
            <Group grow>
              <TextInput
                label="Start Date"
                placeholder="Select start date"
                type="date"
                size="md"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                icon={<IconCalendar size={16} />}
              />
              <TextInput
                label="End Date"
                placeholder="Select end date"
                type="date"
                size="md"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                icon={<IconCalendar size={16} />}
              />
            </Group>
          </Stack>

          <Select
            label="Login Status"
            placeholder="Select login status"
            size="md"
            mt="md"
            value={loginStatus}
            onChange={setLoginStatus}
            data={loginStatusOptions}
            clearable
          />

          <TextInput
            label="IP Address"
            placeholder="Enter IP address"
            size="md"
            mt="md"
            value={ipAddress}
            onChange={(e) => setIpAddress(e.target.value)}
          />
        </form>
      </AppDrawer>
    </>
  );
};

export default LoginHistory;
