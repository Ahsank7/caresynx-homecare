import { Box, createStyles, Pagination } from "@mantine/core";

const useStyles = createStyles((theme) => ({
  pagination: {
    display: "flex",
    justifyContent: "flex-end",
    padding: `${theme.spacing.sm} ${theme.spacing.md}`,
    borderTop: `1px solid ${
      theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[2]
    }`,
  },
}));

export const AppTablePagination = ({
  currentPage,
  pageSize,
  totalRecords,
  onPagination,
}) => {
  const { classes } = useStyles();

  const totalPages = Math.ceil(totalRecords / pageSize);

  return (
    <Box className={classes.pagination}>
      <Pagination
        page={currentPage}
        total={totalPages}
        onChange={onPagination}
        radius="md"
        size="sm"
        withEdges
        siblings={0}
      />
    </Box>
  );
};
