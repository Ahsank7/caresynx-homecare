import React, { useEffect, useState, useCallback } from "react";
import { AppTable, AppModal } from "shared/components";
import { Button, Group, Text, Badge, LoadingOverlay } from "@mantine/core";
import { ProfileTabPanel } from "shared/components/user/ProfileTabPanel";
import { documentService } from "core/services";
import { helperFunctions } from "shared/utils";
import { notifications } from "@mantine/notifications";
import { AddUpdateUserDocument } from "shared/components/user/AddUpdateUserDocument";
import { AppConfirmationModal } from "shared/components/modal/AppConfirmationModal";
import { IconPlus, IconEye, IconEdit, IconTrash } from '@tabler/icons';
import { TruncatedTooltipText } from "shared/components/TruncatedTooltipText";

export function Document({ userId, organizationId, readOnly = false }) {
  const [documents, setDocuments] = useState([]);
  const [totalRecords, setTotalRecords] = useState(0);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isConfirmationModalOpen, setIsConfirmationModalOpen] = useState(false);
  const [currentRowId, setCurrentRowId] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const tableColumns = [
    "Sr No",
    "Document Type",
    "Name",
    "Description",
    "Access Roles",
    "Actions",
  ];

  const fetchDocuments = useCallback(async () => {
    setIsLoading(true);
    try {
      const request = {
        userId: userId,
        sortColumn: "id",
        sortType: "desc",
        pageNumber: 1,
        pageSize: 100,
      };

      const response = await documentService.getDocumentList(request);
      if (response && Array.isArray(response.response)) {
        setDocuments(response.response);
        setTotalRecords(response.totalRecords || response.response.length);
      } else {
        setDocuments([]);
        setTotalRecords(0);
      }
    } catch (error) {
      console.error("Failed to fetch documents:", error);
      notifications.show({
        title: "Error",
        message: "Failed to fetch documents",
        color: "red",
      });
      setDocuments([]);
      setTotalRecords(0);
    } finally {
      setIsLoading(false);
    }
  }, [userId]);

  useEffect(() => {
    fetchDocuments();
  }, [fetchDocuments]);

  const handleActionClick = async (action, rowId) => {
    setCurrentRowId(rowId);

    if (action === "Delete") {
      setIsConfirmationModalOpen(true);
    } else if (action === "Edit") {
      setIsModalOpen(true);
    } else if (action === "Add") {
      setIsModalOpen(true);
    }
  };

  const handleDelete = async () => {
    try {
      const result = await documentService.deleteDocumentItem(currentRowId);
      notifications.show({
        title: "Success",
        message: result?.message || "Document removed successfully",
        color: "green",
      });
      fetchDocuments();
    } catch (error) {
      console.error("Failed to delete document:", error);
      notifications.show({
        title: "Error",
        message: "Failed to delete document",
        color: "red",
      });
    }
    setIsConfirmationModalOpen(false);
  };

  const handleViewDocument = async (documentPath, documentId) => {
    if (documentPath) {
      try {
        // Open the document directly using the provided path
        // The path should already be a complete URL or relative path
        window.open(documentPath, '_blank');
      } catch (error) {
        console.error("Failed to view document:", error);
        notifications.show({
          title: "Error",
          message: "Failed to view document. Please try downloading instead.",
          color: "red",
        });
      }
    } else {
      notifications.show({
        title: "Info",
        message: "Document path not available",
        color: "blue",
      });
    }
  };

  const openAddModal = () => {
    setCurrentRowId(null);
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    fetchDocuments();
  };

  return (
    <>
      <LoadingOverlay visible={isLoading} />
      <ProfileTabPanel
        title="Document"
        description="Uploaded files and records attached to this profile."
        headerActions={
          !readOnly ? (
            <Button leftIcon={<IconPlus size={16} />} onClick={openAddModal}>
              Upload Document
            </Button>
          ) : null
        }
      >

      {documents.length === 0 ? (
        <div style={{ textAlign: "center", padding: "2rem" }}>
          <Text color="dimmed">
            {readOnly
              ? "No documents to display."
              : "No documents found. Upload your first document!"}
          </Text>
        </div>
      ) : (
        <AppTable thead={tableColumns}>
          {documents.map((row, index) => (
            <tr key={row.id || index}>
              <td>{helperFunctions.getRowNumber(100, 1, index)}</td>
              <td>
                <Badge color="brand" variant="light">
                  {row.documentType || "N/A"}
                </Badge>
              </td>
              <td>
                <TruncatedTooltipText value={row.name} maxWidth={220} weight={500} />
              </td>
              <td>
                <TruncatedTooltipText
                  value={row.description || "No description"}
                  maxWidth={240}
                  tooltipWidth={360}
                  size="sm"
                  color="dimmed"
                />
              </td>
              <td>
                <TruncatedTooltipText value={row.accessRoles || "All"} maxWidth={160} size="sm" />
              </td>
              <td>
                <Button.Group>
                  <Button 
                    onClick={() => handleViewDocument(row.documentPath, row.id)}
                    leftIcon={<IconEye size={16} />}
                  >
                    View
                  </Button>
                  <Button
                    color="red"
                    onClick={() => handleActionClick("Delete", row.id)}
                    leftIcon={<IconTrash size={16} />}
                  >
                    Delete
                  </Button>
                </Button.Group>
              </td>
            </tr>
          ))}
        </AppTable>
      )}
      </ProfileTabPanel>

      <AppModal
        opened={isModalOpen}
        onClose={closeModal}
        title={currentRowId ? "Edit Document" : "Upload Document"}
        size="md"
      >
        <AddUpdateUserDocument
          userId={userId}
          id={currentRowId}
          onModalClose={closeModal}
        />
      </AppModal>

      <AppConfirmationModal
        opened={isConfirmationModalOpen}
        onClose={(confirmed) => {
          if (confirmed) handleDelete();
          else setIsConfirmationModalOpen(false);
        }}
        title="Confirm Deletion"
      >
        Are you sure you want to delete this document? This action cannot be undone.
      </AppConfirmationModal>
    </>
  );
}
