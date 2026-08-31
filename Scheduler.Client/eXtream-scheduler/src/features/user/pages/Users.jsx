import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Button, Group, LoadingOverlay, TextInput, Badge } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { AppTable, AppContainer, AppDrawer } from "shared/components";
import { localStoreService, profileService } from "core/services";
import { helperFunctions } from "shared/utils";
import { UserType } from "core/enum";
import { DataTable } from "mantine-datatable";
import { IconEdit, IconSend, IconTrash } from "@tabler/icons";
import { useFranchise } from "core/context/FranchiseContext";
import { TruncatedTooltipText } from "shared/components/TruncatedTooltipText";

const Users = ({ userTypeID }) => {
  const { franchiseId, loading: franchiseLoading } = useFranchise();
  const [isLoading, setIsLoading] = useState(false);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);
  const [clients, setClients] = useState([]);

  const [opened, { open: onFilterBtnOpen, close: onFilterBtnClose }] =
    useDisclosure(false);

  //filers
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [mobileNumber, setMobileNumber] = useState("");
  const [email, setEmail] = useState("");
  const [tableHeight, setTableHeight] = useState(500);
  //filers

  /** [User].[uspGetAllUsers] @pSortColumn values */
  const userSortColumnMap = {
    firstName: "FirstName",
    lastName: "LastName",
    email: "Email",
    phoneNo: "PhoneNo",
    mobileNo: "MobileNo",
    joiningDate: "JoiningDate",
    gender: "Gender",
    birthDate: "BirthDate",
    status: "Status",
  };

  const [sortStatus, setSortStatus] = useState({
    columnAccessor: "firstName",
    direction: "asc",
  });

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

  const navigate = useNavigate();
  const { franchiseName } = useParams();

  const pageSize = 25;
  const tableColumns = [
    {
      accessor: "index",
      title: "Sr No",
      textAlignment: "left",
      render: (record) => clients.indexOf(record) + 1,
      noWrap: true,
    },
    {
      accessor: "userNo",
      title: "User No",
      textAlignment: "left",
      noWrap: true,
    },
    {
      accessor: "firstName",
      title: "First Name",
      textAlignment: "left",
      noWrap: true,
      sortable: true,
      render: (record) => (
        <TruncatedTooltipText value={record.firstName} maxWidth={220} />
      ),
    },
    {
      accessor: "lastName",
      title: "Last Name",
      textAlignment: "left",
      sortable: true,
      render: (record) => (
        <TruncatedTooltipText value={record.lastName} maxWidth={160} />
      ),
    },
    {
      accessor: "alias",
      title: "Alias",
      textAlignment: "left",
      render: (record) => (
        <TruncatedTooltipText value={record.alias} maxWidth={140} />
      ),
    },
    {
      accessor: "phoneNo",
      title: "Phone Number",
      textAlignment: "left",
      sortable: true,
      render: (record) => (
        <TruncatedTooltipText value={record.phoneNo} maxWidth={150} />
      ),
    },
    {
      accessor: "mobileNo",
      title: "Mobile Number",
      textAlignment: "left",
      sortable: true,
      render: (record) => (
        <TruncatedTooltipText value={record.mobileNo} maxWidth={150} />
      ),
    },
    /*{
      accessor: "passportNo",
      title: "Passport Number",
      textAlignment: "left",
    },*/
    {
      accessor: "email",
      title: "Email",
      textAlignment: "left",
      sortable: true,
      render: (record) => (
        <TruncatedTooltipText value={record.email} maxWidth={220} />
      ),
    },
    {
      accessor: "joiningDate",
      title: "DOJ",
      textAlignment: "left",
      render: (record) => new Date(record.joiningDate).toLocaleDateString('en-US'),
      sortable: true,
    },
    {
      accessor: "gender",
      title: "Gender",
      textAlignment: "left",
      sortable: true,
    },
    {
      accessor: "birthDate",
      title: "DOB",
      textAlignment: "left",
      render: (record) => new Date(record.birthDate).toLocaleDateString('en-US'),
      sortable: true,
    },
    /*{
      accessor: "ethnicity",
      title: "Ethnicity",
      textAlignment: "left",
    },
    {
      accessor: "nationality",
      title: "Nationality",
      textAlignment: "left",
    },
    {
      accessor: "identityNo",
      title: "Identity Number",
      textAlignment: "left",
    },
    {
      accessor: "notes",
      title: "Notes",
      textAlignment: "left",
    },*/
    {
      accessor: "status",
      title: "Status",
      textAlignment: "left",
      render: (record) => (
        <Badge
          className="app-status-badge"
          color={record.status === "Active" ? "teal" : "red"}
          variant="light"
        >
          {record.status === "Active" ? "Active" : "Inactive"}
        </Badge>
      ),
      sortable: true,
    },
    {
      accessor: "actions",
      title: "",
      textAlignment: "left",
    },
  ];

  useEffect(() => {
    if (franchiseId && !franchiseLoading) {
      getUsers();
    }
  }, [pageNumber, userTypeID, franchiseId, franchiseLoading, sortStatus.columnAccessor, sortStatus.direction]);

  const getUsers = async (filterOverrides = {}) => {
    if (!franchiseId) {
      console.error('Franchise ID not available');
      return;
    }

    const obj = {};

    obj.franchiseId = franchiseId; // Use franchise ID from context
    obj.firstName = filterOverrides.firstName ?? firstName;
    obj.lastName = filterOverrides.lastName ?? lastName;
    obj.phoneNumber = filterOverrides.phoneNumber ?? phoneNumber;
    obj.mobileNumber = filterOverrides.mobileNumber ?? mobileNumber;
    obj.email = filterOverrides.email ?? email;
    obj.userType = userTypeID;
    obj.pageNumber = filterOverrides.pageNumber ?? pageNumber;
    obj.pageSize = pageSize;
    obj.sortColumn =
      userSortColumnMap[sortStatus.columnAccessor] ?? "FirstName";
    obj.sortType = sortStatus.direction === "asc" ? "ASC" : "DESC";

    setIsLoading(true);
    try {
      const response = await profileService.getUsers(obj);
      console.log('Users Response:', response); // Debug log
      
      setClients(response?.response || []);
      setTotalRecords(response?.totalRecords || 0);
    } catch (error) {
      console.error('Error fetching users:', error);
      setClients([]);
      setTotalRecords(0);
    } finally {
      setIsLoading(false);
    }
  };

  const handlePagination = (pageNumber) => {
    setPageNumber(pageNumber);
  };

  const handleSortStatusChange = (next) => {
    setSortStatus(next);
    setPageNumber(1);
  };

  const handleFilter = async () => {
    onFilterBtnClose();
    if (pageNumber !== 1) {
      setPageNumber(1);
      return;
    }
    await getUsers({ pageNumber: 1 });
  };

  const handleReset = async () => {
    setFirstName("");
    setLastName("");
    setPhoneNumber("");
    setMobileNumber("");
    setEmail("");
    onFilterBtnClose();

    if (pageNumber !== 1) {
      setPageNumber(1);
      return;
    }

    await getUsers({
      firstName: "",
      lastName: "",
      phoneNumber: "",
      mobileNumber: "",
      email: "",
      pageNumber: 1,
    });
  };

  const handleProfileDetail = (selectedRow) => {
    let userID = selectedRow
      ? selectedRow.userId
      : "00000000-0000-0000-0000-000000000000";

    navigate(`/franchises/${franchiseName}/profile/${userID}/${userTypeID}`);
  };

  return (
    <>
      <AppContainer
        title={
          Object.entries(UserType).find(
            ([key, value]) => value == userTypeID
          )?.[0]
        }
        button={
          <Group spacing="xs">
            <Button variant="light" onClick={onFilterBtnOpen}>
              Filter
            </Button>
            <Button onClick={() => handleProfileDetail(null)}>
              Create
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
          records={clients}
          sortStatus={sortStatus}
          onSortStatusChange={handleSortStatusChange}
          noRecordsText="No records to show"

          customRowAttributes={(record) => ({
            onDoubleClick: (e) => {
              handleProfileDetail(record);
            },
          })}
          totalRecords={totalRecords}
          recordsPerPage={pageSize}
          page={pageNumber}
          onPageChange={(p) => handlePagination(p)}
          paginationSize="sm"
        />
        {/* <AppTable
          thead={tableColumns}
          currentPage={pageNumber}
          pageSize={pageSize}
          totalRecords={totalRecords}
          onPagination={handlePagination}
          onFilterBtn={onFilterBtnOpen}
        >
          {clients.map((row, index) => (
            <tr key={index} onClick={() => handleProfileDetail(row)}>
              <td>
                {helperFunctions.getRowNumber(pageSize, pageNumber, index)}
              </td>
              <td>{row.userNo}</td>
              <td>{row.firstName}</td>
              <td>{row.lastName}</td>
              <td>{row.alias}</td>
              <td>{row.phoneNo}</td>
              <td>{row.mobileNo}</td>
              <td>{row.passportNo}</td>
              <td>{row.email}</td>
              <td>{row.joiningDate}</td>
              <td>{row.gender}</td>
              <td>{row.birthDate}</td>
              <td>{row.ethnicity}</td>
              <td>{row.nationality}</td>
              <td>{row.identityNo}</td>
              <td>{row.notes}</td>
              <td>{row.status}</td>
              <td></td>
            </tr>
          ))}
        </AppTable> */}
      </AppContainer>

      <AppDrawer
        opened={opened}
        close={onFilterBtnClose}
        onFilter={handleFilter}
        onReset={handleReset}
        title="Profile filters"
        description="Refine the profile list by name or contact information."
      >
        <form>
          <TextInput
            label="First Name"
            placeholder="john"
            size="md"
            mt="md"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
          />
          <TextInput
            label="Last Name"
            placeholder="Doe"
            size="md"
            mt="md"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
          />
          <TextInput
            label="Phone Number"
            placeholder="0001234567"
            size="md"
            mt="md"
            value={phoneNumber}
            onChange={(e) => setPhoneNumber(e.target.value)}
          />
          <TextInput
            label="Mobile Number"
            placeholder="0001234567"
            size="md"
            mt="md"
            value={mobileNumber}
            onChange={(e) => setMobileNumber(e.target.value)}
          />
          <TextInput
            label="Email"
            placeholder="example@gmail.com"
            size="md"
            mt="md"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </form>
      </AppDrawer>
    </>
  );
};

export default Users;
