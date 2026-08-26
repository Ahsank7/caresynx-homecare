import { get, post, remove } from "./httpService";
import { getOrganizationID } from "./localStoreService";

const orgId = () => getOrganizationID();

const getPayers = () => get(`ClientPayer/Payers?organizationId=${orgId()}`);

const savePayer = (body) => post("ClientPayer/Payer", { ...body, organizationId: body.organizationId || orgId() });

const getCoverage = (clientId) => get(`ClientPayer/Coverage?clientId=${clientId}`);

const saveCoverage = (body) => post("ClientPayer/Coverage", body);

const getPreference = (clientId) => get(`ClientPayer/Preference?clientId=${clientId}`);

const savePreference = (body) => post("ClientPayer/Preference", body);

const getFunding = (clientId) =>
  get(`ClientPayer/Funding?clientId=${clientId}&organizationId=${orgId()}`);

const saveFunding = (body) =>
  post("ClientPayer/Funding", { ...body, organizationId: body.organizationId || orgId() });

const deleteFunding = (id) => remove(`ClientPayer/Funding/${id}?organizationId=${orgId()}`);

const getOrgFunding = () => get(`ClientPayer/OrgFunding?organizationId=${orgId()}`);

const saveOrgFunding = (body) =>
  post("ClientPayer/OrgFunding", { ...body, organizationId: body.organizationId || orgId() });

const deleteOrgFunding = (id) => remove(`ClientPayer/OrgFunding/${id}?organizationId=${orgId()}`);

const getPayerCard = (payerId) =>
  get(`ClientPayer/PayerCard?organizationId=${orgId()}&payerId=${payerId}`);

const savePayerCard = (body) =>
  post("ClientPayer/PayerCard", { ...body, organizationId: body.organizationId || orgId() });

export {
  getPayers,
  savePayer,
  getCoverage,
  saveCoverage,
  getPreference,
  savePreference,
  getFunding,
  saveFunding,
  deleteFunding,
  getOrgFunding,
  saveOrgFunding,
  deleteOrgFunding,
  getPayerCard,
  savePayerCard,
  orgId,
};
