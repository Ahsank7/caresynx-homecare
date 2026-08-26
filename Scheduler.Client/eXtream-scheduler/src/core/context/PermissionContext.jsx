import React, { createContext, useContext, useState, useEffect } from 'react';
import { rolePermissionService } from 'core/services';
import { localStoreService } from 'core/services';

const PermissionContext = createContext();

/** Normalize API booleans (camelCase or PascalCase). */
const toBool = (v) =>
  v === true ||
  v === 1 ||
  v === '1' ||
  (typeof v === 'string' && v.toLowerCase() === 'true');

/**
 * Build menuId -> flags map. Handles PascalCase props and duplicate MenuId rows (AND merge).
 */
const buildPermissionMapFromResponse = (response) => {
  const raw = Array.isArray(response)
    ? response
    : response?.data && Array.isArray(response.data)
      ? response.data
      : null;
  if (!raw || !Array.isArray(raw)) {
    return {};
  }
  const permissionMap = {};
  raw.forEach((permission) => {
    const id = permission.menuId ?? permission.MenuId;
    if (!id) return;
    const next = {
      canView: toBool(permission.canView ?? permission.CanView),
      canCreate: toBool(permission.canCreate ?? permission.CanCreate),
      canEdit: toBool(permission.canEdit ?? permission.CanEdit),
      canDelete: toBool(permission.canDelete ?? permission.CanDelete),
    };
    if (permissionMap[id]) {
      permissionMap[id] = {
        canView: permissionMap[id].canView && next.canView,
        canCreate: permissionMap[id].canCreate && next.canCreate,
        canEdit: permissionMap[id].canEdit && next.canEdit,
        canDelete: permissionMap[id].canDelete && next.canDelete,
      };
    } else {
      permissionMap[id] = next;
    }
  });
  return permissionMap;
};

export const usePermissions = () => {
  const context = useContext(PermissionContext);
  if (!context) {
    throw new Error('usePermissions must be used within a PermissionProvider');
  }
  return context;
};

export const PermissionProvider = ({ children }) => {
  const [permissions, setPermissions] = useState({});
  /** enforce: staff — require explicit allow; allowAll: non-staff or failed parse */
  const [permissionMode, setPermissionMode] = useState('allowAll');
  const [loading, setLoading] = useState(false);
  const [initialized, setInitialized] = useState(false);

  const loadUserPermissions = async () => {
    try {
      // Check if user is logged in first
      const token = localStoreService.getToken();
      if (!token) {
        // No token means user is not logged in, skip permission loading
        console.log('No token found, skipping permission loading');
        setPermissions({});
        setPermissionMode('allowAll');
        setInitialized(true);
        return;
      }

      setLoading(true);
      
      // Safely get user info with error handling
      let userId, userType, organizationId;
      try {
        userId = localStoreService.getUserID();
        userType = localStoreService.getUserType();
        organizationId = localStoreService.getOrganizationID();
      } catch (error) {
        // Token might be invalid or expired
        console.log('Error decoding token, skipping permission loading:', error);
        setPermissions({});
        setPermissionMode('allowAll');
        setInitialized(true);
        return;
      }
      
      // Validate we have required user info
      if (!userId || !organizationId) {
        console.log('Missing user info, skipping permission loading');
        setPermissions({});
        setPermissionMode('allowAll');
        setInitialized(true);
        return;
      }
      
      console.log('Loading permissions for user:', userId, 'type:', userType, 'org:', organizationId);
      
      // Add timeout to prevent infinite loading
      const timeoutPromise = new Promise((_, reject) => 
        setTimeout(() => reject(new Error('Permission loading timeout')), 10000)
      );
      
      // Only load permissions for staff users (userType === 3)
      if (userType === '3' || userType === 3) {
        const response = await Promise.race([
          rolePermissionService.getUserMenuPermissions(userId, organizationId),
          timeoutPromise
        ]);
        console.log('User permissions response:', response);

        const permissionMap = buildPermissionMapFromResponse(response);
        setPermissions(permissionMap);
        setPermissionMode('enforce');
        console.log('Set permissions:', permissionMap);
      } else {
        // For non-staff users, set default permissions (allow all)
        setPermissions({});
        setPermissionMode('allowAll');
      }
      
      setInitialized(true);
    } catch (error) {
      console.error('Failed to load user permissions:', error);
      // Set default permissions on error to prevent blocking
      setPermissions({});
      setPermissionMode('allowAll');
      setInitialized(true);
    } finally {
      setLoading(false);
    }
  };

  const hasPermission = (menuId, permission = 'canView') => {
    if (!initialized) {
      return true;
    }
    // Non-staff or load failure: do not block navigation
    if (permissionMode !== 'enforce') {
      return true;
    }
    // Staff: menu must be present in API payload with the flag true (missing menu => deny)
    const menuPermissions = permissions[menuId];
    if (!menuPermissions) {
      return false;
    }
    return menuPermissions[permission] === true;
  };

  const canView = (menuId) => hasPermission(menuId, 'canView');
  const canCreate = (menuId) => hasPermission(menuId, 'canCreate');
  const canEdit = (menuId) => hasPermission(menuId, 'canEdit');
  const canDelete = (menuId) => hasPermission(menuId, 'canDelete');

  useEffect(() => {
    loadUserPermissions();
  }, []);

  const value = {
    permissions,
    permissionMode,
    loading,
    initialized,
    hasPermission,
    canView,
    canCreate,
    canEdit,
    canDelete,
    reloadPermissions: loadUserPermissions
  };

  return (
    <PermissionContext.Provider value={value}>
      {children}
    </PermissionContext.Provider>
  );
};