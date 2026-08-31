import { Box, Card, Group, Title, ThemeIcon } from "@mantine/core";

export const AppContainer = ({
  title,
  button: Button,
  showDivider,
  margionTop = "1rem",
  icon,
  children,
}) => {
  return (
    <Card
      p={0}
      radius="md"
      shadow="sm"
      withBorder
      sx={(theme) => ({
        height: "100%",
        display: "flex",
        flexDirection: "column",
        overflow: "hidden",
        backgroundColor:
          theme.colorScheme === "dark" ? theme.colors.dark[7] : theme.white,
      })}
    >
      {title && (
        <Card.Section
          px="lg"
          py="md"
          sx={(theme) => ({
            borderBottom: `1px solid ${
              theme.colorScheme === "dark"
                ? theme.colors.dark[4]
                : theme.colors.gray[2]
            }`,
            backgroundColor:
              theme.colorScheme === "dark"
                ? theme.colors.dark[6]
                : theme.colors.gray[0],
          })}
        >
          <Group position="apart" noWrap>
            <Group spacing="sm" noWrap>
              {icon && (
                <ThemeIcon size="lg" radius="md" variant="light" color="brand">
                  {icon}
                </ThemeIcon>
              )}
              <Box>
                {typeof title === "string" ? (
                  <Title order={4} sx={{ fontWeight: 600, letterSpacing: "-0.02em" }}>
                    {title}
                  </Title>
                ) : (
                  title
                )}
              </Box>
            </Group>
            {Button}
          </Group>
        </Card.Section>
      )}

      <Card.Section
        p={0}
        m={0}
        px="lg"
        py="md"
        mt={title ? 0 : margionTop}
        sx={{ flex: 1, minHeight: 0, overflow: "auto" }}
      >
        {children}
      </Card.Section>
    </Card>
  );
};
