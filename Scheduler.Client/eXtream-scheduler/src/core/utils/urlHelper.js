import environment from "../../enviroment";

const PUBLIC_FOLDERS = [
  "ProfileImages",
  "OrganizationLogos",
  "UserDocument",
  "Invoices",
];

const getApiOrigin = () => {
  const base = environment.baseURL || "";
  if (base.startsWith("http://") || base.startsWith("https://")) {
    try {
      return new URL(base).origin;
    } catch {
      return base.replace(/\/api\/?$/i, "").replace(/\/$/, "");
    }
  }
  return "";
};

const toPublicWebPath = (filePath) => {
  if (!filePath) return "";

  const normalized = String(filePath).replace(/\\/g, "/");

  for (const folder of PUBLIC_FOLDERS) {
    const token = `/${folder}/`;
    const lower = normalized.toLowerCase();
    const index = lower.indexOf(token.toLowerCase());
    if (index >= 0) {
      return `/${normalized.substring(index + 1)}`;
    }
    if (lower.startsWith(`${folder.toLowerCase()}/`)) {
      return `/${normalized}`;
    }
  }

  return normalized.startsWith("/") ? normalized : `/${normalized}`;
};

/**
 * Builds a complete URL for document access
 * @param {string} documentPath - The document path from the API
 * @returns {string} - Complete URL for document access
 */
export const buildDocumentUrl = (documentPath) => {
  if (!documentPath) return "";

  if (documentPath.startsWith("http://") || documentPath.startsWith("https://")) {
    return documentPath;
  }

  const origin = getApiOrigin();
  const webPath = toPublicWebPath(documentPath);
  return `${origin}${webPath}`;
};

/**
 * Builds a complete URL for image access
 * @param {string} imagePath - The image path from the API
 * @returns {string} - Complete URL for image access
 */
export const buildImageUrl = (imagePath) => {
  return buildDocumentUrl(imagePath);
};

/**
 * Builds a secure URL for image access (alias for buildImageUrl)
 * @param {string} imagePath - The image path from the API
 * @returns {string} - Complete URL for image access
 */
export const buildSecureImageUrl = (imagePath) => {
  return buildImageUrl(imagePath);
};

/**
 * Builds a complete URL for file access
 * @param {string} filePath - The file path from the API
 * @returns {string} - Complete URL for file access
 */
export const buildFileUrl = (filePath) => {
  return buildDocumentUrl(filePath);
};

/**
 * Checks if a URL is complete (starts with http/https)
 * @param {string} url - The URL to check
 * @returns {boolean} - True if the URL is complete
 */
export const isCompleteUrl = (url) => {
  if (!url) return false;
  return url.startsWith("http://") || url.startsWith("https://");
};

/**
 * Extracts filename from a URL or path
 * @param {string} url - The URL or path
 * @returns {string} - The filename
 */
export const getFilenameFromUrl = (url) => {
  if (!url) return "";

  try {
    const urlObj = new URL(url);
    const pathname = urlObj.pathname;
    return pathname.split("/").pop() || "";
  } catch {
    return url.split("/").pop() || "";
  }
};

/**
 * Gets file extension from URL or path
 * @param {string} url - The URL or path
 * @returns {string} - The file extension (without dot)
 */
export const getFileExtension = (url) => {
  const filename = getFilenameFromUrl(url);
  const lastDotIndex = filename.lastIndexOf(".");
  return lastDotIndex > 0 ? filename.substring(lastDotIndex + 1).toLowerCase() : "";
};

/**
 * Checks if a file is an image based on its extension
 * @param {string} url - The URL or path
 * @returns {boolean} - True if the file is an image
 */
export const isImageFile = (url) => {
  const imageExtensions = ["jpg", "jpeg", "png", "gif", "bmp", "webp", "svg"];
  const extension = getFileExtension(url);
  return imageExtensions.includes(extension);
};
