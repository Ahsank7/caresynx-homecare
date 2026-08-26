import { useCallback, useEffect, useState } from "react";
import {
  Button,
  Grid,
  Select,
  TextInput,
  LoadingOverlay,
  Group,
  Table,
  Text,
  Switch,
} from "@mantine/core";
import { ProfileTabPanel } from "shared/components/user/ProfileTabPanel";
import { clientPayerService, profileService, handleApiError, showSuccessNotification, showErrorNotification } from "core/services";
import { helperFunctions } from "shared/utils";

const BILL_TO = [
  { value: "1", label: "Client (self-pay / household)" },
  { value: "2", label: "Organization payer" },
  { value: "3", label: "Contact (guarantor)" },
];

const isoDateToday = () => new Date().toISOString().split("T")[0];

/**
 * Client: who receives invoices (default bill-to). If "organization payer", add coverage (policy, dates).
 * Payer master data and per-service funding % are managed under Organization settings.
 */
export function ClientPayerAndBilling({ clientId, organizationId, readOnly = false }) {
  const [loading, setLoading] = useState(true);
  const [payers, setPayers] = useState([]);
  const [coverage, setCoverage] = useState([]);
  const [preference, setPreference] = useState({ billToType: 1, payerId: null, userContactId: null });
  const [contacts, setContacts] = useState([]);
  const [cov, setCov] = useState({
    payerId: null,
    effectiveFrom: isoDateToday(),
    effectiveTo: "",
    isDefault: false,
    memberNumber: "",
    policyNumber: "",
  });

  const load = useCallback(async () => {
    if (!clientId || !helperFunctions.isValidGUID(clientId)) return;
    setLoading(true);
    try {
      const [p, c, pref, cl] = await Promise.all([
        clientPayerService.getPayers(),
        clientPayerService.getCoverage(clientId),
        clientPayerService.getPreference(clientId),
        profileService.getContactList({ userId: clientId, sortColumn: "id", sortType: "desc", pageNumber: 1, pageSize: 200 }),
      ]);
      setPayers(Array.isArray(p) ? p : []);
      setCoverage(Array.isArray(c) ? c : []);
      setPreference({
        billToType: pref?.billToType ?? 1,
        payerId: pref?.payerId ? String(pref.payerId) : null,
        userContactId: pref?.userContactId ? String(pref.userContactId) : null,
      });
      setContacts(cl?.response || []);
    } catch (e) {
      handleApiError(e, "Failed to load invoice preferences");
    } finally {
      setLoading(false);
    }
  }, [clientId]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    if (preference.billToType === 2 && preference.payerId) {
      setCov((prev) => ({
        ...prev,
        payerId: prev.payerId || preference.payerId,
      }));
    }
  }, [preference.billToType, preference.payerId]);

  const payerOptions = (payers || []).map((x) => ({ value: String(x.id), label: x.legalName || "Payer" }));
  const contactOptions = (contacts || []).map((c) => ({
    value: String(c.id),
    label: `${c.firstName || ""} ${c.lastName || ""}`.trim() || c.email || c.id,
  }));

  const savePref = async () => {
    try {
      await clientPayerService.savePreference({
        clientId,
        billToType: parseInt(String(preference.billToType), 10),
        payerId: preference.billToType === 2 && preference.payerId ? preference.payerId : null,
        userContactId: preference.billToType === 3 && preference.userContactId ? preference.userContactId : null,
      });
      showSuccessNotification("Invoice preferences saved");
      load();
    } catch (e) {
      handleApiError(e, "Failed to save");
    }
  };

  const addCoverage = async () => {
    if (!cov.payerId) {
      showErrorNotification("Select a payer for this coverage row");
      return;
    }
    try {
      await clientPayerService.saveCoverage({
        id: 0,
        clientId,
        payerId: cov.payerId,
        effectiveFrom: new Date(cov.effectiveFrom + "T12:00:00Z"),
        effectiveTo: cov.effectiveTo ? new Date(cov.effectiveTo + "T12:00:00Z") : null,
        isDefaultBillTo: cov.isDefault,
        memberNumber: cov.memberNumber,
        policyNumber: cov.policyNumber,
        isActive: true,
      });
      showSuccessNotification("Coverage saved");
      setCov((c) => ({
        ...c,
        effectiveFrom: isoDateToday(),
        effectiveTo: "",
        memberNumber: "",
        policyNumber: "",
      }));
      load();
    } catch (e) {
      handleApiError(e, "Failed to save coverage");
    }
  };

  const showOrgPayer = preference.billToType === 2;

  return (
    <ProfileTabPanel
      title="Invoice preferences"
      description="Who receives invoices for this client’s share of charges. Add payers and funding rules under Organization settings. When bills go to an organization payer, add policy / coverage details below."
    >
      <LoadingOverlay visible={loading} />

      <Text weight={600} size="sm" mb="xs">
        Default bill-to
      </Text>
      <Grid>
        <Grid.Col span={4}>
          <Select
            label="Send invoices to"
            data={BILL_TO}
            value={String(preference.billToType)}
            onChange={(v) => {
              const bt = v != null ? parseInt(v, 10) : 1;
              setPreference((p) => ({
                ...p,
                billToType: bt,
                payerId: bt === 2 ? p.payerId : null,
                userContactId: bt === 3 ? p.userContactId : null,
              }));
            }}
            disabled={readOnly}
          />
        </Grid.Col>
        {preference.billToType === 2 && (
          <Grid.Col span={4}>
            <Select
              label="Payer (who is invoiced)"
              placeholder="Choose a payer from the org list"
              data={payerOptions}
              value={preference.payerId}
              onChange={(v) => setPreference((p) => ({ ...p, payerId: v }))}
              clearable
              disabled={readOnly}
            />
          </Grid.Col>
        )}
        {preference.billToType === 3 && (
          <Grid.Col span={4}>
            <Select
              label="Contact (guarantor)"
              placeholder="Select a contact"
              data={contactOptions}
              value={preference.userContactId}
              onChange={(v) => setPreference((p) => ({ ...p, userContactId: v }))}
              disabled={readOnly}
            />
          </Grid.Col>
        )}
        <Grid.Col span={4}>
          <Group mt={24}>
            <Button onClick={savePref} disabled={readOnly}>
              Save
            </Button>
          </Group>
        </Grid.Col>
      </Grid>

      {showOrgPayer && (
        <>
          <Text weight={600} size="sm" mt="lg" mb="xs">
            Payer coverage (policy & effective dates)
          </Text>
          <Text size="sm" color="dimmed" mb="sm">
            Shown only when &quot;Send invoices to&quot; is organization payer. Add one or more coverage rows
            (subscriber / policy, dates). Payers are maintained under Organization → Payers.
          </Text>
          <Grid>
            <Grid.Col span={4}>
              <Select
                label="Payer for this row"
                data={payerOptions}
                value={cov.payerId}
                onChange={(v) => setCov((c) => ({ ...c, payerId: v }))}
                clearable
                disabled={readOnly}
              />
            </Grid.Col>
            <Grid.Col span={3}>
              <TextInput
                type="date"
                label="Effective from"
                value={cov.effectiveFrom}
                onChange={(e) => setCov((c) => ({ ...c, effectiveFrom: e.target.value }))}
                disabled={readOnly}
              />
            </Grid.Col>
            <Grid.Col span={3}>
              <TextInput
                type="date"
                label="Effective to"
                value={cov.effectiveTo}
                onChange={(e) => setCov((c) => ({ ...c, effectiveTo: e.target.value }))}
                disabled={readOnly}
              />
            </Grid.Col>
          </Grid>
          <Grid>
            <Grid.Col span={4}>
              <TextInput
                label="Member / subscriber #"
                value={cov.memberNumber}
                onChange={(e) => setCov((c) => ({ ...c, memberNumber: e.target.value }))}
                disabled={readOnly}
              />
            </Grid.Col>
            <Grid.Col span={4}>
              <TextInput
                label="Policy #"
                value={cov.policyNumber}
                onChange={(e) => setCov((c) => ({ ...c, policyNumber: e.target.value }))}
                disabled={readOnly}
              />
            </Grid.Col>
            <Grid.Col span={4}>
              <Switch
                label="Use as default bill-to when no preference row (fallback)"
                mt={24}
                checked={cov.isDefault}
                onChange={(e) => setCov((c) => ({ ...c, isDefault: e.currentTarget.checked }))}
                disabled={readOnly}
              />
            </Grid.Col>
          </Grid>
          <Button onClick={addCoverage} disabled={readOnly} my="sm">
            Add coverage row
          </Button>

          <Table striped withBorder withColumnBorders mt="sm">
            <thead>
              <tr>
                <th>Payer</th>
                <th>From</th>
                <th>To</th>
                <th>Member #</th>
                <th>Policy #</th>
                <th>Default</th>
              </tr>
            </thead>
            <tbody>
              {coverage.length === 0 ? (
                <tr>
                  <td colSpan={6}>
                    <Text color="dimmed" size="sm">
                      No coverage rows yet.
                    </Text>
                  </td>
                </tr>
              ) : (
                coverage.map((row) => (
                  <tr key={row.id}>
                    <td>{row.payerLegalName || row.payerId}</td>
                    <td>{row.effectiveFrom ? new Date(row.effectiveFrom).toLocaleDateString() : "—"}</td>
                    <td>{row.effectiveTo ? new Date(row.effectiveTo).toLocaleDateString() : "—"}</td>
                    <td>{row.memberNumber || "—"}</td>
                    <td>{row.policyNumber || "—"}</td>
                    <td>{row.isDefaultBillTo ? "Yes" : "No"}</td>
                  </tr>
                ))
              )}
            </tbody>
          </Table>
        </>
      )}
    </ProfileTabPanel>
  );
}
