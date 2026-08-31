import { LoadingOverlay, TextInput, Checkbox, Button, Group, Badge } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Moment from "moment";
import {
  AppTable,
  AppDrawer,
  FilterSection,
  AppContainer,
  AppConfirmationModal,
} from "shared/components";
import { localStoreService, toConfirmService } from "core/services";
import { DataTable } from "mantine-datatable";
import { IconEdit, IconSend, IconTrash, IconDownload, IconCalendar, IconUser, IconUserCheck, IconReceipt, IconClipboard } from "@tabler/icons";
import { notifications } from "@mantine/notifications";
import { useFranchise } from "core/context/FranchiseContext";

const ToConfirm = () => {
  const { franchiseName } = useParams();
  const { franchiseId, loading: franchiseLoading } = useFranchise();
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [clientName, setClientName] = useState("");
  const [serviceProviderName, setServiceProviderName] = useState("");
  const [clientPhoneNumber, setClientPhoneNumber] = useState("");
  const [clientEmail, setClientEmail] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);
  const [clientUserNo, setClientUserNo] = useState("");
  const [serviceProviderUserNo, setServiceProviderUserNo] = useState("");
  const [taskId, setTaskId] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [servicesTasks, setServicesTasks] = useState([]);
  const [selectedServicesTasks, setSelectedServicesTasks] = useState([]);
  const [isConfirmationModalOpen, setIsConfirmationModalOpen] = useState(false);
  const [tableHeight, setTableHeight] = useState(500);


  const [opened, { open, close }] = useDisclosure(false);

  const pageSize = 25;

  const tableColumns = [
    {
      accessor: "recordType",
      title: "Type",
      textAlignment: "left",
      render: (record) => (
        <Badge
          className="app-status-badge"
          color={record.recordType === "Task" ? "brand" : "teal"}
          variant="light"
        >
          {record.recordType}
        </Badge>
      ),
      noWrap: true,
    },
    {
      accessor: "isConfirmed",
      title: "Confirmed",
      textAlignment: "left",
      render: (record) => (record.isConfirmed ? "Yes" : "No"),
      noWrap: true,
    },
    {
      accessor: "taskId",
      title: "Task ID",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "startTime",
      title: "Start",
      textAlignment: "left",
        render: (record) => Moment(record.startTime).format("h:mm a"),
      noWrap: true,
    },
    {
      accessor: "endTime",
      title: "End",
      textAlignment: "left",
        render: (record) => Moment(record.endTime).format("h:mm a"),
      noWrap: true,
    },
    {
      accessor: "date",
      title: "Date",
      textAlignment: "left",
      render: (record) => Moment(record.date).format("YYYY-MM-DD"),
      noWrap: true,
      },
     {
          accessor: "checkInTime",
          title: "Check In",
          textAlignment: "left",
         render: (record) => record.checkInTime ? Moment(record.checkInTime).format("h:mm a") : '-',
          noWrap: true,
     },
     {
          accessor: "checkOutTime",
          title: "Check Out",
          textAlignment: "left",
         render: (record) => record.checkOutTime ? Moment(record.checkOutTime).format("h:mm a") : '-',
          noWrap: true,
     },
    {
      accessor: "serviceType",
      title: "Service Type",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "clientUserNo",
      title: "Client UserNo",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "clientName",
      title: "Client Name",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "clientEmail",
      title: "Client Email",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "clientPhone",
      title: "Client Phone",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "clientMobile",
      title: "Client Mobile",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "serviceProviderUserNo",
      title: "ServiceProvider UserNo",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "serviceProviderName",
      title: "ServiceProvider Name",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "serviceProviderEmail",
      title: "ServiceProvider Email",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "serviceProviderPhone",
      title: "ServiceProvider Phone",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "serviceProviderMobile",
      title: "ServiceProvider Mobile",
      textAlignment: "left",
      noWrap: true,
    },

  ];

  useEffect(() => {
    if (franchiseId && !franchiseLoading) {
      getServicesTasks();
    }
  }, [pageNumber, franchiseId, franchiseLoading]);

  const getServicesTasks = async (filterOverrides = {}) => {
    const obj = {};

    obj.taskId = filterOverrides.taskId ?? taskId;
    obj.startDate =
      filterOverrides.startDate !== undefined
        ? filterOverrides.startDate || null
        : startDate
          ? startDate
          : null;
    obj.endDate =
      filterOverrides.endDate !== undefined
        ? filterOverrides.endDate || null
        : endDate
          ? endDate
          : null;
    obj.clientUserNo = filterOverrides.clientUserNo ?? clientUserNo;
    obj.clientName = filterOverrides.clientName ?? clientName;
    obj.serviceProviderUserNo =
      filterOverrides.serviceProviderUserNo ?? serviceProviderUserNo;
    obj.serviceProviderName =
      filterOverrides.serviceProviderName ?? serviceProviderName;
    obj.franchiseId = franchiseId; // Use franchise ID from context
    obj.pageNumber = filterOverrides.pageNumber ?? pageNumber;
    obj.pageSize = pageSize;

    setIsLoading(true);
    const { response, totalRecords } = await toConfirmService.getServicesTasks(obj);
    setServicesTasks(response);
    setTotalRecords(totalRecords);
    setIsLoading(false);
  };

  const handlePagination = (pageNumber) => {
    setPageNumber(pageNumber);
  };

  const handleFilter = async () => {
    close();
    if (pageNumber !== 1) {
      setPageNumber(1);
      return;
    }
    await getServicesTasks({ pageNumber: 1 });
  };

  useEffect(() => {
    const calculateTableHeight = () => {
      const windowHeight = window.innerHeight;
      // Account for header, navigation, padding, and status buttons
      const reservedHeight = 200; // Adjust this value based on your layout
      const calculatedHeight = Math.max(400, windowHeight - reservedHeight);
      setTableHeight(calculatedHeight);
    };

    // Calculate initial height
    calculateTableHeight();

    // Recalculate on window resize
    window.addEventListener('resize', calculateTableHeight);

    // Cleanup
    return () => window.removeEventListener('resize', calculateTableHeight);
  }, []);

  const handleReset = async () => {
    setClientUserNo("");
    setClientName("");
    setServiceProviderUserNo("");
    setServiceProviderName("");
    setTaskId("");
    setStartDate("");
    setEndDate("");
    close();

    if (pageNumber !== 1) {
      setPageNumber(1);
      return;
    }

    await getServicesTasks({
      clientUserNo: "",
      clientName: "",
      serviceProviderUserNo: "",
      serviceProviderName: "",
      taskId: "",
      startDate: "",
      endDate: "",
      pageNumber: 1,
    });
  };

  const handleProfileDetail = (selectedRow) => { };

  const handleDownloadExcel = () => {
    if (!servicesTasks || servicesTasks.length === 0) return;

    // Prepare data for Excel export
    const excelData = servicesTasks.map((record, index) => ({
      'Sr No': index + 1,
      'Is Confirmed': record.isConfirmed ? "Yes" : "No",
      'Task ID': record.taskId || '-',
      'Schedule ID': record.scheduleId || '-',
      'Start Time': record.startTime ? Moment(record.startTime).format("HH:mm") : '-',
      'End Time': record.endTime ? Moment(record.endTime).format("HH:mm") : '-',
      'Date': record.date ? Moment(record.date).format("YYYY-MM-DD") : '-',
      'Client User No': record.clientUserNo || '-',
      'Client Name': record.clientName || '-',
      'Client Email': record.clientEmail || '-',
      'Client Phone': record.clientPhone || '-',
      'Service Provider User No': record.serviceProviderUserNo || '-',
      'Service Provider Name': record.serviceProviderName || '-',
      'Service Type': record.serviceType || '-',
      'Notes': record.notes || '-'
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
    let filename = `ToConfirm_Tasks_${Moment().format('YYYY-MM-DD')}`;
    if (startDate && endDate) {
      filename = `ToConfirm_Tasks_${startDate}_to_${endDate}`;
    } else if (startDate) {
      filename = `ToConfirm_Tasks_from_${startDate}`;
    } else if (endDate) {
      filename = `ToConfirm_Tasks_until_${endDate}`;
    }
    
    link.setAttribute('download', `${filename}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const handleOpenClientProfile = (selectedRow) => {
    if (selectedRow.clientId) {
      navigate(`/franchises/${franchiseName}/profile/${selectedRow.clientId}/1`);
    } else {
      notifications.show({
        title: "Error",
        message: "Client ID not available",
        color: "red",
      });
    }
  };

  const handleOpenServiceProviderProfile = (selectedRow) => {
    if (selectedRow.serviceProviderId) {
      navigate(`/franchises/${franchiseName}/profile/${selectedRow.serviceProviderId}/2`);
    } else {
      notifications.show({
        title: "Error",
        message: "Service Provider ID not available",
        color: "red",
      });
    }
  };

  return (
    <>
      <AppContainer
        title="To Confirm"
        showDivider="true"
        button={
          <Group spacing="sm">
            <Button
              disabled={selectedServicesTasks.length === 0}
              onClick={() => setIsConfirmationModalOpen(true)}
              variant="filled"
              size="sm"
            >
              Confirm Tasks
            </Button>
            <Button
              onClick={open}
              variant="light"
              size="sm"
            >
              Filter
            </Button>
            <Button
              variant="filled"
              color="teal"
              size="sm"
              leftIcon={<IconDownload size={16} />}
              onClick={handleDownloadExcel}
              disabled={!servicesTasks || servicesTasks.length === 0}
            >
              Download Excel
            </Button>
          </Group>
        }
      >
        <LoadingOverlay visible={isLoading} />

        <DataTable
          height="70vh"
          striped
          highlightOnHover
          columns={tableColumns}
          records={servicesTasks}
          noRecordsText="No records to show"
          rowContextMenu={{
            items: (record) => [
              {
                key: "open client profile",
                icon: <IconUser size={16} />,
                onClick: () => handleOpenClientProfile(record),
              },
              {
                key: "open service provider profile",
                icon: <IconUserCheck size={16} />,
                onClick: () => handleOpenServiceProviderProfile(record),
              },
            ],
          }}
          totalRecords={totalRecords}
          recordsPerPage={pageSize}
          page={pageNumber}
          onPageChange={(p) => handlePagination(p)}
          paginationSize="lg"
          selectedRecords={selectedServicesTasks.filter(record => record.recordType === 'Task')}
          onSelectedRecordsChange={(records) => {
            // Only allow selection of Task records, filter out Expense records
            const taskRecords = records.filter(record => record.recordType === 'Task');
            setSelectedServicesTasks(taskRecords);
          }}
          idAccessor={(record) => {
            // Create unique ID by combining record type with appropriate ID
            if (record.recordType === 'Task') {
              return `task_${record.taskId}`;
            } else {
              return `expense_${record.expenseId}`;
            }
          }}
        />
      </AppContainer>

      <AppDrawer
        opened={opened}
        close={close}
        onFilter={handleFilter}
        onReset={handleReset}
        title="To confirm filters"
        description="Limit rows before confirming billing and wages. Empty fields are ignored."
      >
        <form>
          <FilterSection title="Client">
            <TextInput
              label="User number"
              description="Format e.g. 1-1-1"
              placeholder="Search by user number"
              size="md"
              value={clientUserNo}
              onChange={(e) => setClientUserNo(e.target.value)}
            />
            <TextInput
              label="Name"
              placeholder="First or last name"
              size="md"
              value={clientName}
              onChange={(e) => setClientName(e.target.value)}
            />
          </FilterSection>

          <FilterSection title="Service provider">
            <TextInput
              label="User number"
              description="Format e.g. 1-1-1"
              placeholder="Search by user number"
              size="md"
              value={serviceProviderUserNo}
              onChange={(e) => setServiceProviderUserNo(e.target.value)}
            />
            <TextInput
              label="Name"
              placeholder="First or last name"
              size="md"
              value={serviceProviderName}
              onChange={(e) => setServiceProviderName(e.target.value)}
            />
          </FilterSection>

          <FilterSection title="Task">
            <TextInput
              label="Task ID"
              placeholder="Numeric task id"
              size="md"
              value={taskId}
              onChange={(e) => setTaskId(e.target.value)}
            />
          </FilterSection>

          <FilterSection title="Date range">
            <Group grow align="flex-start">
              <TextInput
                label="Start"
                type="date"
                size="md"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                icon={<IconCalendar size={16} />}
              />
              <TextInput
                label="End"
                type="date"
                size="md"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                icon={<IconCalendar size={16} />}
              />
            </Group>
          </FilterSection>
        </form>
      </AppDrawer>

      <AppConfirmationModal
        opened={isConfirmationModalOpen}
        onClose={async (confirmed) => {
          if (confirmed) {
            // Since we only allow selection of Task records, selectedServicesTasks contains only tasks
            const tasks = selectedServicesTasks;
            
            // Confirm tasks - this will automatically confirm associated expenses
            if (tasks.length > 0) {
              await toConfirmService.CalculateBillingAndWageAmounts(
                tasks.map((row) => row.taskId).join(","),
                localStoreService.getOrganizationID()
              );
            }

            setSelectedServicesTasks([]);
            setIsConfirmationModalOpen(false);
            getServicesTasks();
          } else setIsConfirmationModalOpen(false);
        }}
        title="Confirm Tasks"
      >
        Are you sure you want to confirm the selected tasks? 
        <br />
        <small style={{ color: '#666' }}>
          Note: Confirming tasks will automatically confirm their associated expenses.
        </small>
      </AppConfirmationModal>
    </>
  );
};

export default ToConfirm;
