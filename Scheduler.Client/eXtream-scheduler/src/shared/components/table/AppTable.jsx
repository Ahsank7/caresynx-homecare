import { useState } from "react";
import { Table, ScrollArea, Button, Box, createStyles } from "@mantine/core";

import { AppTableHeader, AppTablePagination, AppTableBody } from "shared/components";

const useStyles = createStyles((theme) => ({
  filterBtn: {
    position: "absolute",
    right: 0,
    top: "0.75rem",
    zIndex: 1,
    borderBottomLeftRadius: "2rem",
    borderTopLeftRadius: "2rem",
    fontWeight: 600,
  },
}));

export const AppTable = ({
  thead,
  currentPage = 1,
  pageSize,
  totalRecords,
  onPagination,
  onFilterBtn,
  children: tableBody,
  height = "68vh",
  horizontalSpacing = 'xl',
  /** When set, table area uses max-height and grows with content until cap (e.g. modals). */
  tableMaxHeight,
  showPagination = true,
}) => {
  const [scrolled, setScrolled] = useState(false);
  const { classes } = useStyles();

  const filterButton = onFilterBtn && (
    <Button
      size="md"
      className={classes.filterBtn}
      onClick={onFilterBtn}
    >
      Filter
    </Button>
  );

  const tableEl = (
    <Table
      verticalSpacing="sm"
      horizontalSpacing={horizontalSpacing}
      striped
      highlightOnHover
    >
      <AppTableHeader thead={thead} scrolled={scrolled} />
      <AppTableBody>
        {tableBody}
      </AppTableBody>
    </Table>
  );

  return (
    <div style={{ position: "relative" }}>
      {tableMaxHeight ? (
        <Box
          sx={{
            position: "relative",
            maxHeight: tableMaxHeight,
            overflowY: "auto",
            overflowX: "auto",
          }}
          onScroll={(e) => setScrolled(e.currentTarget.scrollTop > 0)}
        >
          {filterButton}
          {tableEl}
        </Box>
      ) : (
        <ScrollArea
          sx={{ height: height }}
          onScrollPositionChange={({ y }) => setScrolled(y)}
        >
          {filterButton}
          {tableEl}
        </ScrollArea>
      )}

      {showPagination && onPagination && (
        <AppTablePagination
          currentPage={currentPage}
          pageSize={pageSize}
          totalRecords={totalRecords}
          onPagination={onPagination}
        />
      )}
    </div>
  );
};
