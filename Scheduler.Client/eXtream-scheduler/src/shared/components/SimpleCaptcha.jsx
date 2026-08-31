import { ActionIcon, Group, Paper, Text, TextInput, Tooltip } from "@mantine/core";
import { IconRefresh } from "@tabler/icons";

export const createCaptchaChallenge = () => {
  const left = Math.floor(Math.random() * 8) + 2;
  const right = Math.floor(Math.random() * 8) + 2;

  return {
    id: `${Date.now()}-${left}-${right}`,
    prompt: `${left} + ${right}`,
    answer: String(left + right),
  };
};

export const isCaptchaValid = (value, answer) => {
  return String(value || "").trim() === String(answer);
};

const SimpleCaptcha = ({ challenge, inputProps, onRefresh, mt = "md" }) => {
  return (
    <Paper
      withBorder
      radius="md"
      p="sm"
      mt={mt}
      style={{
        background: "#f8fafc",
        borderColor: "rgba(99, 102, 241, 0.18)",
      }}
    >
      <Group position="apart" align="center" spacing="sm" noWrap>
        <div>
          <Text size="xs" c="dimmed" fw={600} transform="uppercase">
            Security check
          </Text>
          <Text fw={700} size="sm">
            {challenge.prompt} = ?
          </Text>
        </div>

        <Tooltip label="New challenge" withArrow>
          <ActionIcon
            variant="light"
            color="brand"
            radius="md"
            onClick={onRefresh}
            aria-label="New security challenge"
          >
            <IconRefresh size={16} />
          </ActionIcon>
        </Tooltip>
      </Group>

      <TextInput
        mt="sm"
        size="sm"
        radius="md"
        label="Answer"
        placeholder="Type the result"
        inputMode="numeric"
        autoComplete="off"
        {...inputProps}
      />
    </Paper>
  );
};

export default SimpleCaptcha;
