import {
  Button,
  LoadingOverlay,
  TextInput,
  MultiSelect,
  Group,
  Stack,
  Text,
  Badge,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Moment from "moment";
import {
  localStoreService,
  planboardService,
  lookupService,
} from "core/services";
import {
  AppTable,
  AppContainer,
  AppDrawer,
  UpdateAppointment,
  AppModal,
  TaskLogModal,
  StatusLegend,
} from "shared/components";
import { useFranchise } from "core/context/FranchiseContext";
import { TaskExpenses } from "shared/components/planboard/TaskExpenses";
import { AddTaskExpense } from "shared/components/planboard/AddTaskExpense";
import { notifications } from "@mantine/notifications";
import { DataTable } from "mantine-datatable";
import { IconEdit, IconSend, IconTrash, IconDownload, IconCalendar, IconHistory, IconUser, IconUserCheck, IconReceipt, IconPlus } from "@tabler/icons";

const Planboard = () => {
  const { franchiseName } = useParams();
  const { franchiseId, loading: franchiseLoading } = useFranchise();
  const navigate = useNavigate();
  
  // Components imported successfully
  const [isLoading, setIsLoading] = useState(false);
  const [servicesTasks, setServicesTasks] = useState([]);
  const [clientName, setClientName] = useState("");
  const [serviceProviderName, setServiceProviderName] = useState("");
  const [clientUserNo, setClientUserNo] = useState("");
  const [serviceProviderUserNo, setServiceProviderUserNo] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [taskId, setTaskId] = useState("");
  const [taskStatus, setTaskStatus] = useState([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);
  const [statusOptions, setStatusOptions] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedRecord, setSelectedRecord] = useState(0);
  const [tableHeight, setTableHeight] = useState(500);
  const [isTaskLogModalOpen, setIsTaskLogModalOpen] = useState(false);
  const [selectedTaskForLog, setSelectedTaskForLog] = useState(null);
  const [isTaskExpensesModalOpen, setIsTaskExpensesModalOpen] = useState(false);
  const [isAddExpenseModalOpen, setIsAddExpenseModalOpen] = useState(false);
  const [selectedTaskForExpense, setSelectedTaskForExpense] = useState(null);

  const [opened, { open, close }] = useDisclosure(false);

  /** Maps DataTable column accessors to [uspGetPlanboardTasks] @pSortColumn values */
  const planboardSortColumnMap = {
    taskId: "TaskId",
    startTime: "StartTime",
    endTime: "EndTime",
    date: "Date",
    serviceType: "ServiceType",
    clientName: "ClientName",
    clientEmail: "ClientEmail",
    clientPhone: "ClientPhone",
    clientMobile: "ClientMobile",
    serviceProviderName: "ServiceProviderName",
    serviceProviderEmail: "ServiceProviderEmail",
    serviceProviderPhone: "ServiceProviderPhone",
    serviceProviderMobile: "ServiceProviderMobile",
  };

  const [sortStatus, setSortStatus] = useState({
    columnAccessor: "taskId",
    direction: "desc",
  });

  const pageSize = 25;

  const statusBadge = (status) => {
    const map = {
      Scheduled: { color: "violet", label: "Scheduled" },
      Delayed: { color: "red", label: "Delayed" },
      "In-Progress": { color: "teal", label: "In Progress" },
      Completed: { color: "brand", label: "Completed" },
      Cancelled: { color: "yellow", label: "Cancelled" },
      Unassigned: { color: "gray", label: "Unassigned" },
    };
    const cfg = map[status] || { color: "gray", label: status || "—" };
    return (
      <Badge className="app-status-badge" color={cfg.color} variant="light" radius="sm" size="md">
        {cfg.label}
      </Badge>
    );
  };

  const stickyCol = {
    position: "sticky",
    left: 0,
    zIndex: 1,
    background: "inherit",
  };

  const tableColumns = [
    {
      accessor: "taskId",
      title: "Task ID",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
      titleStyle: stickyCol,
      cellsStyle: stickyCol,
    },
    {
      accessor: "startTime",
      title: "Start",
      textAlignment: "left",
      render: (record) => Moment(record.startTime).format("h:mm a"),
      noWrap: true,
      sortable: true,
    },
    {
      accessor: "endTime",
      title: "End",
      textAlignment: "left",
      render: (record) => Moment(record.endTime).format("h:mm a"),
      noWrap: true,
      sortable: true,
    },
    {
      accessor: "date",
      title: "Date",
      textAlignment: "left",
      render: (record) => Moment(record.date).format("YYYY-MM-DD"),
      noWrap: true,
      sortable: true,
    },
    {
      accessor: "CheckInTime",
      title: "Check-in",
      textAlignment: "left",
      render: (record) => record.checkInTime ? Moment(record.checkInTime).format("h:mm a") : "—",
      noWrap: true,
      hidden: true,
    },
    {
      accessor: "CheckOutTime",
      title: "Check-out",
      textAlignment: "left",
      render: (record) => record.checkOutTime ? Moment(record.checkOutTime).format("h:mm a") : "—",
      noWrap: true,
      hidden: true,
    },
    {
      accessor: "taskStatus",
      title: "Status",
      textAlignment: "left",
      render: (record) => statusBadge(record.taskStatus),
      noWrap: true,
    },
    {
      accessor: "serviceType",
      title: "Service Type",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
    },
    //{
    //  accessor: "serviceName",
    //  title: "Service Name",
    //  textAlignment: "left",
    //  noWrap: true,
    //},
    {
      accessor: "clientUserNo",
      title: "Client No",
      textAlignment: "left",
      noWrap: true,
      hidden: true,
    },
    {
      accessor: "clientName",
      title: "Client",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
    },
    {
      accessor: "clientEmail",
      title: "Client Email",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
      hidden: true,
    },
    {
      accessor: "clientPhone",
      title: "Client Phone",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
      hidden: true,
    },
    {
      accessor: "clientMobile",
      title: "Client Mobile",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
      hidden: true,
    },
    {
      accessor: "serviceProviderUserNo",
      title: "Provider No",
      textAlignment: "left",
      noWrap: true,
      hidden: true,
    },
    {
      accessor: "serviceProviderName",
      title: "Provider",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
    },
    {
      accessor: "serviceProviderEmail",
      title: "Provider Email",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
      hidden: true,
    },
    {
      accessor: "serviceProviderPhone",
      title: "Provider Phone",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
      hidden: true,
    },
    {
      accessor: "serviceProviderMobile",
      title: "Provider Mobile",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
      hidden: true,
    },
    {
      accessor: "isConfirmed",
      title: "Confirmed",
      textAlignment: "left",
      render: (record) => (
        <Badge
          className="app-status-badge"
          color={record.isConfirmed ? "teal" : "gray"}
          variant="light"
        >
          {record.isConfirmed ? "Yes" : "No"}
        </Badge>
      ),
      noWrap: true,
    },
  ];

  useEffect(() => {
    if (franchiseId && !franchiseLoading) {
      getServicesTasks();
    }
  }, [pageNumber, franchiseId, franchiseLoading, sortStatus.columnAccessor, sortStatus.direction]);

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

  const getServicesTasks = async (filterOverrides = {}) => {
    const obj = {};

    const nextTaskStatus = filterOverrides.taskStatus ?? taskStatus;

    obj.taskStatusIds = Array.isArray(nextTaskStatus)
      ? nextTaskStatus.toString()
      : nextTaskStatus || "";
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
    obj.sortColumn =
      planboardSortColumnMap[sortStatus.columnAccessor] ?? "TaskId";
    obj.sortType = sortStatus.direction === "asc" ? "ASC" : "DESC";

    setIsLoading(true);
    const { response, totalRecords } = await planboardService.getServicesTasks(obj);
    setServicesTasks(response);
    setTotalRecords(totalRecords);
    setIsLoading(false);
  };

  const handlePagination = (pageNumber) => {
    setPageNumber(pageNumber);
  };

  const handleSortStatusChange = (next) => {
    setSortStatus(next);
    setPageNumber(1);
  };

  const handleFilter = async () => {
    close();
    if (pageNumber !== 1) {
      setPageNumber(1);
      return;
    }
    await getServicesTasks({ pageNumber: 1 });
  };

  const handleReset = async () => {
    setServiceProviderName("");
    setClientName("");
    setClientUserNo("");
    setServiceProviderUserNo("");
    setStartDate("");
    setEndDate("");
    setTaskId("");
    setTaskStatus([]);
    close();

    if (pageNumber !== 1) {
      setPageNumber(1);
      return;
    }

    await getServicesTasks({
      serviceProviderName: "",
      clientName: "",
      clientUserNo: "",
      serviceProviderUserNo: "",
      startDate: "",
      endDate: "",
      taskId: "",
      taskStatus: [],
      pageNumber: 1,
    });
  };

  useEffect(() => {
    const fetchLookupData = async () => {
      try {
        const typeResponse = await lookupService.getLookupList({
          lookupType: "TaskStatus",
        });
        setStatusOptions(
          (typeResponse?.result || []).map((item) => ({
            value: item.id,
            label: item.name,
          }))
        );
      } catch (error) {
        notifications.show({
          title: "Error",
          message: "Failed to fetch lookup data",
          color: "red",
        });
      }
    };

    if (statusOptions.length === 0) {
      fetchLookupData();
    }
  }, [statusOptions.length]);

  const handleProfileDetail = (selectedRow) => {
    setIsModalOpen(true);
    setSelectedRecord(selectedRow.taskId);
  };

  const handleViewTaskLog = (selectedRow) => {
    setSelectedTaskForLog({
      id: selectedRow.taskId,
      title: `Task #${selectedRow.taskId} - ${selectedRow.clientName || 'Unknown Client'}`
    });
    setIsTaskLogModalOpen(true);
  };

  const handleViewTaskExpenses = (selectedRow) => {
    setSelectedTaskForExpense({
      taskId: selectedRow.taskId,
      userId: selectedRow.serviceProviderId, // Assuming service provider is the user
      taskStatus: selectedRow.taskStatus,
      isConfirmed: selectedRow.isConfirmed,
    });
    setIsTaskExpensesModalOpen(true);
  };

  const handleAddExpenseToTask = (selectedRow) => {
    setSelectedTaskForExpense({
      taskId: selectedRow.taskId,
      userId: selectedRow.serviceProviderId, // Assuming service provider is the user
    });
    setIsAddExpenseModalOpen(true);
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

  const handleDownloadExcel = () => {
    if (!servicesTasks || servicesTasks.length === 0) return;

    // Prepare data for Excel export
    const excelData = servicesTasks.map((record, index) => ({
      'Sr No': index + 1,
      'Task ID': record.taskId || '-',
      'Start Time': record.startTime ? Moment(record.startTime).format("h:mm a") : '-',
      'End Time': record.endTime ? Moment(record.endTime).format("h:mm a") : '-',
      'Date': record.date ? Moment(record.date).format("YYYY-MM-DD") : '-',
      'Check In Time': record.checkInTime ? Moment(record.checkInTime).format("h:mm a") : '-',
      'Check Out Time': record.checkOutTime ? Moment(record.checkOutTime).format("h:mm a") : '-',
      'Task Status': record.taskStatus || '-',
      'Client User No': record.clientUserNo || '-',
      'Client Name': record.clientName || '-',
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
    let filename = `Planboard_Tasks_${Moment().format('YYYY-MM-DD')}`;
    if (startDate && endDate) {
      filename = `Planboard_Tasks_${startDate}_to_${endDate}`;
    } else if (startDate) {
      filename = `Planboard_Tasks_from_${startDate}`;
    } else if (endDate) {
      filename = `Planboard_Tasks_until_${endDate}`;
    }
    
    link.setAttribute('download', `${filename}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  return (
    <>
      <AppContainer
        title="Planboard"
        button={
          <Group spacing="xs">
            <Button
              variant="light"
              size="sm"
              onClick={open}
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
        showDivider="true"
      >
        <LoadingOverlay visible={isLoading} />

        <DataTable
          height="70vh"
          striped
          highlightOnHover
          columns={tableColumns}
          records={servicesTasks}
          sortStatus={sortStatus}
          onSortStatusChange={handleSortStatusChange}
          noRecordsText={isLoading ? 'Loading Planboard Task...' : 'No tasks found'}
          rowContextMenu={{
            items: (record) => {
              const isTaskCompletedAndConfirmed = record.taskStatus === 'Completed' && record.isConfirmed;
              
              const items = [
                {
                  key: "update appointment",
                  icon: <IconEdit size={16} />,
                  onClick: () => handleProfileDetail(record),
                },
                {
                  key: "view task log",
                  icon: <IconHistory size={16} />,
                  onClick: () => handleViewTaskLog(record),
                },
                {
                  key: "view task expenses",
                  icon: <IconReceipt size={16} />,
                  onClick: () => handleViewTaskExpenses(record),
                },
                // Only show add expense option if task is not completed and confirmed
                ...(isTaskCompletedAndConfirmed ? [] : [{
                  key: "add expense to task",
                  icon: <IconPlus size={16} />,
                  onClick: () => handleAddExpenseToTask(record),
                }]),
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
              ];
              return items;
            },
          }}
          totalRecords={totalRecords}
          recordsPerPage={pageSize}
          page={pageNumber}
          onPageChange={(p) => handlePagination(p)}
          paginationSize="sm"
          idAccessor="taskId"
        />
        <StatusLegend mt="md" mb="xs" />
      </AppContainer>

      <AppDrawer
        opened={opened}
        close={close}
        onFilter={handleFilter}
        onReset={handleReset}
      >
        <form>
          <TextInput
            label="Client UserNo"
            placeholder="1-1-1"
            size="md"
            mt="md"
            value={clientUserNo}
            onChange={(e) => setClientUserNo(e.target.value)}
          />
          <TextInput
            label="Client Name"
            placeholder="john"
            size="md"
            mt="md"
            value={clientName}
            onChange={(e) => setClientName(e.target.value)}
          />
          <TextInput
            label="Service Provider UserNo"
            placeholder="1-1-1"
            size="md"
            mt="md"
            value={serviceProviderUserNo}
            onChange={(e) => setServiceProviderUserNo(e.target.value)}
          />
          <TextInput
            label="Service Provider Name"
            placeholder="john"
            size="md"
            mt="md"
            value={serviceProviderName}
            onChange={(e) => setServiceProviderName(e.target.value)}
          />
          <TextInput
            label="Task Id"
            placeholder="1"
            size="md"
            mt="md"
            value={taskId}
            onChange={(e) => setTaskId(e.target.value)}
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

          <MultiSelect
            label="Task Status"
            placeholder="Select Status"
            size="md"
            mt="md"
            value={taskStatus}
            onChange={(values) => setTaskStatus(values)}
            data={statusOptions}
          />
        </form>
      </AppDrawer>
      <AppModal
         opened={isModalOpen}
         onClose={() => setIsModalOpen(false)}
         title={"Appointment"}
         size="xl"
       >
         <UpdateAppointment 
           taskID={selectedRecord} 
           franchiseName={franchiseName}
           onModalClose={async () => {
             setIsModalOpen(false);
             await getServicesTasks();
           }}
         />
       </AppModal>

       <TaskLogModal
         opened={isTaskLogModalOpen}
         onClose={() => setIsTaskLogModalOpen(false)}
         taskId={selectedTaskForLog?.id}
         taskTitle={selectedTaskForLog?.title}
       />

       <TaskExpenses
         opened={isTaskExpensesModalOpen}
         onClose={() => setIsTaskExpensesModalOpen(false)}
         taskId={selectedTaskForExpense?.taskId}
         userId={selectedTaskForExpense?.userId}
         organizationId={localStoreService.getOrganizationID()}
         taskStatus={selectedTaskForExpense?.taskStatus}
         isConfirmed={selectedTaskForExpense?.isConfirmed}
       />

       <AddTaskExpense
         opened={isAddExpenseModalOpen}
         onClose={() => setIsAddExpenseModalOpen(false)}
         taskId={selectedTaskForExpense?.taskId}
         userId={selectedTaskForExpense?.userId}
         organizationId={localStoreService.getOrganizationID()}
         onExpenseAdded={() => {
           // Refresh the expenses modal if it's open
           if (isTaskExpensesModalOpen) {
             // This will trigger a refresh when the user opens the expenses modal again
           }
         }}
       />
    </>
  );
};

export default Planboard;
