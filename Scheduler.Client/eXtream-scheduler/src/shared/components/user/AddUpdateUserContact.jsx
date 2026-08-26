import { Button, TextInput, Select, NumberInput, Grid, Switch } from "@mantine/core";
import { useForm, zodResolver } from "@mantine/form";
import { useState, useEffect } from "react";
import { z as zod } from "zod";
import { notifications } from "@mantine/notifications";
import { contactService, lookupService } from "core/services";

const phoneRegex = /^\+?[0-9]+$/;

const schema = zod.object({
  firstName: zod.string().trim().nonempty("First Name is required").max(50, "First Name must be 50 characters or less"),
  surName: zod.string().trim().max(50, "Surname must be 50 characters or less"),
  lastName: zod.string().trim().nonempty("Last Name is required").max(50, "Last Name must be 50 characters or less"),
  alias: zod.string().trim().max(50, "Alias must be 50 characters or less"),
  phoneNo: zod.string().trim().max(15, "Phone Number must be 15 characters or less").refine((value) => !value || phoneRegex.test(value), "Phone number should only contain numbers and optionally start with +"),
  mobileNo: zod.string().trim().nonempty("Mobile Number is required").max(15, "Mobile Number must be 15 characters or less").regex(phoneRegex, "Mobile number should only contain numbers and optionally start with +"),
  email: zod.string().trim().nonempty("Email is required").email("Please enter a valid email address").max(100, "Email must be 100 characters or less"),
  passportNo: zod.string().trim().max(50, "Passport Number must be 50 characters or less"),
  identityNo: zod.string().trim().max(50, "Identity Number must be 50 characters or less"),
  birthDate: zod.string().nonempty("Birth Date is required").refine((value) => new Date(value) <= new Date(), "Birth Date cannot be in the future"),
  contactTypeId: zod.number().positive("Contact Type is required"),
  addressLine1: zod.string().trim().nonempty("Address Line 1 is required").max(100, "Address Line 1 must be 100 characters or less"),
  addressLine2: zod.string().trim().max(100, "Address Line 2 must be 100 characters or less"),
  addressLine3: zod.string().trim().max(100, "Address Line 3 must be 100 characters or less"),
  countyId: zod.number().positive("County is required"),
  stateId: zod.number().positive("State is required"),
  countryId: zod.number().positive("Country is required"),
  latitude: zod.number().min(-90, "Latitude must be between -90 and 90").max(90, "Latitude must be between -90 and 90").optional(),
  longitude: zod.number().min(-180, "Longitude must be between -180 and 180").max(180, "Longitude must be between -180 and 180").optional(),
});

