import enviroment from "enviroment";
import * as localStoreService from "./localStoreService";

const baseUrl = enviroment.mcpBridgeURL.replace(/\/$/, "");

const buildContextHeaders = () => {
  const token = localStoreService.getToken();
  const organizationId = localStoreService.getOrganizationID();
  const franchiseId = localStoreService.getFranchiseID();
  const userId = localStoreService.getUserID();

  return {
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...(organizationId ? { "X-MCP-Organization-Id": organizationId } : {}),
    ...(franchiseId ? { "X-MCP-Franchise-Id": franchiseId } : {}),
    ...(userId ? { "X-MCP-User-Id": userId } : {}),
  };
};

const handleBridgeResponse = async (response) => {
  const payload = await response.json();

  if (!response.ok) {
    throw new Error(payload?.error || "MCP bridge request failed");
  }

  return payload;
};

export const listTools = async () => {
  const response = await fetch(`${baseUrl}/tools`, {
    headers: buildContextHeaders(),
  });
  const payload = await handleBridgeResponse(response);
  return payload.tools || [];
};

export const health = async () => {
  const response = await fetch(`${baseUrl}/health`, {
    headers: buildContextHeaders(),
  });
  return handleBridgeResponse(response);
};

export const configCheck = async () => {
  const response = await fetch(`${baseUrl}/config-check`, {
    headers: buildContextHeaders(),
  });
  return handleBridgeResponse(response);
};

export const callTool = async (name, args) => {
  const response = await fetch(`${baseUrl}/tools/call`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...buildContextHeaders(),
    },
    body: JSON.stringify({
      name,
      arguments: args,
    }),
  });

  return handleBridgeResponse(response);
};

export const chat = async (message, history = []) => {
  const response = await fetch(`${baseUrl}/chat`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...buildContextHeaders(),
    },
    body: JSON.stringify({
      message,
      history,
    }),
  });

  return handleBridgeResponse(response);
};
