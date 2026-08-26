import { useEffect, useMemo, useState } from "react";
import {
  Alert,
  Badge,
  Button,
  Card,
  Code,
  Divider,
  Group,
  Loader,
  Modal,
  Paper,
  SegmentedControl,
  Select,
  Stack,
  Text,
  Textarea,
} from "@mantine/core";
import { notifications } from "@mantine/notifications";
import {
  IconAlertCircle,
  IconChecks,
  IconMessageCircle2,
  IconRefresh,
  IconRobot,
  IconSend,
} from "@tabler/icons";
import { AppContainer } from "shared/components";
import { localStoreService, mcpChatService } from "core/services";

const WRITE_TOOLS = new Set([
  "users.create",
  "users.update_profile",
  "address.create",
  "contact.create",
  "schedule.create",
]);

const getStarterExamples = () => {
  const organizationId = localStoreService.getOrganizationID() || "";
  const franchiseId = localStoreService.getFranchiseID() || "";
  const userId = localStoreService.getUserID() || "";

  return {
    "users.search": {
      franchiseId,
      userType: 1,
      firstName: "Ahsan",
      pageNumber: 1,
      pageSize: 10,
    },
    "users.create": {
      userType: 1,
      franchiseId,
      firstName: "Ahsan",
      lastName: "Khan",
      email: "ahsan@example.com",
      phoneNo: "+92511234567",
      mobileNo: "+923001234567",
      birthDate: "1990-01-01",
      joiningDate: "2026-04-15",
    },
    "users.update_profile": {
      userId,
      email: "updated@example.com",
      mobileNo: "+923001234567",
    },
    "address.create": {
      userId,
      addressLine1: "House 1, Main Street",
      addressTypeId: 1,
      countyId: 1,
      stateId: 1,
      countryId: 1,
    },
    "contact.create": {
      userId,
      franchiseId,
      firstName: "John",
      lastName: "Doe",
      mobileNo: "+923001234567",
      email: "john@example.com",
      contactTypeId: 1,
    },
    "schedule.create": {
      organizationId,
      createdBy: userId,
      clientId: "",
      csvServiceProviderIds: "",
      serviceType: 1,
      csvServiceIds: "1",
      scheduleDescription: "Morning personal care visit",
      startTime: "2026-04-20T09:00:00",
      endTime: "2026-04-20T11:00:00",
      recurrencePattern: 1,
      recurrenceInterval: 1,
    },
  };
};

const formatToolResult = (result) => {
  if (result?.content?.[0]?.text) {
    return result.content[0].text;
  }

  return JSON.stringify(result, null, 2);
};

const buildChatSummary = (toolName, args) =>
  `Run \`${toolName}\` with:\n${JSON.stringify(args, null, 2)}`;