export const AddUpdateUserContact = ({
  id,
  userId,
  onModalClose,
  organizationId,
  franchiseId,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [isFetching, setIsFetching] = useState(false);
  const [contactTypeOptions, setContactTypeOptions] = useState([]);
  const [contactGenderOptions, setContactGenderOptions] = useState([]);
  const [contactTitleOptions, setContactTitleOptions] = useState([]);
  const [contactCountyOptions, setContactCountyOptions] = useState([]);
  const [contactStateOptions, setContactStateOptions] = useState([]);
  const [contactCountryOptions, setContactCountryOptions] = useState([]);

  const form = useForm({
    validate: zodResolver(schema),
    initialValues: {
      firstName: "",
      surName: "",
      lastName: "",
      alias: "",
      phoneNo: "",
      mobileNo: "",
      email: "",
      passportNo: "",
      identityNo: "",
      birthDate: "",
      titleId: 0,
      genderId: 0,
      contactTypeId: 0,
      addressLine1: "",
      addressLine2: "",
      addressLine3: "",
      countyId: 0,
      stateId: 0,
      countryId: 0,
      latitude: 0.000,
      longitude: 0.000,
      isBillingContact: false,
    },
    validateInputOnBlur: true,
  });

  useEffect(() => {
    if (id) {
      setIsFetching(true);
      contactService
        .getContactItem(id)
        .then((response) => {
          console.log('Contact Item Response:', response); // Debug log
          form.setValues({
            firstName: response?.firstName || "",
            surName: response?.surName || "",
            lastName: response?.lastName || "",
            alias: response?.alias || "",
            phoneNo: response?.phoneNo || "",
            mobileNo: response?.mobileNo || "",
            email: response?.email || "",
            passportNo: response?.passportNo || "",
            identityNo: response?.identityNo || "",
            birthDate: response?.birthDate ? response.birthDate.split("T")[0] : "",
            titleId: response?.titleId || 0,
            genderId: response?.genderId || 0,
            contactTypeId: response?.contactTypeId || 0,
            addressLine1: response?.addressLine1 || "",
            addressLine2: response?.addressLine2 || "",
            addressLine3: response?.addressLine3 || "",
            countyId: response?.countyId || 0,
            stateId: response?.stateId || 0,
            countryId: response?.countryId || 0,
            latitude: response?.latitude || 0.000,
            longitude: response?.longitude || 0.000,
            isBillingContact: !!response?.isBillingContact,
          });
        })
        .catch((error) => {
          notifications.show({
            withCloseButton: true,
            autoClose: 5000,
            title: "Error",
            message: "Failed to fetch Contact item",
            color: "red",
            style: {
              backgroundColor: "white",
            },
          });
        })
        .finally(() => setIsFetching(false));
    }
  }, [id]);

  useEffect(() => {
    const fetchLookupData = async () => {
      try {
        const contactTypeResponse = await lookupService.getLookupList({
          lookupType: "ContactType",
          organizationId,
        });
        setContactTypeOptions(
          (contactTypeResponse?.result || []).map((item) => ({
            value: item.id,
            label: item.name,
          }))
        );

        const genderResponse = await lookupService.getLookupList({
          lookupType: "Gender",
          organizationId,
        });
        setContactGenderOptions(
          (genderResponse?.result || []).map((item) => ({
            value: item.id,
            label: item.name,
          }))
        );

        const titleResponse = await lookupService.getLookupList({
          lookupType: "Title",
          organizationId,
        });
        setContactTitleOptions(
          (titleResponse?.result || []).map((item) => ({
            value: item.id,
            label: item.name,
          }))
        );

        const countyResponse = await lookupService.getLookupList({
          lookupType: "County",
          organizationId,
        });
        setContactCountyOptions(
          (countyResponse?.result || []).map((item) => ({
            value: item.id,
            label: item.name,
          }))
        );

        const stateResponse = await lookupService.getLookupList({
          lookupType: "State",
          organizationId,
        });
        setContactStateOptions(
          (stateResponse?.result || []).map((item) => ({
            value: item.id,
            label: item.name,
          }))
        );

        const countryResponse = await lookupService.getLookupList({
          lookupType: "Country",
          organizationId,
        });
        setContactCountryOptions(
          (countryResponse?.result || []).map((item) => ({
            value: item.id,
            label: item.name,
          }))
        );
      } catch (error) {
        notifications.show({
          title: "Error",
          message: "Failed to fetch lookup data",
          color: "red",
        });
      }
    };

    if (
      contactTypeOptions.length === 0 &&
      contactGenderOptions.length === 0 &&
      contactTitleOptions.length === 0 &&
      contactCountyOptions.length === 0 &&
      contactStateOptions.length === 0 &&
      contactCountryOptions.length === 0
    ) {
      fetchLookupData();
    }
  }, [
    contactTypeOptions.length,
    contactGenderOptions.length,
    contactTitleOptions.length,
    contactCountyOptions.length,
    contactStateOptions.length,
    contactCountryOptions.length,
    organizationId,
  ]);

  const handleSubmit = async (values) => {
    setIsLoading(true);
    let userContactObj = {
      id: id,
      userId: userId,
      firstName: values.firstName,
      surName: values.surName,
      lastName: values.lastName,
      alias: values.alias,
      phoneNo: values.phoneNo,
      mobileNo: values.mobileNo,
      email: values.email,
      passportNo: values.passportNo,
      identityNo: values.identityNo,
      birthDate: values.birthDate,
      titleId: values.titleId,
      genderId: values.genderId,
      contactTypeId: values.contactTypeId,
      addressLine1: values.addressLine1,
      addressLine2: values.addressLine2,
      addressLine3: values.addressLine3,
      countyId: values.countyId,
      stateId: values.stateId,
      countryId: values.countryId,
      latitude: values.latitude,
      longitude: values.longitude,
      franchiseId: franchiseId,
      isBillingContact: values.isBillingContact,
    };

    try {
      const result = await contactService.saveUpdateContact(userContactObj);
      notifications.show({
        withCloseButton: true,
        autoClose: 5000,
        title: "Success",
        message: result?.message || "Contact saved successfully",
        color: "green",
        style: {
          backgroundColor: "white",
        },
      });

      onModalClose();
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

  if (isFetching) return <div>Loading...</div>;

  return (
    <form onSubmit={form.onSubmit(handleSubmit)}>
      <Grid>
        <Grid.Col span={4}>
          <Select
            label="Contact Type"
            required
            placeholder="Select Contact Type"
            data={contactTypeOptions}
            {...form.getInputProps("contactTypeId")}
            tabIndex={1}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <Select
            label="Title"
            placeholder="Select Title"
            data={contactTitleOptions}
            {...form.getInputProps("titleId")}
            tabIndex={2}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <TextInput
            label="First Name"
            required
            placeholder="Enter First Name"
            maxLength={50}
            {...form.getInputProps("firstName")}
            tabIndex={3}
          />
        </Grid.Col>
        <Grid.Col span={12}>
          <Switch
            label="Billing / guarantor contact (can receive invoices for this client)"
            {...form.getInputProps("isBillingContact", { type: "checkbox" })}
          />
        </Grid.Col>
      </Grid>
      <Grid>
        <Grid.Col span={4}>
          <TextInput
            label="Surname"
            placeholder="Enter Surname"
            maxLength={50}
            {...form.getInputProps("surName")}
            tabIndex={4}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <TextInput
            label="Last Name"
            required
            placeholder="Enter Last Name"
            maxLength={50}
            {...form.getInputProps("lastName")}
            tabIndex={5}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <TextInput 
            label="Alias"
            placeholder="Enter Alias"
            maxLength={50}
            {...form.getInputProps("alias")}
            tabIndex={6}
          />
        </Grid.Col>
      </Grid>
      <Grid>
        <Grid.Col span={4}>
          <TextInput
            label="Phone Number"
            placeholder="Enter Phone Number"
            maxLength={15}
            {...form.getInputProps("phoneNo")}
            tabIndex={7}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <TextInput
            label="Mobile Number"
            required
            placeholder="Enter Mobile Number"
            maxLength={15}
            {...form.getInputProps("mobileNo")}
            tabIndex={8}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <TextInput
            label="Email"
            required
            placeholder="Enter Email"
            maxLength={100}
            {...form.getInputProps("email")}
            tabIndex={9}
          />
        </Grid.Col>
      </Grid>
      <Grid>
        <Grid.Col span={4}>
          <Select
            label="Gender"
            placeholder="Select Gender"
            data={contactGenderOptions}
            {...form.getInputProps("genderId")}
            tabIndex={10}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <TextInput
            label="Birth Date"
            type="date"
            required
            max={new Date().toISOString().split("T")[0]}
            {...form.getInputProps("birthDate")}
            tabIndex={11}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <TextInput
            label="Identity Number"
            placeholder="Enter Identity Number"
            maxLength={50}
            {...form.getInputProps("identityNo")}
            tabIndex={12}
          />
        </Grid.Col>
      </Grid>
      <Grid>
        <Grid.Col span={12}>
          <TextInput
            label="Address Line 1"
            required
            placeholder="Enter Address Line 1 (max 100 characters)"
            maxLength={100}
            {...form.getInputProps("addressLine1")}
            tabIndex={13}
          />
          <TextInput
            label="Address Line 2"
            placeholder="Enter Address Line 2 (max 100 characters)"
            maxLength={100}
            {...form.getInputProps("addressLine2")}
            tabIndex={14}
          />
          <TextInput
            label="Address Line 3"
            placeholder="Enter Address Line 3 (max 100 characters)"
            maxLength={100}
            {...form.getInputProps("addressLine3")}
            tabIndex={15}
          />
        </Grid.Col>
      </Grid>
      <Grid>
        <Grid.Col span={4}>
          <Select
            label="County"
            required
            placeholder="Select County"
            data={contactCountyOptions}
            {...form.getInputProps("countyId")}
            tabIndex={16}
          />
          <NumberInput
            label="Latitude"
            precision={6}
            step={0.000001}
            defaultValue={0.000}
            {...form.getInputProps("latitude")}
            tabIndex={17}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <Select
            label="State"
            required
            placeholder="Select State"
            data={contactStateOptions}
            {...form.getInputProps("stateId")}
            tabIndex={18}
          />
          <NumberInput
            label="Longitude"
            precision={6}
            step={0.000001}
            defaultValue={0.000}
            {...form.getInputProps("longitude")}
            tabIndex={19}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <Select
            label="Country"
            required
            placeholder="Select Country"
            data={contactCountryOptions}
            {...form.getInputProps("countryId")}
            tabIndex={20}
          />
        </Grid.Col>
      </Grid>
      <Button
        type="submit"
        fullWidth
        mt="xl"
        size="md"
        loading={isLoading}
        loaderPosition="right"
        tabIndex={21}
      >
        Save
      </Button>
    </form>
  );
};
