// Server-side Play Billing verification STUB
// Implement using Google Play Developer API and a service account.

async function verifyPurchase(packageName, productId, purchaseToken) {
  // TODO: Exchange this token with Play Developer API to verify purchase state
  // This function must validate signature, purchaseState, and consumption.
  // Use googleapis or direct HTTP calls with service account credentials.
  console.warn('verifyPurchase called — implement Play Developer API integration');
  return { valid: false, reason: 'not_implemented' };
}

module.exports = { verifyPurchase };
