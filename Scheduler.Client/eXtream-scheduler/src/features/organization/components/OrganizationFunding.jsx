import { useCallback, useEffect, useState, useMemo } from "react";
import {
  Button,
  Grid,
  Select,
  TextInput,
  NumberInput,
  LoadingOverlay,
  Table,
  Text,
  Paper,
} from "@mantine/core";
import {
  clientPayerService,
  servicesService,
  handleApiError,
  showSuccessNotification,
  showErrorNotification,
} from "core/services";

const isoDateToday = () => new Date().toISOString().split("T")[0];

/**
 * Organization-level funding: which org payer funds what % of a service (or all services), for a date range.
 * Client-specific rules (if any) take precedence in billing; this is the org default.
 */
export default function OrganizationFunding({ organizationId }) {
  const [loading, setLoading] = useState(true);
  const [payers, setPayers] = useState([]);
  const [serviceOptions, setServiceOptions] = useState([]);
  const [funding, setFunding] = useState([]);
  const [rule, setRule] = useState({
    payerId: null,
    serviceId: null,
    fundedPercent: 100,
    effectiveFrom: isoDateToday(),
    effectiveTo: "",
  });

  const serviceLabelById = useMemo(() => {
    const m = new Map();
    serviceOptions.forEach((o) => {
      if (o.value === "" || o.value == null) return;
      const n = parseInt(o.value, 10);
      if (!Number.isNaN(n)) m.set(n, o.label);
    });
    return m;
  }, [serviceOptions]);

  const loadAll = useCallback(async () => {
    if (!organizationId) return;
    setLoading(true);
    try {
      const [pay, svc, fund] = await Promise.all([
        clientPayerService.getPayers(),
        servicesService.getAllServicesForOrganization(organizationId),
        clientPayerService.getOrgFunding(),
      ]);
      setPayers(Array.isArray(pay) ? pay : []);
      if (Array.isArray(svc) && svc.length > 0) {
        setServiceOptions(svc);
      } else {
        setServiceOptions([{ value: "", label: "All services" }]);
      }
      setFunding(Array.isArray(fund) ? fund : []);
    } catch (e) {
      handleApiError(e, "Failed to load payers, services, or funding rules");
    } finally {
      setLoading(false);
    }
  }, [organizationId]);

  useEffect(() => {
    loadAll();
  }, [loadAll]);

  const payerOptions = payers.map((x) => ({
    value: String(x.id ?? x.Id),
    label: (x.legalName ?? x.LegalName) || "Payer",
  }));
  const payerName = (pid) =>
    payers.find((p) => String(p.id ?? p.Id) === String(pid))?.legalName || String(pid);

  const addRule = async () => {
    if (!rule.payerId) {
      showErrorNotification("Select a payer");
      return;
    }
    try {
      await clientPayerService.saveOrgFunding({
        id: 0,
        organizationId,
        payerId: rule.payerId,
        serviceId: rule.serviceId,
        fundedPercent: rule.fundedPercent,
        effectiveFrom: new Date(rule.effectiveFrom + "T12:00:00Z"),
        effectiveTo: rule.effectiveTo ? new Date(rule.effectiveTo + "T12:00:00Z") : null,
        isActive: true,
      });
      showSuccessNotification("Funding rule saved");
      setRule({
        payerId: null,
        serviceId: null,
        fundedPercent: 100,
        effectiveFrom: isoDateToday(),
        effectiveTo: "",
      });
      loadAll();
    } catch (e) {
      handleApiError(e, "Failed to save rule");
    }
  };

  const onDelete = async (id) => {
    try {
      await clientPayerService.deleteOrgFunding(id);
      showSuccessNotification("Rule removed");
      loadAll();
    } catch (e) {
      handleApiError(e, "Failed to delete");
    }
  };

  return (
    <Paper withBorder p="md">
      <LoadingOverlay visible={loading} />
      <Text size="sm" color="dimmed" mb="md">
        Set which organization payer funds what share of billable work (all services or a specific service), and for
        which period. This applies after visit rates are calculated. Client-specific funding rules, when added later,
        override these defaults.
      </Text>

      <Text weight={600} size="sm" mt="md" mb="xs">
        New rule
      </Text>
      <Grid>
        <Grid.Col span={3}>
          <Select
            label="Payer"
            placeholder="Organization payer"
            data={payerOptions}
            value={rule.payerId ? String(rule.payerId) : null}
            onChange={(v) => setRule((r) => ({ ...r, payerId: v }))}
            searchable
            clearable
          />
        </Grid.Col>
        <Grid.Col span={3}>
          <Select
            label="Service"
            data={serviceOptions}
            value={rule.serviceId != null ? String(rule.serviceId) : ""}
            onChange={(v) => setRule((r) => ({ ...r, serviceId: v === "" || v == null ? null : parseInt(v, 10) }))}
            searchable
          />
        </Grid.Col>
        <Grid.Col span={2}>
          <NumberInput
            label="Funded %"
            min={0}
            max={100}
            value={rule.fundedPercent}
            onChange={(v) => setRule((r) => ({ ...r, fundedPercent: v ?? 0 }))}
          />
        </Grid.Col>
        <Grid.Col span={2}>
          <TextInput
            type="date"
            label="From"
            value={rule.effectiveFrom}
            onChange={(e) => setRule((r) => ({ ...r, effectiveFrom: e.target.value }))}
          />
        </Grid.Col>
        <Grid.Col span={2}>
          <TextInput
            type="date"
            label="To"
            value={rule.effectiveTo}
            onChange={(e) => setRule((r) => ({ ...r, effectiveTo: e.target.value }))}
          />
        </Grid.Col>
      </Grid>
      <Button my="sm" onClick={addRule}>
        Add rule
      </Button>

      <Text weight={600} size="sm" mt="lg" mb="xs">
        Organization rules
      </Text>
      <Table striped withBorder withColumnBorders>
        <thead>
          <tr>
            <th>Id</th>
            <th>Payer</th>
            <th>Service</th>
            <th>Funded %</th>
            <th>From</th>
            <th>To</th>
            <th> </th>
          </tr>
        </thead>
        <tbody>
          {funding.length === 0 ? (
            <tr>
              <td colSpan={7}>
                <Text size="sm" color="dimmed">
                  No rules — full client responsibility (unless a client-level rule exists).
                </Text>
              </td>
            </tr>
          ) : (
            funding.map((f) => {
              const rowId = f.id ?? f.Id;
              const pid = f.payerId ?? f.PayerId;
              const sid = f.serviceId ?? f.ServiceId;
              return (
              <tr key={rowId}>
                <td>{rowId}</td>
                <td>{payerName(pid)}</td>
                <td>
                  {sid == null
                    ? "All services"
                    : serviceLabelById.get(sid) ?? String(sid)}
                </td>
                <td>{f.fundedPercent ?? f.FundedPercent}</td>
                <td>{(f.effectiveFrom || f.EffectiveFrom) ? new Date(f.effectiveFrom ?? f.EffectiveFrom).toLocaleDateString() : "—"}</td>
                <td>{(f.effectiveTo || f.EffectiveTo) ? new Date(f.effectiveTo ?? f.EffectiveTo).toLocaleDateString() : "—"}</td>
                <td>
                  <Button size="xs" color="red" variant="light" onClick={() => onDelete(rowId)}>
                    Delete
                  </Button>
                </td>
              </tr>
              );
            })
          )}
        </tbody>
      </Table>
    </Paper>
  );
}
