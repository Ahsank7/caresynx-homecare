/* eslint-env node */
// Use require.resolve so ESLint resolves config from a single path (avoids plugin conflict from path casing).
module.exports = {
  root: true,
  extends: [
    require.resolve('eslint-config-react-app'),
    require.resolve('eslint-config-react-app/jest'),
  ],
};
