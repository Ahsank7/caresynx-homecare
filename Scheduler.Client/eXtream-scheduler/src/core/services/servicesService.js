import { get, post, put, remove } from "./httpService";

const getServiceTypes = async (organizationId) => {
    return await get(`Services/List/ServiceType?organizationId=${organizationId}`);
};

const getServicesByType = async (serviceTypeId) => {
    return await get(`Services/List?serviceTypeId=${serviceTypeId}`);
};

/** Flat list of { value, label } for all services in an org (for selects). API lists services per service type. */
const getAllServicesForOrganization = async (organizationId) => {
    const stRes = await getServiceTypes(organizationId);
    let types = stRes?.result || stRes?.response;
    if (!Array.isArray(types)) types = Array.isArray(stRes) ? stRes : [];
    if (!Array.isArray(types) || types.length === 0) {
        return [{ value: "", label: "All services" }];
    }
    const out = [];
    for (const t of types) {
        const typeId = t.id ?? t.Id;
        const typeName = t.name || t.Name || "Service type";
        if (typeId == null) continue;
        const listRes = await getServicesByType(typeId);
        let arr = listRes?.response;
        if (!Array.isArray(arr)) arr = listRes?.result;
        if (!Array.isArray(arr)) arr = Array.isArray(listRes) ? listRes : [];
        for (const s of arr) {
            const sid = s.id ?? s.Id;
            const sname = s.name || s.Name || String(sid);
            if (sid != null) {
                out.push({ value: String(sid), label: `${typeName} — ${sname}` });
            }
        }
    }
    return [{ value: "", label: "All services" }, ...out];
};

const addServiceType = async (serviceTypeObj) => {
    return await post("Services/ServiceType", serviceTypeObj);
};

const updateServiceType = async (serviceTypeObj) => {
    return await put("Services/ServiceType", serviceTypeObj);
};

const deleteServiceType = async (serviceTypeId) => {
    return await remove(`Services/ServiceType/${serviceTypeId}`);
};

const addService = async (serviceObj) => {
    return await post("Services", serviceObj);
};

const updateService = async (serviceTypeObj) => {
    return await put("Services", serviceTypeObj);
};

const deleteService = async (serviceId) => {
    return await remove(`Services/${serviceId}`);
};

const getServicesList = async (params) => {
    const queryParams = new URLSearchParams();
    if (params.organizationId) queryParams.append('organizationId', params.organizationId);
    if (params.serviceTypeId) queryParams.append('serviceTypeId', params.serviceTypeId);
    
    const queryString = queryParams.toString();
    const url = queryString ? `Services/List?${queryString}` : 'Services/List';
    return await get(url);
};

export { getServiceTypes, getServicesByType, getAllServicesForOrganization, addServiceType, updateServiceType, deleteServiceType, addService, updateService, deleteService, getServicesList };
