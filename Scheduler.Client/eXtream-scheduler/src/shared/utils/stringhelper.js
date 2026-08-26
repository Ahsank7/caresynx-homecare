const removeSpaceFromString = (value) => {
  if (!value) return "";

  return value.replace(/\s/g, "");
};

/**
 * Matches Scheduler.API Services.Franchise.FranchiseRepository.CreateFranchiseAdminUserAsync:
 * spaces removed (ASCII space only), lowercased, then "admin" / "admin1234" suffixes.
 */
const getFranchiseDefaultAdminCredentials = (franchiseName) => {
  if (!franchiseName) return { username: "", password: "" };
  const sanitized = String(franchiseName).replace(/ /g, "").toLowerCase();
  return {
    username: `${sanitized}admin`,
    password: `${sanitized}admin1234`,
  };
};

export { removeSpaceFromString, getFranchiseDefaultAdminCredentials };
