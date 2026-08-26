import { apiClient } from "./apiClient.js";
import { config } from "./config.js";
import { runAdminChat } from "./openaiChat.js";

const asTextResult = (title, data) => ({
  content: [
    {
      type: "text",
      text: `${title}\n\n${JSON.stringify(data, null, 2)}`,
    },
  ],
});

const requireWritesEnabled = (confirm) => {
  if (!config.writeEnabled) {
    throw new Error(
      "Write tools are disabled. Set MCP_WRITE_ENABLED=true to allow create/update actions."
    );
  }

  if (confirm !== true) {
    throw new Error(
      "This is a write tool. Pass confirm=true only after the admin explicitly approves the action."
    );
  }
};

const normalizePagedResponse = (result) => {
  if (result && typeof result === "object" && "response" in result) {
    return result;
  }

  return {
    response: Array.isArray(result) ? result : result ? [result] : [],
    totalRecords: Array.isArray(result) ? result.length : result ? 1 : 0,
  };
};

const resolveContextValue = (argsValue, contextValue, configValue) =>
  argsValue || contextValue || configValue;

const requireIntegerField = (value, fieldName) => {
  if (!Number.isInteger(value)) {
    throw new Error(`${fieldName} is required and must be an integer.`);
  }
};

const getExistingUserPayload = async (userId, requestContext = {}) => {
  const user = await apiClient.getUserById(userId, requestContext);

  return {
    id: userId,
    userType: user.userType,
    franchiseId: user.franchiseId || config.defaultFranchiseId,
    firstName: user.firstName || "",
    surName: user.surName || "",
    lastName: user.lastName || "",
    alias: user.alias || "",
    userName: user.userName || "",
    phoneNo: user.phoneNo || "",
    mobileNo: user.mobileNo || "",
    email: user.email || "",
    passportNo: user.passportNo || "",
    identityNo: user.identityNo || "",
    ethnicityId: user.ethnicityId || 0,
    passwordHash: user.passwordHash || "",
    birthDate: user.birthDate ? `${user.birthDate}`.split("T")[0] : "",
    joiningDate: user.joiningDate ? `${user.joiningDate}`.split("T")[0] : "",
    notes: user.notes || "",
    userNo: user.userNo || "",
    addressLine1: user.addressLine1 || "",
    addressLine2: user.addressLine2 || "",
    addressLine3: user.addressLine3 || "",
    addressId: user.addressId || null,
    countyId: user.countyId || 0,
    maritalStatusId: user.maritalStatusId || 0,
    stateId: user.stateId || 0,
    countryId: user.countryId || 0,
    latitude: user.latitude || 0,
    longitude: user.longitude || 0,
    titleId: user.titleId || 0,
    genderId: user.genderId || 0,
    nationalityID: user.nationalityID || 0,
  };
};

