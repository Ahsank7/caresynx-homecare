import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
} from "@modelcontextprotocol/sdk/types.js";
import { toolDefinitions } from "./toolDefinitions.js";
import { toolHandlers } from "./toolHandlers.js";
import { config } from "./config.js";
import { startHttpBridge } from "./httpBridge.js";

const server = new Server(
  {
    name: "scheduler-mcp-server",
    version: "0.1.0",
  },
  {
    capabilities: {
      tools: {},
    },
  }
);

server.setRequestHandler(ListToolsRequestSchema, async () => {
  return {
    tools: toolDefinitions,
  };
});

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const toolName = request.params.name;
  const handler = toolHandlers[toolName];

  if (!handler) {
    throw new Error(`Unknown tool: ${toolName}`);
  }

  const args = request.params.arguments || {};
  return handler(args);
});

const transport = new StdioServerTransport();
if (config.httpPort > 0) {
  startHttpBridge({
    port: config.httpPort,
    toolDefinitions,
    toolHandlers,
  });
}
await server.connect(transport);
