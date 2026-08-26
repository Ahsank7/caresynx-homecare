const enviroment = {
    baseURL: process.env.REACT_APP_API_URL || "/api/",
    mcpBridgeURL: process.env.REACT_APP_MCP_BRIDGE_URL || "http://localhost:8787",
};

export default enviroment;
