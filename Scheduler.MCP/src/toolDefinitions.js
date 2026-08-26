import { config } from "./config.js";

const guidSchema = {
  type: "string",
  description: "GUID value",
};

export const toolDefinitions = [
  {
    name: "users.search",
    description:
      "Search users by franchise and optional filters such as name, email, phone, user number, and user type.",
    inputSchema: {
      type: "object",
      properties: {
        franchiseId: { ...guidSchema, description: "Franchise GUID" },
        userType: {
          type: "integer",
          description: "1 = client, 2 = service provider, 3 = staff",
        },
        firstName: { type: "string" },
        lastName: { type: "string" },
        email: { type: "string" },
        phoneNumber: { type: "string" },
        mobileNumber: { type: "string" },
        userNo: { type: "string" },
        pageNumber: { type: "integer", default: 1 },
        pageSize: { type: "integer", default: 25 },
        sortColumn: { type: "string", default: "FirstName" },
        sortType: { type: "string", enum: ["ASC", "DESC"], default: "ASC" },
      },
      required: [],
    },
  },
  {
    name: "users.get_profile",
    description: "Get the full profile details for a user by GUID.",
    inputSchema: {
      type: "object",
      properties: {
        userId: guidSchema,
      },
      required: ["userId"],
    },
  },
  {
    name: "users.create",
    description:
      "Create a new user profile through the existing Users/SaveUpdate API. Use only after admin confirmation.",
    inputSchema: {
      type: "object",
      properties: {
        confirm: {
          type: "boolean",
          description: "Must be true before write actions are allowed.",
        },
        userType: {
          type: "integer",
          description: "1 = client, 2 = service provider, 3 = staff",
        },
        franchiseId: {
          ...guidSchema,
          description: "Franchise GUID. Defaults to MCP_DEFAULT_FRANCHISE_ID if omitted.",
        },
        firstName: { type: "string" },
        surName: { type: "string" },
        lastName: { type: "string" },
        alias: { type: "string" },
        email: { type: "string" },
        phoneNo: { type: "string" },
        mobileNo: { type: "string" },
        birthDate: { type: "string", description: "YYYY-MM-DD" },
        joiningDate: { type: "string", description: "YYYY-MM-DD" },
        genderId: { type: "integer" },
        titleId: { type: "integer" },
        ethnicityId: { type: "integer" },
        nationalityID: { type: "integer" },
        maritalStatusId: { type: "integer" },
        identityNo: { type: "string" },
        passportNo: { type: "string" },
        notes: { type: "string" },
        addressLine1: { type: "string" },
        addressLine2: { type: "string" },
        addressLine3: { type: "string" },
        countyId: { type: "integer" },
        stateId: { type: "integer" },
        countryId: { type: "integer" },
        latitude: { type: "number" },
        longitude: { type: "number" },
      },
      required: [
        "confirm",
        "userType",
        "firstName",
        "lastName",
        "email",
        "phoneNo",
        "mobileNo",
        "birthDate",
        "joiningDate",
      ],
    },
  },
  {
    name: "users.update_profile",
    description:
      "Update an existing user profile by merging the provided fields with the current profile. Use only after admin confirmation.",
    inputSchema: {
      type: "object",
      properties: {
        confirm: {
          type: "boolean",
          description: "Must be true before write actions are allowed.",
        },
        userId: guidSchema,
        firstName: { type: "string" },
        surName: { type: "string" },
        lastName: { type: "string" },
        alias: { type: "string" },
        email: { type: "string" },
        phoneNo: { type: "string" },
        mobileNo: { type: "string" },
        birthDate: { type: "string", description: "YYYY-MM-DD" },
        joiningDate: { type: "string", description: "YYYY-MM-DD" },
        genderId: { type: "integer" },
        titleId: { type: "integer" },
        ethnicityId: { type: "integer" },
        nationalityID: { type: "integer" },
        maritalStatusId: { type: "integer" },
        identityNo: { type: "string" },
        passportNo: { type: "string" },
        notes: { type: "string" },
        addressLine1: { type: "string" },
        addressLine2: { type: "string" },
        addressLine3: { type: "string" },
        countyId: { type: "integer" },
        stateId: { type: "integer" },
        countryId: { type: "integer" },
        latitude: { type: "number" },
        longitude: { type: "number" },
      },
      required: ["confirm", "userId"],
    },
  },
  {
    name: "address.create",
    description:
      "Create a new address record for a user. Use only after admin confirmation.",
    inputSchema: {
      type: "object",
      properties: {
        confirm: {
          type: "boolean",
          description: "Must be true before write actions are allowed.",
        },
        userId: guidSchema,
        addressLine1: { type: "string" },
        addressLine2: { type: "string" },
        addressLine3: { type: "string" },
        addressTypeId: { type: "integer" },
        countyId: { type: "integer" },
        stateId: { type: "integer" },
        countryId: { type: "integer" },
        latitude: { type: "number" },
        longitude: { type: "number" },
      },
      required: [
        "confirm",
        "userId",
        "addressLine1",
        "addressTypeId",
        "countyId",
        "stateId",
        "countryId"
      ],
    },
  },
  {
    name: "contact.create",
    description:
      "Create a new contact record for a user. Use only after admin confirmation.",
    inputSchema: {
      type: "object",
      properties: {
        confirm: {
          type: "boolean",
          description: "Must be true before write actions are allowed.",
        },
        userId: guidSchema,
        franchiseId: {
          ...guidSchema,
          description: "Franchise GUID. Defaults to MCP_DEFAULT_FRANCHISE_ID if omitted.",
        },
        firstName: { type: "string" },
        surName: { type: "string" },
        lastName: { type: "string" },
        alias: { type: "string" },
        phoneNo: { type: "string" },
        mobileNo: { type: "string" },
        email: { type: "string" },
        identityNo: { type: "string" },
        birthDate: { type: "string", description: "YYYY-MM-DD" },
        notes: { type: "string" },
        addressLine1: { type: "string" },
        addressLine2: { type: "string" },
        addressLine3: { type: "string" },
        countyId: { type: "integer" },
        stateId: { type: "integer" },
        countryId: { type: "integer" },
        latitude: { type: "number" },
        longitude: { type: "number" },
        titleId: { type: "integer" },
        genderId: { type: "integer" },
        contactTypeId: { type: "integer" },
      },
      required: [
        "confirm",
        "userId",
        "firstName",
        "lastName",
        "mobileNo",
        "email",
        "contactTypeId"
      ],
    },
  },
  {
    name: "availability.list",
    description:
      "Get availability records for a user. Useful for checking service provider availability windows.",
    inputSchema: {
      type: "object",
      properties: {
        userId: guidSchema,
        pageNumber: { type: "integer", default: 1 },
        pageSize: { type: "integer", default: 25 },
      },
      required: ["userId"],
    },
  },
  {
    name: "schedule.create",
    description:
      "Create an appointment or schedule entry. Use only after admin confirmation.",
    inputSchema: {
      type: "object",
      properties: {
        confirm: {
          type: "boolean",
          description: "Must be true before write actions are allowed.",
        },
        organizationId: {
          ...guidSchema,
          description:
            "Organization GUID. Defaults to MCP_DEFAULT_ORGANIZATION_ID if omitted.",
        },
        createdBy: {
          ...guidSchema,
          description:
            "Admin/service user GUID. Defaults to MCP_ACTING_USER_ID if omitted.",
        },
        clientId: guidSchema,
        csvServiceProviderIds: {
          type: "string",
          description: "Comma-separated service provider GUIDs",
        },
        serviceType: { type: "integer" },
        csvServiceIds: {
          type: "string",
          description: "Comma-separated service IDs",
        },
        scheduleDescription: { type: "string" },
        startTime: { type: "string", description: "ISO date-time start" },
        endTime: { type: "string", description: "ISO date-time end" },
        recurrencePattern: { type: "integer", default: 1 },
        recurrenceInterval: { type: "integer", default: 1 },
        recurrenceDaysOfWeek: { type: "string" },
        recurrenceDayOfMonth: { type: "string" },
        recurrenceDayOfYear: { type: "string" },
      },
      required: [
        "confirm",
        "organizationId",
        "clientId",
        "csvServiceProviderIds",
        "serviceType",
        "csvServiceIds",
        "scheduleDescription",
        "startTime",
        "endTime",
      ],
    },
  },
  {
    name: "schedule.get_client_tasks",
    description: "Get tasks for a specific client.",
    inputSchema: {
      type: "object",
      properties: {
        organizationId: guidSchema,
        clientId: guidSchema,
        startDate: { type: "string", description: "YYYY-MM-DD" },
        endDate: { type: "string", description: "YYYY-MM-DD" },
        statusIds: { type: "string", description: "Comma-separated task status IDs" },
      },
      required: ["clientId", "startDate", "endDate"],
    },
  },
  {
    name: "schedule.get_service_provider_tasks",
    description: "Get tasks for a specific service provider.",
    inputSchema: {
      type: "object",
      properties: {
        organizationId: guidSchema,
        serviceProviderId: guidSchema,
        startDate: { type: "string", description: "YYYY-MM-DD" },
        endDate: { type: "string", description: "YYYY-MM-DD" },
        statusIds: { type: "string", description: "Comma-separated task status IDs" },
      },
      required: ["serviceProviderId", "startDate", "endDate"],
    },
  },
  {
    name: "planboard.tasks",
    description:
      "Get planboard tasks using the same task search API as the main admin planboard screen.",
    inputSchema: {
      type: "object",
      properties: {
        franchiseId: {
          ...guidSchema,
          description: `Franchise GUID. Defaults to ${config.defaultFranchiseId || "MCP_DEFAULT_FRANCHISE_ID"}.`,
        },
        taskId: { type: "string" },
        clientUserNo: { type: "string" },
        clientName: { type: "string" },
        serviceProviderUserNo: { type: "string" },
        serviceProviderName: { type: "string" },
        startDate: { type: "string", description: "YYYY-MM-DD" },
        endDate: { type: "string", description: "YYYY-MM-DD" },
        taskStatusIds: {
          type: "array",
          items: { type: "integer" },
        },
        pageNumber: { type: "integer", default: 1 },
        pageSize: { type: "integer", default: 25 },
        sortColumn: { type: "string", default: "TaskId" },
        sortType: { type: "string", enum: ["ASC", "DESC"], default: "DESC" },
      },
      required: [],
    },
  },
  {
    name: "to_confirm.tasks",
    description:
      "Get tasks pending confirm/billing workflows through the same API used by the To Confirm screen.",
    inputSchema: {
      type: "object",
      properties: {
        franchiseId: {
          ...guidSchema,
          description: `Franchise GUID. Defaults to ${config.defaultFranchiseId || "MCP_DEFAULT_FRANCHISE_ID"}.`,
        },
        taskId: { type: "string" },
        clientUserNo: { type: "string" },
        clientName: { type: "string" },
        serviceProviderUserNo: { type: "string" },
        serviceProviderName: { type: "string" },
        startDate: { type: "string", description: "YYYY-MM-DD" },
        endDate: { type: "string", description: "YYYY-MM-DD" },
        pageNumber: { type: "integer", default: 1 },
        pageSize: { type: "integer", default: 25 },
      },
      required: [],
    },
  },
];
