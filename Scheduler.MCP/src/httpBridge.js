import http from "node:http";
import { configDiagnostics } from "./config.js";

const writeJson = (res, statusCode, payload) => {
  res.writeHead(statusCode, {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": "*",
    "Access-Control-Allow-Headers":
      "Content-Type, Authorization, X-MCP-Organization-Id, X-MCP-Franchise-Id, X-MCP-User-Id",
    "Access-Control-Allow-Methods": "GET,POST,OPTIONS",
  });
  res.end(JSON.stringify(payload));
};

const readJsonBody = async (req) =>
  new Promise((resolve, reject) => {
    let body = "";

    req.on("data", (chunk) => {
      body += chunk.toString();
    });

    req.on("end", () => {
      if (!body) {
        resolve({});
        return;
      }

      try {
        resolve(JSON.parse(body));
      } catch {
        reject(new Error("Invalid JSON body"));
      }
    });

    req.on("error", reject);
  });

const getRequestContext = (req) => {
  const authorization = req.headers.authorization || "";
  const apiToken = authorization.startsWith("Bearer ")
    ? authorization.slice("Bearer ".length).trim()
    : "";

  return {
    apiToken,
    organizationId: `${req.headers["x-mcp-organization-id"] || ""}`,
    franchiseId: `${req.headers["x-mcp-franchise-id"] || ""}`,
    userId: `${req.headers["x-mcp-user-id"] || ""}`,
  };
};

export const startHttpBridge = ({ port, toolDefinitions, toolHandlers }) => {
  const server = http.createServer(async (req, res) => {
    try {
      const requestContext = getRequestContext(req);

      if (req.method === "OPTIONS") {
        writeJson(res, 200, { ok: true });
        return;
      }

      if (req.method === "GET" && req.url === "/health") {
        writeJson(res, 200, { ok: true, service: "scheduler-mcp-bridge" });
        return;
      }

      if (req.method === "GET" && req.url === "/config-check") {
        writeJson(res, 200, configDiagnostics);
        return;
      }

      if (req.method === "GET" && req.url === "/tools") {
        writeJson(res, 200, { tools: toolDefinitions });
        return;
      }

      if (req.method === "POST" && req.url === "/tools/call") {
        const body = await readJsonBody(req);
        const toolName = body?.name;
        const args = body?.arguments || {};
        const handler = toolHandlers[toolName];

        if (!handler) {
          writeJson(res, 404, { error: `Unknown tool: ${toolName}` });
          return;
        }

        const result = await handler(args, requestContext);
        writeJson(res, 200, result);
        return;
      }

      if (req.method === "POST" && req.url === "/chat") {
        const body = await readJsonBody(req);
        const chatHandler = toolHandlers.__chat__;
        const result = await chatHandler(body || {}, requestContext);
        writeJson(res, 200, result);
        return;
      }

      writeJson(res, 404, { error: "Not found" });
    } catch (error) {
      writeJson(res, 500, { error: error.message || "Bridge error" });
    }
  });

  server.listen(port, "0.0.0.0", () => {
    console.error(`MCP HTTP bridge listening on http://0.0.0.0:${port}`);
  });
  return server;
};
