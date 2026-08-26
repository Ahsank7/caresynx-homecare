import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const rootDir = path.resolve(__dirname, "..");
const envFilePath = path.join(rootDir, ".env");

const parseEnvFile = (filePath) => {
  if (!fs.existsSync(filePath)) {
    return {};
  }

  const contents = fs.readFileSync(filePath, "utf8");

  return contents.split(/\r?\n/).reduce((accumulator, line) => {
    const trimmedLine = line.trim();

    if (!trimmedLine || trimmedLine.startsWith("#")) {
      return accumulator;
    }

    const separatorIndex = trimmedLine.indexOf("=");

    if (separatorIndex === -1) {
      return accumulator;
    }

    const key = trimmedLine.slice(0, separatorIndex).trim();
    const value = trimmedLine.slice(separatorIndex + 1).trim();

    if (key && (process.env[key] === undefined || process.env[key] === "")) {
      accumulator[key] = value;
    }

    return accumulator;
  }, {});
};

const fileEnv = parseEnvFile(envFilePath);

Object.entries(fileEnv).forEach(([key, value]) => {
  process.env[key] = value;
});

const required = (name, fallback = "") => {
  const value = process.env[name] ?? fallback;

  if (!value) {
    throw new Error(`Missing required environment variable: ${name}`);
  }

  return value;
};

export const config = {
  apiBaseUrl: required("MCP_API_BASE_URL", "https://localhost:7094/api"),
  apiToken: (process.env.MCP_API_TOKEN || "").trim(),
  openaiApiKey: (process.env.OPENAI_API_KEY || "").trim(),
  openaiModel: (process.env.MCP_OPENAI_MODEL || "gpt-5.4-mini").trim(),
  openaiReasoningEffort:
    (process.env.MCP_OPENAI_REASONING_EFFORT || "low").trim(),
  defaultOrganizationId: (process.env.MCP_DEFAULT_ORGANIZATION_ID || "").trim(),
  defaultFranchiseId: (process.env.MCP_DEFAULT_FRANCHISE_ID || "").trim(),
  actingUserId: (process.env.MCP_ACTING_USER_ID || "").trim(),
  writeEnabled: `${process.env.MCP_WRITE_ENABLED || "false"}`.toLowerCase() === "true",
  allowInsecureLocalHttps:
    `${process.env.MCP_ALLOW_INSECURE_LOCAL_HTTPS || "true"}`.toLowerCase() ===
    "true",
  httpPort: process.env.MCP_HTTP_PORT
    ? Number(process.env.MCP_HTTP_PORT)
    : 8787,
};

export const configDiagnostics = {
  envFilePath,
  envFileExists: fs.existsSync(envFilePath),
  apiBaseUrl: config.apiBaseUrl,
  httpPort: config.httpPort,
  writeEnabled: config.writeEnabled,
  allowInsecureLocalHttps: config.allowInsecureLocalHttps,
  openaiConfigured: Boolean(config.openaiApiKey),
  openaiKeyLength: config.openaiApiKey.length,
  openaiKeyPrefix: config.openaiApiKey
    ? `${config.openaiApiKey.slice(0, 7)}...`
    : "",
  openaiModel: config.openaiModel,
  openaiReasoningEffort: config.openaiReasoningEffort,
  hasFallbackApiToken: Boolean(config.apiToken),
  defaultOrganizationIdConfigured: Boolean(config.defaultOrganizationId),
  defaultFranchiseIdConfigured: Boolean(config.defaultFranchiseId),
  actingUserIdConfigured: Boolean(config.actingUserId),
};

if (config.allowInsecureLocalHttps) {
  process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
}
