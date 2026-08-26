import {
    Button,
    LoadingOverlay,
    TextInput,
    Collapse,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { Fragment, useEffect, useState } from "react";
import Moment from 'moment';
import { localStoreService, planboardService } from "core/services";
import { AppTable, AppContainer, AppDrawer, FilterSection } from "shared/components";

const BillingDetail = () => {
    const [isLoading, setIsLoading] = useState(false);
    const [invoices, setInvoices] = useState([]);
    const [details, setDetails] = useState([]);
    const [clientName, setClientName] = useState("");
    const [serviceProviderName, setServiceProviderName] = useState("");
    const [clientPhoneNumber, setClientPhoneNumber] = useState("");
    const [clientEmail, setClientEmail] = useState("");
    const [pageNumber, setPageNumber] = useState(1);
    const [totalRecords, setTotalRecords] = useState(0);

    const [opened, { open, close }] = useDisclosure(false);

    const pageSize = 25;
    const invoiceColumns = [
        "Id",
        "Details",
        "TotalAmount",
        "Date",
        "StartDate",
        "EndDate",
        "DueDate",
        "IsPaid",
        "ClientId",
    ];
    const detailColumns = [
        "Id",
        "BillingInvoiceId",
        "TaskId",
        "Amount",
    ];

    useEffect(() => {
        getInvoices();
    }, [pageNumber]);

    const getInvoices = async (filterOverrides) => {
        const obj = {};

        obj.clientName = filterOverrides?.clientName ?? clientName;
        obj.clientEmail = filterOverrides?.clientEmail ?? clientEmail;
        obj.clientPhoneNumber = filterOverrides?.clientPhoneNumber ?? clientPhoneNumber;
        obj.serviceProviderName = filterOverrides?.serviceProviderName ?? serviceProviderName;
        obj.franchiseId = localStoreService.getFranchiseID();
        obj.pageNumber = filterOverrides?.pageNumber ?? pageNumber;
        obj.pageSize = pageSize;

        setIsLoading(true);
        const { data: invoicesData, totalRecords } = await planboardService.getInvoices(obj);
        setInvoices(invoicesData);
        setTotalRecords(totalRecords);
        setIsLoading(false);
    };

    const handlePagination = (pageNumber) => {
        setPageNumber(pageNumber);
    };

    const handleFilter = async () => {
        close();
        await getInvoices();
    };

    const handleReset = async () => {
        setClientName("");
        setServiceProviderName("");
        setClientPhoneNumber("");
        setClientEmail("");
        setPageNumber(1);
        close();
        await getInvoices({
            clientName: "",
            clientEmail: "",
            clientPhoneNumber: "",
            serviceProviderName: "",
            pageNumber: 1,
        });
    };

    const handleExpand = async (invoiceId) => {
        const { data: detailsData } = await planboardService.getInvoiceDetails(invoiceId);
        setDetails(detailsData);
    };

    return (
        <>
            <AppContainer
                title="Billing"
                button={<Button>Create Task</Button>}
                showDivider="true"
            >
                <LoadingOverlay visible={isLoading} />
                <AppTable
                    thead={invoiceColumns}
                    currentPage={pageNumber}
                    pageSize={pageSize}
                    totalRecords={totalRecords}
                    onPagination={handlePagination}
                    onFilterBtn={open}
                >
                    {invoices.map((invoice, index) => (
                        <Fragment key={index}>
                            <tr>
                                <td>{invoice.Id}</td>
                                <td>{invoice.Details}</td>
                                <td>{invoice.TotalAmount}</td>
                                <td>{Moment(invoice.Date).format('YYYY-MM-DD')}</td>
                                <td>{Moment(invoice.StartDate).format('YYYY-MM-DD')}</td>
                                <td>{Moment(invoice.EndDate).format('YYYY-MM-DD')}</td>
                                <td>{Moment(invoice.DueDate).format('YYYY-MM-DD')}</td>
                                <td>{invoice.IsPaid ? 'Yes' : 'No'}</td>
                                <td>{invoice.ClientId}</td>
                            </tr>
                            <tr>
                                <td colSpan={9}>
                                    <Collapse in={details.length > 0 && details[0].BillingInvoiceId === invoice.Id}>
                                        <AppTable thead={detailColumns}>
                                            {details.filter(detail => detail.BillingInvoiceId === invoice.Id).map((detail, detailIndex) => (
                                                <tr key={detailIndex}>
                                                    <td>{detail.Id}</td>
                                                    <td>{detail.BillingInvoiceId}</td>
                                                    <td>{detail.TaskId}</td>
                                                    <td>{detail.Amount}</td>
                                                </tr>
                                            ))}
                                        </AppTable>
                                    </Collapse>
                                </td>
                            </tr>
                        </Fragment>
                    ))}
                </AppTable>
            </AppContainer>

            <AppDrawer
                opened={opened}
                close={close}
                onFilter={handleFilter}
                onReset={handleReset}
                title="Invoice filters"
                description="Search invoices by client contact details or provider name."
            >
                <form>
                    <FilterSection title="Client">
                        <TextInput
                            label="Name"
                            placeholder="Client name contains…"
                            size="md"
                            value={clientName}
                            onChange={(e) => setClientName(e.target.value)}
                        />
                        <TextInput
                            label="Email"
                            placeholder="name@example.com"
                            size="md"
                            value={clientEmail}
                            onChange={(e) => setClientEmail(e.target.value)}
                        />
                        <TextInput
                            label="Phone"
                            placeholder="Phone number"
                            size="md"
                            value={clientPhoneNumber}
                            onChange={(e) => setClientPhoneNumber(e.target.value)}
                        />
                    </FilterSection>
                    <FilterSection title="Provider">
                        <TextInput
                            label="Provider name"
                            placeholder="Service provider name contains…"
                            size="md"
                            value={serviceProviderName}
                            onChange={(e) => setServiceProviderName(e.target.value)}
                        />
                    </FilterSection>
                </form>
            </AppDrawer>
        </>
    );
};

export default BillingDetail;