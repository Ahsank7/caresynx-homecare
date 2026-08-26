import { useCallback, useEffect, useState } from "react";
import {
  Button,
  Grid,
  TextInput,
  LoadingOverlay,
  Table,
  Text,
  Group,
  Paper,
  Modal,
  NumberInput,
} from "@mantine/core";
import {
  clientPayerService,
  handleApiError,
  showSuccessNotification,
  showErrorNotification,
} from "core/services";

/**
 * Organization-level payers: list in a grid and add/edit billing parties (insurers, government, etc.).
 */
export default function OrganizationPayers({ organizationId }) {
  const [loading, setLoading] = useState(true);
  const [payers, setPayers] = useState([]);
  const [form, setForm] = useState({
    id: null,
    legalName: "",
    billingEmail: "",
  });

  const [cardModalOpen, setCardModalOpen] = useState(false);
  const [cardPayer, setCardPayer] = useState(null);
  const [cardLoading, setCardLoading] = useState(false);
  const [cardForm, setCardForm] = useState({
    cardId: null,
    cardHolderName: "",
    cardNumber: "",
    cvv: "",
    expiryMonth: 12,
    expiryYear: new Date().getFullYear() + 2,
    typeId: 1,
  });

  const load = useCallback(async () => {
    if (!organizationId) return;
    setLoading(true);
    try {
      const p = await clientPayerService.getPayers();
      setPayers(Array.isArray(p) ? p : []);
    } catch (e) {
      handleApiError(e, "Failed to load payers");
    } finally {
      setLoading(false);
    }
  }, [organizationId]);

  useEffect(() => {
    load();
  }, [load]);

  const resetForm = () => setForm({ id: null, legalName: "", billingEmail: "" });

  const save = async () => {
    if (!form.legalName?.trim()) {
      showErrorNotification("Legal name is required");
      return;
    }
    try {
      await clientPayerService.savePayer({
        id: form.id || undefined,
        organizationId,
        legalName: form.legalName.trim(),
        payerType: 0,
        billingEmail: form.billingEmail?.trim() || null,
        isActive: true,
      });
      showSuccessNotification(form.id ? "Payer updated" : "Payer added");
      resetForm();
      load();
    } catch (e) {
      handleApiError(e, "Failed to save payer");
    }
  };

  const startEdit = (row) => {
    setForm({
      id: row.id,
      legalName: row.legalName || "",
      billingEmail: row.billingEmail || "",
    });
  };

  const openCardModal = async (row) => {
    setCardPayer(row);
    setCardModalOpen(true);
    setCardLoading(true);
    setCardForm({
      cardId: null,
      cardHolderName: "",
      cardNumber: "",
      cvv: "",
      expiryMonth: 12,
      expiryYear: new Date().getFullYear() + 2,
      typeId: 1,
    });
    try {
      const data = await clientPayerService.getPayerCard(row.id);
      if (data && data.cardId) {
        setCardForm((f) => ({
          ...f,
          cardId: data.cardId,
          cardHolderName: data.cardHolderName || "",
          expiryMonth: data.expiryMonth || 12,
          expiryYear: data.expiryYear || f.expiryYear,
          typeId: data.typeId || 1,
          cardNumber: "",
          cvv: "",
        }));
      }
    } catch (e) {
      handleApiError(e, "Failed to load payer card");
    } finally {
      setCardLoading(false);
    }
  };

  const saveCard = async () => {
    if (!cardPayer) return;
    if (!cardForm.cardHolderName?.trim()) {
      showErrorNotification("Card holder name is required");
      return;
    }
    if (!cardForm.cardId) {
      if (!cardForm.cardNumber?.trim()) {
        showErrorNotification("Card number is required");
        return;
      }
      if (!cardForm.cvv?.trim()) {
        showErrorNotification("CVV is required for new cards");
        return;
      }
    }
    try {
      const body = {
        organizationId,
        payerId: cardPayer.id,
        cardId: cardForm.cardId || undefined,
        cardHolderName: cardForm.cardHolderName.trim(),
        expiryMonth: cardForm.expiryMonth,
        expiryYear: cardForm.expiryYear,
        typeId: cardForm.typeId,
      };
      if (cardForm.cardNumber?.trim()) body.cardNumber = cardForm.cardNumber.trim();
      if (cardForm.cvv?.trim()) body.cvv = cardForm.cvv.trim();

      await clientPayerService.savePayerCard(body);
      showSuccessNotification("Payer payment method saved");
      setCardModalOpen(false);
      setCardPayer(null);
    } catch (e) {
      handleApiError(e, "Failed to save payer card");
    }
  };

  return (
    <Paper withBorder p="md">
      <LoadingOverlay visible={loading} />
      <Text size="sm" color="dimmed" mb="md">
        Add payers once here; they can then be linked to clients under each client&apos;s invoice preferences
        (coverage and funding). For invoices billed to an organization payer, store a payment method here for
        auto-charge; otherwise those invoices stay unpaid until manual collection.
      </Text>

      <Text weight={600} size="sm" mb="xs">
        {form.id ? "Edit payer" : "Add payer"}
      </Text>
      <Grid>
        <Grid.Col span={4}>
          <TextInput
            label="Legal name"
            value={form.legalName}
            onChange={(e) => setForm((f) => ({ ...f, legalName: e.target.value }))}
            required
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <TextInput
            label="Billing email"
            type="email"
            value={form.billingEmail}
            onChange={(e) => setForm((f) => ({ ...f, billingEmail: e.target.value }))}
          />
        </Grid.Col>
        <Grid.Col span={4}>
          <Group mt={28}>
            <Button onClick={save}>{form.id ? "Save changes" : "Add payer"}</Button>
            {form.id && (
              <Button variant="default" onClick={resetForm}>
                Cancel edit
              </Button>
            )}
          </Group>
        </Grid.Col>
      </Grid>

      <Text weight={600} size="sm" mt="xl" mb="xs">
        Payers in this organization
      </Text>
      <Table striped highlightOnHover withBorder withColumnBorders>
        <thead>
          <tr>
            <th>Legal name</th>
            <th>Billing email</th>
            <th style={{ width: 220 }}> </th>
          </tr>
        </thead>
        <tbody>
          {payers.length === 0 ? (
            <tr>
              <td colSpan={3}>
                <Text color="dimmed" size="sm">
                  No payers yet.
                </Text>
              </td>
            </tr>
          ) : (
            payers.map((row) => (
              <tr key={row.id}>
                <td>{row.legalName}</td>
                <td>{row.billingEmail || "—"}</td>
                <td>
                  <Group spacing="xs">
                    <Button size="xs" variant="light" onClick={() => startEdit(row)}>
                      Edit
                    </Button>
                    <Button size="xs" variant="outline" onClick={() => openCardModal(row)}>
                      Payment method
                    </Button>
                  </Group>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </Table>

      <Modal
        opened={cardModalOpen}
        onClose={() => {
          setCardModalOpen(false);
          setCardPayer(null);
        }}
        title={cardPayer ? `Payment method — ${cardPayer.legalName}` : "Payment method"}
        size="md"
      >
        <LoadingOverlay visible={cardLoading} />
        <Text size="sm" color="dimmed" mb="md">
          Used when a client&apos;s bill-to is this organization payer. Card data is encrypted like client cards.
        </Text>
        <Grid>
          <Grid.Col span={12}>
            <TextInput
              label="Cardholder name"
              value={cardForm.cardHolderName}
              onChange={(e) => setCardForm((f) => ({ ...f, cardHolderName: e.target.value }))}
              required
            />
          </Grid.Col>
          <Grid.Col span={12}>
            <TextInput
              label={cardForm.cardId ? "Card number (leave blank to keep existing)" : "Card number"}
              value={cardForm.cardNumber}
              onChange={(e) => setCardForm((f) => ({ ...f, cardNumber: e.target.value }))}
            />
          </Grid.Col>
          <Grid.Col span={6}>
            <NumberInput
              label="Expiry month"
              min={1}
              max={12}
              value={cardForm.expiryMonth}
              onChange={(v) => setCardForm((f) => ({ ...f, expiryMonth: v ?? 1 }))}
            />
          </Grid.Col>
          <Grid.Col span={6}>
            <NumberInput
              label="Expiry year"
              min={new Date().getFullYear()}
              max={new Date().getFullYear() + 20}
              value={cardForm.expiryYear}
              onChange={(v) => setCardForm((f) => ({ ...f, expiryYear: v ?? f.expiryYear }))}
            />
          </Grid.Col>
          <Grid.Col span={12}>
            <TextInput
              label={cardForm.cardId ? "CVV (required if changing card)" : "CVV"}
              value={cardForm.cvv}
              onChange={(e) => setCardForm((f) => ({ ...f, cvv: e.target.value }))}
            />
          </Grid.Col>
          <Grid.Col span={12}>
            <Group position="right">
              <Button variant="default" onClick={() => setCardModalOpen(false)}>
                Cancel
              </Button>
              <Button onClick={saveCard}>Save</Button>
            </Group>
          </Grid.Col>
        </Grid>
      </Modal>
    </Paper>
  );
}
