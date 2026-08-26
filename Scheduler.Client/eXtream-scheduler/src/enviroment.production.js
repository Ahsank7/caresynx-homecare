const enviroment = {
    // Production environment
    baseURL: "http://178.105.20.159:5000/api/",
    // HTTPS origin for Scheduler.MCP httpBridge (nginx or ALB in front of EC2:8787). Override via REACT_APP_MCP_BRIDGE_URL at build time.
    mcpBridgeURL: process.env.REACT_APP_MCP_BRIDGE_URL || "https://mcp.caresynx.com",
};

export default enviroment;