const AdminCopilot = () => {
  const [mode, setMode] = useState("chat");
  const [isLoading, setIsLoading] = useState(false);
  const [isCheckingBridge, setIsCheckingBridge] = useState(true);
  const [bridgeHealthy, setBridgeHealthy] = useState(false);
  const [bridgeConfig, setBridgeConfig] = useState(null);
  const [tools, setTools] = useState([]);
  const [selectedTool, setSelectedTool] = useState("");
  const [argumentsText, setArgumentsText] = useState("{}");
  const [chatInput, setChatInput] = useState("");
  const [messages, setMessages] = useState([
    {
      role: "assistant",
      text:
        "Admin Copilot is ready. Pick a tool, review the JSON payload, and run it. Write actions require approval before execution.",
    },
  ]);
  const [chatMessages, setChatMessages] = useState([
    {
      role: "assistant",
      text:
        "Ask a natural-language admin question and I’ll use the live read tools to answer it. For create or update actions, use Tool Runner.",
    },
  ]);
  const [pendingAction, setPendingAction] = useState(null);

  const toolOptions = useMemo(
    () =>
      tools.map((tool) => ({
        value: tool.name,
        label: tool.name,
      })),
    [tools]
  );

  const selectedToolDefinition = tools.find((tool) => tool.name === selectedTool);

  const loadTools = async () => {
    setIsCheckingBridge(true);
    try {
      await mcpChatService.health();
      setBridgeHealthy(true);
      const configInfo = await mcpChatService.configCheck();
      setBridgeConfig(configInfo);
      const nextTools = await mcpChatService.listTools();
      setTools(nextTools);

      if (!selectedTool && nextTools.length > 0) {
        setSelectedTool(nextTools[0].name);
        setArgumentsText(
          JSON.stringify(getStarterExamples()[nextTools[0].name] || {}, null, 2)
        );
      }
    } catch (error) {
      setBridgeHealthy(false);
      setBridgeConfig(null);
      notifications.show({
        title: "MCP bridge unavailable",
        message: error.message || "Could not reach the MCP bridge",
        color: "red",
      });
    } finally {
      setIsCheckingBridge(false);
    }
  };

  useEffect(() => {
    loadTools();
  }, []);

  const appendMessage = (role, text) => {
    setMessages((current) => [...current, { role, text }]);
  };

  const appendChatMessage = (role, text) => {
    setChatMessages((current) => [...current, { role, text }]);
  };

  const handleToolChange = (value) => {
    setSelectedTool(value || "");
    setArgumentsText(
      JSON.stringify(getStarterExamples()[value] || {}, null, 2)
    );
  };

  const executeTool = async (toolName, rawArgs, { autoConfirm = false } = {}) => {
    const parsedArgs = JSON.parse(rawArgs);
    const finalArgs = autoConfirm ? { ...parsedArgs, confirm: true } : parsedArgs;

    appendMessage("user", buildChatSummary(toolName, finalArgs));

    if (WRITE_TOOLS.has(toolName) && finalArgs.confirm !== true) {
      setPendingAction({
        toolName,
        args: parsedArgs,
      });
      appendMessage(
        "assistant",
        `Approval required for \`${toolName}\`. Review the action and approve to continue.`
      );
      return;
    }

    setIsLoading(true);
    try {
      const result = await mcpChatService.callTool(toolName, finalArgs);
      appendMessage("assistant", formatToolResult(result));
      notifications.show({
        title: "Tool executed",
        message: `${toolName} completed successfully`,
        color: "green",
      });
    } catch (error) {
      appendMessage("assistant", `Error: ${error.message}`);
      notifications.show({
        title: "Tool failed",
        message: error.message || "The MCP tool call failed",
        color: "red",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleRun = async () => {
    if (!selectedTool) {
      notifications.show({
        title: "Tool required",
        message: "Select a tool first",
        color: "yellow",
      });
      return;
    }

    try {
      await executeTool(selectedTool, argumentsText);
    } catch (error) {
      notifications.show({
        title: "Invalid JSON",
        message: error.message || "Arguments must be valid JSON",
        color: "red",
      });
    }
  };

  const handleApprovePending = async () => {
    if (!pendingAction) {
      return;
    }

    const action = pendingAction;
    setPendingAction(null);
    await executeTool(action.toolName, JSON.stringify(action.args), {
      autoConfirm: true,
    });
  };

  const handleCancelPending = () => {
    if (pendingAction) {
      appendMessage(
        "assistant",
        `Cancelled pending action \`${pendingAction.toolName}\`.`
      );
    }
    setPendingAction(null);
  };

  const handleSendChat = async () => {
    const trimmedMessage = chatInput.trim();

    if (!trimmedMessage) {
      notifications.show({
        title: "Message required",
        message: "Type a question for the AI assistant first",
        color: "yellow",
      });
      return;
    }

    const historyForRequest = [...chatMessages, { role: "user", text: trimmedMessage }];

    appendChatMessage("user", trimmedMessage);
    setChatInput("");
    setIsLoading(true);

    try {
      const result = await mcpChatService.chat(trimmedMessage, chatMessages);
      const toolNote =
        Array.isArray(result?.usedTools) && result.usedTools.length > 0
          ? `\n\nUsed tools: ${result.usedTools.map((tool) => tool.name).join(", ")}`
          : "";

      appendChatMessage(
        "assistant",
        `${result?.answer || "I could not generate a response."}${toolNote}`
      );
    } catch (error) {
      appendChatMessage("assistant", `Error: ${error.message}`);
      notifications.show({
        title: "Chat failed",
        message: error.message || "The AI chat request failed",
        color: "red",
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <AppContainer
        title="AI Copilot"
        showDivider="true"
        button={
          <Group spacing="xs">
            <Badge color={bridgeHealthy ? "green" : "red"} variant="filled">
              {isCheckingBridge ? "Checking bridge" : bridgeHealthy ? "Bridge online" : "Bridge offline"}
            </Badge>
            <Button
              variant="outline"
              leftIcon={<IconRefresh size={16} />}
              onClick={loadTools}
              loading={isCheckingBridge}
            >
              Refresh
            </Button>
          </Group>
        }
      >
        <Stack spacing="md">
          <Alert
            icon={<IconRobot size={16} />}
            color="blue"
            variant="light"
            title="Admin workflow assistant"
          >
            Use Chat Assistant for natural-language read queries, or Tool Runner for explicit MCP-backed admin actions. Write tools still pause for approval first.
          </Alert>

          {!bridgeHealthy && !isCheckingBridge ? (
            <Alert
              icon={<IconAlertCircle size={16} />}
              color="red"
              variant="light"
              title="MCP bridge unavailable"
            >
              Start `Scheduler.MCP` with `MCP_HTTP_PORT` configured, then refresh this page.
            </Alert>
          ) : null}

          {bridgeHealthy && bridgeConfig ? (
            <Alert
              color={bridgeConfig.openaiConfigured ? "green" : "yellow"}
              variant="light"
              title="Bridge config"
            >
              {`OpenAI key: ${bridgeConfig.openaiConfigured ? "loaded" : "missing"} | Model: ${bridgeConfig.openaiModel} | Env file: ${bridgeConfig.envFileExists ? "found" : "missing"}`}
            </Alert>
          ) : null}

          <SegmentedControl
            value={mode}
            onChange={setMode}
            data={[
              { label: "Chat Assistant", value: "chat" },
              { label: "Tool Runner", value: "tools" },
            ]}
          />

          {mode === "chat" ? (
            <Group align="flex-start" grow>
              <Card withBorder radius="md" shadow="sm" style={{ minHeight: 540 }}>
                <Stack spacing="sm">
                  <Group position="apart">
                    <Group spacing="xs">
                      <IconRobot size={18} />
                      <Text weight={600}>Chat Assistant</Text>
                    </Group>
                    {isLoading ? <Loader size="sm" /> : null}
                  </Group>

                  <Alert color="gray" variant="light">
                    Ask business questions like `How many users are registered, what are their ages, and give me the email of the youngest?`
                  </Alert>

                  <Textarea
                    label="Your question"
                    minRows={8}
                    autosize
                    value={chatInput}
                    onChange={(event) => setChatInput(event.target.value)}
                    placeholder="Ask about users, tasks, availability, planboard, or to-confirm data..."
                  />

                  <Group position="right">
                    <Button
                      leftIcon={<IconSend size={16} />}
                      onClick={handleSendChat}
                      disabled={!bridgeHealthy}
                      loading={isLoading}
                    >
                      Ask AI
                    </Button>
                  </Group>
                </Stack>
              </Card>

              <Card withBorder radius="md" shadow="sm" style={{ minHeight: 540 }}>
                <Stack spacing="sm">
                  <Group spacing="xs">
                    <IconMessageCircle2 size={18} />
                    <Text weight={600}>Chat</Text>
                  </Group>
                  <Divider />
                  <Stack spacing="sm" style={{ maxHeight: 460, overflowY: "auto" }}>
                    {chatMessages.map((message, index) => (
                      <Paper
                        key={index}
                        p="sm"
                        radius="md"
                        withBorder
                        style={{
                          backgroundColor:
                            message.role === "assistant" ? "#f8fbff" : "#f7f7f7",
                        }}
                      >
                        <Text size="xs" color="dimmed" tt="uppercase" mb={6}>
                          {message.role}
                        </Text>
                        <Code
                          block
                          style={{
                            whiteSpace: "pre-wrap",
                            wordBreak: "break-word",
                          }}
                        >
                          {message.text}
                        </Code>
                      </Paper>
                    ))}
                  </Stack>
                </Stack>
              </Card>
            </Group>
          ) : (
            <Group align="flex-start" grow>
              <Card withBorder radius="md" shadow="sm" style={{ minHeight: 540 }}>
                <Stack spacing="sm">
                  <Group position="apart">
                    <Group spacing="xs">
                      <IconRobot size={18} />
                      <Text weight={600}>Tool Runner</Text>
                    </Group>
                    {isLoading ? <Loader size="sm" /> : null}
                  </Group>

                  <Select
                    label="Tool"
                    placeholder="Select MCP tool"
                    data={toolOptions}
                    value={selectedTool}
                    onChange={handleToolChange}
                    searchable
                  />

                  {selectedToolDefinition ? (
                    <Paper withBorder p="sm" radius="md">
                      <Text size="sm" weight={600}>
                        {selectedToolDefinition.name}
                      </Text>
                      <Text size="sm" color="dimmed" mt={4}>
                        {selectedToolDefinition.description}
                      </Text>
                    </Paper>
                  ) : null}

                  <Textarea
                    label="Arguments JSON"
                    minRows={16}
                    autosize
                    value={argumentsText}
                    onChange={(event) => setArgumentsText(event.target.value)}
                    placeholder='{"firstName":"Ahsan"}'
                    styles={{
                      input: {
                        fontFamily: "Consolas, Monaco, monospace",
                      },
                    }}
                  />

                  <Group position="right">
                    <Button
                      leftIcon={<IconSend size={16} />}
                      onClick={handleRun}
                      disabled={!bridgeHealthy || !selectedTool}
                      loading={isLoading}
                    >
                      Run Tool
                    </Button>
                  </Group>
                </Stack>
              </Card>

              <Card withBorder radius="md" shadow="sm" style={{ minHeight: 540 }}>
                <Stack spacing="sm">
                  <Group spacing="xs">
                    <IconMessageCircle2 size={18} />
                    <Text weight={600}>Tool Conversation</Text>
                  </Group>
                  <Divider />
                  <Stack spacing="sm" style={{ maxHeight: 460, overflowY: "auto" }}>
                    {messages.map((message, index) => (
                      <Paper
                        key={index}
                        p="sm"
                        radius="md"
                        withBorder
                        style={{
                          backgroundColor:
                            message.role === "assistant" ? "#f8fbff" : "#f7f7f7",
                        }}
                      >
                        <Text size="xs" color="dimmed" tt="uppercase" mb={6}>
                          {message.role}
                        </Text>
                        <Code
                          block
                          style={{
                            whiteSpace: "pre-wrap",
                            wordBreak: "break-word",
                          }}
                        >
                          {message.text}
                        </Code>
                      </Paper>
                    ))}
                  </Stack>
                </Stack>
              </Card>
            </Group>
          )}
        </Stack>
      </AppContainer>

      <Modal
        opened={Boolean(pendingAction)}
        onClose={handleCancelPending}
        title="Approve write action"
        centered
      >
        <Stack spacing="md">
          <Alert
            icon={<IconAlertCircle size={16} />}
            color="yellow"
            variant="light"
            title="Confirmation required"
          >
            This MCP tool will perform a write action against your Scheduler API.
          </Alert>

          {pendingAction ? (
            <Code
              block
              style={{
                whiteSpace: "pre-wrap",
                wordBreak: "break-word",
              }}
            >
              {buildChatSummary(pendingAction.toolName, pendingAction.args)}
            </Code>
          ) : null}

          <Group position="right">
            <Button variant="default" onClick={handleCancelPending}>
              Cancel
            </Button>
            <Button
              color="green"
              leftIcon={<IconChecks size={16} />}
              onClick={handleApprovePending}
              loading={isLoading}
            >
              Approve and Run
            </Button>
          </Group>
        </Stack>
      </Modal>
    </>
  );
};

export default AdminCopilot;
