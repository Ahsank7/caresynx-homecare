import { Button, TextInput, Loader, Select, LoadingOverlay, Title } from "@mantine/core";
import { ProfileTabPanel } from "shared/components/user/ProfileTabPanel";
import { useForm, zodResolver } from "@mantine/form";
import { useState, useEffect } from "react";
import { z as zod } from "zod";
import { notifications } from "@mantine/notifications";
import { accountService, lookupService } from "core/services";

const schema = zod.object({
  accountHolderName: zod.string().nonempty("Account Holder Name is required").max(50, "Account Holder Name must be 50 characters or less"),
  accountNumber: zod.string().nonempty("Account Number is required").max(50, "Account Number must be 50 characters or less"),
  branchCode: zod.string().nonempty("Branch Code is required").max(10, "Branch Code must be 10 characters or less"),
  iban: zod.string().nonempty("IBAN is required").max(50, "IBAN must be 50 characters or less"),
  bankId: zod.number({
    required_error: "Bank is required",
    invalid_type_error: "Bank is required"
  }).min(1, "Bank is required"),
  bankAccountId: zod.string(),
});

export const BankInfo = ({ userId, organizationId, readOnly = false }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [isFetching, setIsFetching] = useState(false);
  const [bankOptions, setBankOptions] = useState([]);
  const form = useForm({
    validate: zodResolver(schema),
    initialValues: {
      accountHolderName: "",
      accountNumber: "",
      bankId: null,
      branchCode: "",
      iban: "",
      bankAccountId: "", // Initialize bankAccountId in the form state
    },
    validateInputOnBlur: true,
  });

  const fetchBankData = async () => {
    try {
      const bankResponse = await lookupService.getLookupList({
        lookupType: "Banks",
        organizationId,
      });
      setBankOptions(
        (bankResponse?.result || []).map((item) => ({ value: item.id, label: item.name }))
      );
    } catch (error) {
      notifications.show({
        id: "bank-lookup-fetch-error",
        title: "Error",
        message: "Failed to fetch bank data",
        color: "red",
      });
    }
  };

  const fetchUserBankAccount = async () => {
    if (userId) {
      setIsFetching(true);
      try {
        const response = await accountService.getUserBankAccount(userId);
        console.log('Bank Info Response:', response); // Debug log
        
        form.setValues({
          accountHolderName: response?.accountHolderName || "",
          accountNumber: response?.accountNumber || "",
          bankId: response?.bankId || null,
          branchCode: response?.branchCode || "",
          iban: response?.iban || "",
          bankAccountId: response?.bankAccountId || "", // Set bankAccountId from the response
        });
      } catch (error) {
        notifications.show({
          id: "user-bank-account-fetch-error",
          withCloseButton: true,
          autoClose: 5000,
          title: "Error",
          message: "Failed to fetch Bank item",
          color: "red",
          style: {
            backgroundColor: "white",
          },
        });
      } finally {
        setIsFetching(false);
      }
    }
  };

  useEffect(() => {
    fetchUserBankAccount();
  }, [userId]);

  useEffect(() => {
    if (!organizationId) return;
    fetchBankData();
  }, [organizationId]);

  const handleSubmit = async (values) => {
    setIsLoading(true);
    
    // Debug: Log the values being submitted
    console.log('Form values being submitted:', values);
    console.log('Bank ID value:', values.bankId, 'Type:', typeof values.bankId);
    
    let userBankInfoObj = {
      userId: userId,
      accountHolderName: values.accountHolderName,
      accountNumber: values.accountNumber,
      bankId: values.bankId,
      branchCode: values.branchCode,
      iban: values.iban,
      bankAccountId: values.bankAccountId, // Assuming bankAccountId is fetched and stored in the form state
    };

    try {
      const result = await accountService.upsertUserBankAccount(userBankInfoObj);
      notifications.show({
        withCloseButton: true,
        autoClose: 5000,
        title: "Success",
        message: result?.message || "Bank account information saved successfully",
        color: "green",
        style: {
          backgroundColor: "white",
        },
      });

      fetchUserBankAccount();
    } catch (error) {
      notifications.show({
        withCloseButton: true,
        autoClose: 5000,
        title: "Error",
        message: "Please try again",
        color: "red",
        style: {
          backgroundColor: "white",
        },
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <LoadingOverlay visible={isFetching} />
      <ProfileTabPanel
        title="Bank Account Info"
        description="Bank details used for payouts and reimbursements."
      >
        <form
          onSubmit={
            readOnly
              ? (e) => e.preventDefault()
              : form.onSubmit((values) => handleSubmit(values))
          }
        >
          <div
            style={{
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
              width: "100%",
            }}
          >
            <div style={{ maxWidth: "400px", width: "100%" }}>
              <Title order={5} size="h6" mb="sm" c="dimmed">
                Account details
              </Title>
              <Select
                label="Bank"
                placeholder="Select Bank"
                {...form.getInputProps("bankId")}
                data={bankOptions}
                required
                error={form.errors.bankId}
                clearable
                disabled={readOnly}
              />
              <TextInput
                label="Account Holder Name"
                placeholder="Enter Account Holder Name (max 50 characters)"
                required
                maxLength={50}
                {...form.getInputProps("accountHolderName")}
                disabled={readOnly}
              />
              <TextInput
                label="Account Number"
                placeholder="Enter Account Number (max 50 characters)"
                {...form.getInputProps("accountNumber")}
                maxLength={50}
                required
                disabled={readOnly}
              />
              <TextInput
                label="Branch Code"
                placeholder="Enter Branch Code (max 10 characters)"
                {...form.getInputProps("branchCode")}
                maxLength={10}
                required
                disabled={readOnly}
              />
              <TextInput
                label="IBAN"
                placeholder="Enter IBAN (max 50 characters)"
                {...form.getInputProps("iban")}
                maxLength={50}
                required
                disabled={readOnly}
              />

              {!readOnly && (
                <Button
                  type="submit"
                  fullWidth
                  mt="xl"
                  size="md"
                  loading={isLoading}
                  loaderPosition="right"
                >
                  Save
                </Button>
              )}
            </div>
          </div>
        </form>
      </ProfileTabPanel>
    </>
  );
};
