import { config } from "./config.js";

const buildUrl = (path) => {
  const base = config.apiBaseUrl.replace(/\/$/, "");
  const nextPath = path.replace(/^\//, "");
  return `${base}/${nextPath}`;
};

const unwrapApiResponse = async (response) => {
  const payload = await response.json();

  const success =
    payload?.isSuccess === true ||
    (typeof payload?.status === "number" &&
      payload.status >= 200 &&
      payload.status < 300);

  if (!response.ok || !success) {
    const errorDetails = [];

    if (payload?.message) {
      errorDetails.push(payload.message);
    }

    if (Array.isArray(payload?.errors) && payload.errors.length > 0) {
      errorDetails.push(payload.errors.join(", "));
    }

    if (payload?.errors && !Array.isArray(payload.errors)) {
      errorDetails.push(JSON.stringify(payload.errors));
    }

    const message =
      errorDetails.join(" | ") ||
      `API request failed with status ${response.status}`;
    throw new Error(message);
  }

  return payload?.data;
};

const request = async (method, path, body, requestContext = {}) => {
  const apiToken = requestContext.apiToken || config.apiToken;
  const url = buildUrl(path);

  if (!apiToken) {
    throw new Error(
      "Missing API token. Provide Authorization from the frontend session or set MCP_API_TOKEN."
    );
  }

  let response;

  try {
    response = await fetch(url, {
      method,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${apiToken}`,
      },
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch (error) {
    const causeMessage =
      error?.cause?.message || error?.message || "Unknown fetch error";

    throw new Error(
      `Failed to call Scheduler API at ${url}. ${causeMessage}`
    );
  }

  return unwrapApiResponse(response);
};

export const apiClient = {
  getUserById(userId, requestContext) {
    const params = new URLSearchParams({ UserID: userId });
    return request("GET", `Users/Details?${params.toString()}`, undefined, requestContext);
  },

  searchUsers(payload, requestContext) {
    return request("POST", "Users/List", payload, requestContext);
  },

  createOrUpdateUser(payload, requestContext) {
    return request("POST", "Users/SaveUpdate", payload, requestContext);
  },

  createOrUpdateAddress(payload, requestContext) {
    return request("POST", "Address/SaveUpdateAddress", payload, requestContext);
  },

  createOrUpdateContact(payload, requestContext) {
    return request("POST", "Contact/SaveUpdate", payload, requestContext);
  },

  listAvailability(payload, requestContext) {
    return request("POST", "Availability/List", payload, requestContext);
  },

  createSchedule(payload, requestContext) {
    return request("POST", "Scheduler/CreateAppointment", payload, requestContext);
  },

  getClientTasks(payload, requestContext) {
    return request("POST", "Scheduler/GetClientTasks", payload, requestContext);
  },

  getServiceProviderTasks(payload, requestContext) {
    return request("POST", "Scheduler/GetServiceProviderTasks", payload, requestContext);
  },

  getPlanboardTasks(payload, requestContext) {
    return request("POST", "PlanBoard/ServicesTask", payload, requestContext);
  },

  getToConfirmTasks(payload, requestContext) {
    return request("POST", "ToConfirm/ServicesTask", payload, requestContext);
  },
};
