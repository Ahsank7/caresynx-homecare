import { Box, Modal } from "@mantine/core";

export const AppModal = ({
    opened,
    onClose,
    size = "md",
    title,
    children,
    zIndex,
}) => {
    return (
        <Modal
            opened={opened}
            onClose={onClose}
            title={title}
            size={size}
            zIndex={zIndex || 400}
            centered
            closeOnEscape={false}
            closeOnClickOutside={false}
            padding="lg"
            radius="md"
            shadow="xl"
            overlayBlur={4}
            overlayOpacity={0.45}
            transitionProps={{
                transition: "pop",
                duration: 200,
                timingFunction: "ease",
            }}
            styles={(theme) => ({
              title: {
                fontWeight: 600,
                fontSize: theme.fontSizes.lg,
              },
              header: {
                borderBottom: `1px solid ${
                  theme.colorScheme === "dark"
                    ? theme.colors.dark[4]
                    : theme.colors.gray[2]
                }`,
                marginBottom: theme.spacing.md,
                paddingBottom: theme.spacing.sm,
              },
            })}
        >
            <Box style={{ overflow: "hidden" }}>{children}</Box>
        </Modal>
    );
};
