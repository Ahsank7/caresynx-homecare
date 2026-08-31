import React, { useEffect, useState, useCallback } from "react";
import { Button, Badge, LoadingOverlay } from "@mantine/core";
import { ProfileTabPanel } from "shared/components/user/ProfileTabPanel";
import { notifications } from "@mantine/notifications";
import { AppTable, AppModal } from "shared/components";
import { AddUpdateUserAddress } from "shared/components/user/AddUpdateUserAddress";
import { AppConfirmationModal } from "shared/components/modal/AppConfirmationModal";
import { profileService, addressService } from "core/services";
import { helperFunctions } from "shared/utils";
import { IconPlus, IconEdit, IconTrash } from "@tabler/icons";
import { TruncatedTooltipText } from "shared/components/TruncatedTooltipText";

export function Address({ userId, organizationId, readOnly = false }) {
  const [Address, setAddress] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isConfirmationModalOpen, setIsConfirmationModalOpen] = useState(false);
  const [currentRowId, setCurrentRowId] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const tableColumns = [
    "SrNo",
    "Type",
    "Address Line 1",
    "County",
    "State",
    "Country",
    "Actions",
  ];

  const fetchAddress = useCallback(async () => {
    setIsLoading(true);
    const request = {
      userId: userId,
      sortColumn: "id",
      sortType: "desc",
      pageNumber: 1,
      pageSize: 100,
    };

    try {
      const response = await profileService.getAddressList(request);
      console.log('Address Response:', response); // Debug log
      setAddress(response?.response || []);
    } catch (error) {
      console.error("Failed to fetch address data:", error);
      setAddress([]);
    } finally {
      setIsLoading(false);
    }
  }, [userId]);

  useEffect(() => {
    fetchAddress();
  }, [fetchAddress]);

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
      const result = await addressService.deleteAddressItem(currentRowId);
      notifications.show({
        title: "Success",
        message: result?.message || "Address removed successfully",
        color: "green",
      });
      fetchAddress();
    } catch (error) {
      console.error("Failed to delete Address item:", error);
    }
    setIsConfirmationModalOpen(false);
  };

  const openAddModal = () => {
    setCurrentRowId(null);
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    fetchAddress();
  };

  return (
    <>
      <LoadingOverlay visible={isLoading} />
      <ProfileTabPanel
        title="Address"
        description="Manage physical addresses linked to this user."
        headerActions={
          !readOnly ? (
            <Button leftIcon={<IconPlus size={16} />} onClick={openAddModal}>
              Create
            </Button>
          ) : null
        }
      >
      <AppTable thead={tableColumns}>
        {Address &&
          Address.map((row, index) => (
            <tr key={index}>
              <td>{helperFunctions.getRowNumber(100, 1, index)}</td>
              <td>
                <Badge color="brand" variant="light">
                  {row.addressType || "N/A"}
                </Badge>
              </td>
              <td>
                <TruncatedTooltipText value={row.addressLine1} maxWidth={220} tooltipWidth={340} />
              </td>
              <td>{row.county}</td>
              <td>{row.state}</td>
              <td>{row.country}</td>
              <td>
                {!readOnly ? (
                  <Button.Group>
                    <Button
                      onClick={() => handleActionClick("Edit", row.id)}
                      leftIcon={<IconEdit size={16} />}
                    >
                      Edit
                    </Button>
                    <Button
                      color="red"
                      onClick={() => handleActionClick("Delete", row.id)}
                      leftIcon={<IconTrash size={16} />}
                    >
                      Delete
                    </Button>
                  </Button.Group>
                ) : (
                  <span style={{ color: "#868e96", fontSize: 12 }}>View only</span>
                )}
              </td>
            </tr>
          ))}
      </AppTable>
      </ProfileTabPanel>

      <AppModal
        opened={isModalOpen}
        onClose={closeModal}
        title={currentRowId ? "Edit Address" : "Add Address"}
        size="xl"
      >
        <AddUpdateUserAddress
          userId={userId}
          id={currentRowId}
          onModalClose={closeModal}
          organizationId={organizationId}
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
        Are you sure you want to delete this Address item?
      </AppConfirmationModal>
    </>
  );
}
