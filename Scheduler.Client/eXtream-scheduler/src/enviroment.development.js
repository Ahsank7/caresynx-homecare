const enviroment = {
    // Development: point directly at local API (Swagger at https://localhost:7094/index.html)
    baseURL: process.env.REACT_APP_API_URL || "https://localhost:7094/api/",
    mcpBridgeURL: process.env.REACT_APP_MCP_BRIDGE_URL || "http://localhost:8787",
};

export default enviroment;