export const toolHandlers = {
  async __chat__(args, requestContext = {}) {
    return runAdminChat({
      message: args.message || "",
      history: Array.isArray(args.history) ? args.history : [],
      requestContext,
    });
  },

  async "users.search"(args, requestContext = {}) {
    const basePayload = {
      franchiseId: resolveContextValue(
        args.franchiseId,
        requestContext.franchiseId,
        config.defaultFranchiseId
      ),
      firstName: args.firstName || "",
      lastName: args.lastName || "",
      email: args.email || "",
      phoneNumber: args.phoneNumber || "",
      mobileNumber: args.mobileNumber || "",
      userNo: args.userNo || "",
      pageNumber: args.pageNumber || 1,
      pageSize: args.pageSize || 25,
      sortColumn: args.sortColumn || "FirstName",
      sortType: args.sortType || "ASC",
    };

    if (!basePayload.franchiseId) {
      throw new Error("franchiseId is required either in tool input or MCP_DEFAULT_FRANCHISE_ID.");
    }

    if (args.userType !== undefined && args.userType !== null) {
      requireIntegerField(args.userType, "userType");

      const result = await apiClient.searchUsers(
        {
          ...basePayload,
          userType: args.userType,
        },
        requestContext
      );
      return asTextResult("User search results", normalizePagedResponse(result));
    }

    const userTypes = [1, 2, 3];
    const results = await Promise.all(
      userTypes.map((userType) =>
        apiClient.searchUsers(
          {
            ...basePayload,
            userType,
          },
          requestContext
        )
      )
    );

    const response = results.flatMap((result) =>
      Array.isArray(result?.response) ? result.response : []
    );
    const totalRecords = results.reduce(
      (sum, result) => sum + (result?.totalRecords || 0),
      0
    );

    return asTextResult("User search results", {
      response,
      totalRecords,
    });
  },

  async "users.get_profile"(args, requestContext = {}) {
    const result = await apiClient.getUserById(args.userId, requestContext);
    return asTextResult("User profile", result);
  },

  async "users.create"(args, requestContext = {}) {
    requireWritesEnabled(args.confirm);

    const payload = {
      id: null,
      userType: args.userType,
      franchiseId: resolveContextValue(
        args.franchiseId,
        requestContext.franchiseId,
        config.defaultFranchiseId
      ),
      firstName: args.firstName,
      surName: args.surName || "",
      lastName: args.lastName,
      alias: args.alias || "",
      userName: "",
      phoneNo: args.phoneNo,
      mobileNo: args.mobileNo,
      email: args.email,
      passportNo: args.passportNo || "",
      identityNo: args.identityNo || "",
      ethnicityId: args.ethnicityId || 0,
      passwordHash: "",
      birthDate: args.birthDate,
      joiningDate: args.joiningDate,
      notes: args.notes || "",
      userNo: "",
      addressLine1: args.addressLine1 || "",
      addressLine2: args.addressLine2 || "",
      addressLine3: args.addressLine3 || "",
      addressId: null,
      countyId: args.countyId || 0,
      maritalStatusId: args.maritalStatusId || 0,
      stateId: args.stateId || 0,
      countryId: args.countryId || 0,
      latitude: args.latitude || 0,
      longitude: args.longitude || 0,
      titleId: args.titleId || 0,
      genderId: args.genderId || 0,
      nationalityID: args.nationalityID || 0,
    };

    if (!payload.franchiseId) {
      throw new Error("franchiseId is required either in tool input or MCP_DEFAULT_FRANCHISE_ID.");
    }

    const result = await apiClient.createOrUpdateUser(payload, requestContext);
    return asTextResult("User created", {
      createdUserId: result,
      inputSummary: {
        firstName: payload.firstName,
        lastName: payload.lastName,
        email: payload.email,
        userType: payload.userType,
      },
    });
  },

  async "users.update_profile"(args, requestContext = {}) {
    requireWritesEnabled(args.confirm);

    const payload = {
      ...(await getExistingUserPayload(args.userId, requestContext)),
      ...(args.firstName !== undefined ? { firstName: args.firstName } : {}),
      ...(args.surName !== undefined ? { surName: args.surName } : {}),
      ...(args.lastName !== undefined ? { lastName: args.lastName } : {}),
      ...(args.alias !== undefined ? { alias: args.alias } : {}),
      ...(args.email !== undefined ? { email: args.email } : {}),
      ...(args.phoneNo !== undefined ? { phoneNo: args.phoneNo } : {}),
      ...(args.mobileNo !== undefined ? { mobileNo: args.mobileNo } : {}),
      ...(args.birthDate !== undefined ? { birthDate: args.birthDate } : {}),
      ...(args.joiningDate !== undefined ? { joiningDate: args.joiningDate } : {}),
      ...(args.genderId !== undefined ? { genderId: args.genderId } : {}),
      ...(args.titleId !== undefined ? { titleId: args.titleId } : {}),
      ...(args.ethnicityId !== undefined ? { ethnicityId: args.ethnicityId } : {}),
      ...(args.nationalityID !== undefined ? { nationalityID: args.nationalityID } : {}),
      ...(args.maritalStatusId !== undefined ? { maritalStatusId: args.maritalStatusId } : {}),
      ...(args.identityNo !== undefined ? { identityNo: args.identityNo } : {}),
      ...(args.passportNo !== undefined ? { passportNo: args.passportNo } : {}),
      ...(args.notes !== undefined ? { notes: args.notes } : {}),
      ...(args.addressLine1 !== undefined ? { addressLine1: args.addressLine1 } : {}),
      ...(args.addressLine2 !== undefined ? { addressLine2: args.addressLine2 } : {}),
      ...(args.addressLine3 !== undefined ? { addressLine3: args.addressLine3 } : {}),
      ...(args.countyId !== undefined ? { countyId: args.countyId } : {}),
      ...(args.stateId !== undefined ? { stateId: args.stateId } : {}),
      ...(args.countryId !== undefined ? { countryId: args.countryId } : {}),
      ...(args.latitude !== undefined ? { latitude: args.latitude } : {}),
      ...(args.longitude !== undefined ? { longitude: args.longitude } : {}),
    };

    const result = await apiClient.createOrUpdateUser(payload, requestContext);
    return asTextResult("User updated", {
      updatedUserId: result || args.userId,
      inputSummary: args,
    });
  },

  async "address.create"(args, requestContext = {}) {
    requireWritesEnabled(args.confirm);

    const result = await apiClient.createOrUpdateAddress({
      id: null,
      userId: args.userId,
      addressLine1: args.addressLine1,
      addressLine2: args.addressLine2 || "",
      addressLine3: args.addressLine3 || "",
      addressTypeId: args.addressTypeId,
      countyId: args.countyId,
      stateId: args.stateId,
      countryId: args.countryId,
      latitude: args.latitude || 0,
      longitude: args.longitude || 0,
    }, requestContext);

    return asTextResult("Address created", {
      addressId: result,
      inputSummary: args,
    });
  },

  async "contact.create"(args, requestContext = {}) {
    requireWritesEnabled(args.confirm);

    const franchiseId = resolveContextValue(
      args.franchiseId,
      requestContext.franchiseId,
      config.defaultFranchiseId
    );

    if (!franchiseId) {
      throw new Error("franchiseId is required either in tool input or MCP_DEFAULT_FRANCHISE_ID.");
    }

    const result = await apiClient.createOrUpdateContact({
      id: null,
      userId: args.userId,
      firstName: args.firstName,
      surName: args.surName || "",
      lastName: args.lastName,
      alias: args.alias || "",
      phoneNo: args.phoneNo || "",
      mobileNo: args.mobileNo,
      email: args.email,
      identityNo: args.identityNo || "",
      birthDate: args.birthDate || null,
      notes: args.notes || "",
      addressLine1: args.addressLine1 || "",
      addressLine2: args.addressLine2 || "",
      addressLine3: args.addressLine3 || "",
      countyId: args.countyId || 0,
      stateId: args.stateId || 0,
      countryId: args.countryId || 0,
      latitude: args.latitude || 0,
      longitude: args.longitude || 0,
      titleId: args.titleId || 0,
      genderId: args.genderId || 0,
      contactTypeId: args.contactTypeId,
      franchiseId,
    }, requestContext);

    return asTextResult("Contact created", {
      contactId: result,
      inputSummary: args,
    });
  },

  async "availability.list"(args, requestContext = {}) {
    const result = await apiClient.listAvailability({
      userId: args.userId,
      pageNumber: args.pageNumber || 1,
      pageSize: args.pageSize || 25,
    }, requestContext);

    return asTextResult("Availability list", normalizePagedResponse(result));
  },

  async "schedule.create"(args, requestContext = {}) {
    requireWritesEnabled(args.confirm);

    const payload = {
      organizationId: resolveContextValue(
        args.organizationId,
        requestContext.organizationId,
        config.defaultOrganizationId
      ),
      createdBy: resolveContextValue(
        args.createdBy,
        requestContext.userId,
        config.actingUserId
      ),
      clientId: args.clientId,
      csvServiceProviderIds: args.csvServiceProviderIds,
      serviceType: args.serviceType,
      csvServiceIds: args.csvServiceIds,
      scheduleDescription: args.scheduleDescription,
      startTime: args.startTime,
      endTime: args.endTime,
      recurrencePattern: args.recurrencePattern || 1,
      recurrenceInterval: args.recurrenceInterval || 1,
      recurrenceDaysOfWeek: args.recurrenceDaysOfWeek || "",
      recurrenceDayOfMonth: args.recurrenceDayOfMonth || "",
      recurrenceDayOfYear: args.recurrenceDayOfYear || "",
    };

    if (!payload.organizationId) {
      throw new Error(
        "organizationId is required either in tool input or MCP_DEFAULT_ORGANIZATION_ID."
      );
    }

    if (!payload.createdBy) {
      throw new Error(
        "createdBy is required either in tool input or MCP_ACTING_USER_ID."
      );
    }

    const result = await apiClient.createSchedule(payload, requestContext);
    return asTextResult("Schedule created", {
      scheduleId: result,
      inputSummary: payload,
    });
  },

  async "schedule.get_client_tasks"(args, requestContext = {}) {
    const result = await apiClient.getClientTasks({
      organizationId: resolveContextValue(
        args.organizationId,
        requestContext.organizationId,
        config.defaultOrganizationId
      ),
      clientId: args.clientId,
      startDate: args.startDate,
      endDate: args.endDate,
      statusIds: args.statusIds || "",
    }, requestContext);

    return asTextResult("Client schedule tasks", result);
  },

  async "schedule.get_service_provider_tasks"(args, requestContext = {}) {
    const result = await apiClient.getServiceProviderTasks({
      organizationId: resolveContextValue(
        args.organizationId,
        requestContext.organizationId,
        config.defaultOrganizationId
      ),
      serviceProviderId: args.serviceProviderId,
      startDate: args.startDate,
      endDate: args.endDate,
      statusIds: args.statusIds || "",
    }, requestContext);

    return asTextResult("Service provider schedule tasks", result);
  },

  async "planboard.tasks"(args, requestContext = {}) {
    const result = await apiClient.getPlanboardTasks({
      franchiseId: resolveContextValue(
        args.franchiseId,
        requestContext.franchiseId,
        config.defaultFranchiseId
      ),
      taskStatusIds: Array.isArray(args.taskStatusIds)
        ? args.taskStatusIds.join(",")
        : "",
      taskId: args.taskId || "",
      startDate: args.startDate || null,
      endDate: args.endDate || null,
      clientUserNo: args.clientUserNo || "",
      clientName: args.clientName || "",
      serviceProviderUserNo: args.serviceProviderUserNo || "",
      serviceProviderName: args.serviceProviderName || "",
      pageNumber: args.pageNumber || 1,
      pageSize: args.pageSize || 25,
      sortColumn: args.sortColumn || "TaskId",
      sortType: args.sortType || "DESC",
    }, requestContext);

    return asTextResult("Planboard tasks", normalizePagedResponse(result));
  },

  async "to_confirm.tasks"(args, requestContext = {}) {
    const result = await apiClient.getToConfirmTasks({
      franchiseId: resolveContextValue(
        args.franchiseId,
        requestContext.franchiseId,
        config.defaultFranchiseId
      ),
      taskId: args.taskId || "",
      startDate: args.startDate || null,
      endDate: args.endDate || null,
      clientUserNo: args.clientUserNo || "",
      clientName: args.clientName || "",
      serviceProviderUserNo: args.serviceProviderUserNo || "",
      serviceProviderName: args.serviceProviderName || "",
      pageNumber: args.pageNumber || 1,
      pageSize: args.pageSize || 25,
    }, requestContext);

    return asTextResult("To confirm tasks", normalizePagedResponse(result));
  },
};
