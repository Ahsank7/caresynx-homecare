/** Rules for new/changed credential passwords (admin Credentials tab). */
export const CREDENTIAL_PASSWORD_REQUIREMENT_TEXT =
  "At least 6 characters, one uppercase letter, one number, and one special character.";

/**
 * @param {string} password
 * @returns {{ ok: boolean, message?: string }}
 */
export function validateNewCredentialPassword(password) {
  if (!password || password.length < 6) {
    return {
      ok: false,
      message: `Password is too short. ${CREDENTIAL_PASSWORD_REQUIREMENT_TEXT}`,
    };
  }
  if (!/[A-Z]/.test(password)) {
    return {
      ok: false,
      message: `Add an uppercase letter. ${CREDENTIAL_PASSWORD_REQUIREMENT_TEXT}`,
    };
  }
  if (!/\d/.test(password)) {
    return {
      ok: false,
      message: `Add a number. ${CREDENTIAL_PASSWORD_REQUIREMENT_TEXT}`,
    };
  }
  if (!/[^A-Za-z0-9]/.test(password)) {
    return {
      ok: false,
      message: `Add a special character. ${CREDENTIAL_PASSWORD_REQUIREMENT_TEXT}`,
    };
  }
  return { ok: true };
}
