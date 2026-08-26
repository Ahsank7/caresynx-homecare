import { config } from "./config.js";
import { toolDefinitions } from "./toolDefinitions.js";
import { toolHandlers } from "./toolHandlers.js";

const READ_ONLY_TOOL_NAMES = new Set(
  toolDefinitions
    .map((tool) => tool.name)
    .filter(
      (name) =>
        ![
          "users.create",
          "users.update_profile",
          "address.create",
          "contact.create",
          "schedule.create",
        ].includes(name)
    )
);

const toOpenAIToolName = (toolName) => toolName.replace(/[^a-zA-Z0-9_-]/g, "_");

const fromOpenAIToolName = (toolName, toolNameMap) => toolNameMap.get(toolName) || toolName;

const SYSTEM_INSTRUCTIONS = `You are an admin copilot for the CaresynX Scheduler platform.

Use the available tools when they help answer the admin's question with factual data from the live system.

Rules:
- Prefer tool use over guessing.
- Use only the read-only tools provided.
- If a request would require a write action, explain that the admin should use the Tool Runner for now.
- When giving summaries, keep them concise but include concrete numbers, names, dates, and emails when relevant.
- If the user asks for counts, ages, or comparisons, compute them from the tool results before answering.
- If the data returned is incomplete, say what is missing instead of guessing.
`;

const buildTools = () => {
  const toolNameMap = new Map();

  const tools = toolDefinitions
    .filter((tool) => READ_ONLY_TOOL_NAMES.has(tool.name))
    .map((tool) => {
      const openAiToolName = toOpenAIToolName(tool.name);
      toolNameMap.set(openAiToolName, tool.name);

      return {
        type: "function",
        name: openAiToolName,
        description: tool.description,
        strict: false,
        parameters: {
          ...tool.inputSchema,
          additionalProperties: false,
        },
      };
    });

  return { tools, toolNameMap };
};

const buildInputMessages = (history = [], message) => {
  const normalizedHistory = history
    .filter((item) => item?.role && item?.text)
    .map((item) => ({
      role: item.role,
      content: item.text,
    }));

  return [...normalizedHistory, { role: "user", content: message }];
};

const callOpenAI = async (payload) => {
  if (!config.openaiApiKey) {
    throw new Error(
      "Missing OPENAI_API_KEY. Add it to Scheduler.MCP/.env to enable AI chat mode."
    );
  }

  const response = await fetch("https://api.openai.com/v1/responses", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${config.openaiApiKey}`,
    },
    body: JSON.stringify(payload),
  });

  const result = await response.json();

  if (!response.ok) {
    const message =
      result?.error?.message ||
      result?.message ||
      `OpenAI request failed with status ${response.status}`;
    throw new Error(message);
  }

  return result;
};

const extractText = (response) => {
  if (response?.output_text) {
    return response.output_text;
  }

  const outputMessages = Array.isArray(response?.output)
    ? response.output.filter((item) => item.type === "message")
    : [];

  const texts = outputMessages
    .flatMap((item) => item.content || [])
    .filter((item) => item.type === "output_text")
    .map((item) => item.text);

  return texts.join("\n\n").trim();
};

export const runAdminChat = async ({ message, history = [], requestContext = {} }) => {
  const { tools, toolNameMap } = buildTools();
  const input = buildInputMessages(history, message);
  const usedTools = [];

  for (let attempt = 0; attempt < 8; attempt += 1) {
    const response = await callOpenAI({
      model: config.openaiModel,
      instructions: SYSTEM_INSTRUCTIONS,
      input,
      tools,
      reasoning: {
        effort: config.openaiReasoningEffort,
      },
    });

    input.push(...(response.output || []));

    const toolCalls = (response.output || []).filter(
      (item) => item.type === "function_call"
    );

    if (toolCalls.length === 0) {
      return {
        answer:
          extractText(response) ||
          "I could not produce a final answer from the available data.",
        usedTools,
      };
    }

    for (const toolCall of toolCalls) {
      const args = JSON.parse(toolCall.arguments || "{}");
      const internalToolName = fromOpenAIToolName(toolCall.name, toolNameMap);
      const handler = toolHandlers[internalToolName];

      if (!handler || !READ_ONLY_TOOL_NAMES.has(internalToolName)) {
        input.push({
          type: "function_call_output",
          call_id: toolCall.call_id,
          output: JSON.stringify({
            error:
              "That tool is not available in chat mode. Use the Tool Runner for explicit write actions.",
          }),
        });
        continue;
      }

      const toolResult = await handler(args, requestContext);
      usedTools.push({
        name: internalToolName,
        arguments: args,
      });

      input.push({
        type: "function_call_output",
        call_id: toolCall.call_id,
        output: JSON.stringify(toolResult),
      });
    }
  }

  throw new Error(
    "The AI assistant exceeded the maximum tool-call loop while answering this request."
  );
};
