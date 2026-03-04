const express = require('express');
const storage = require('../storage');

const router = express.Router();

router.post('/purchase-receipt', async (req, res) => {
  const { userId, productId, purchaseToken } = req.body || {};

  if (!userId || !productId || !purchaseToken) {
    return res.status(400).json({ error: 'Missing required fields' });
  }

  try {
    await storage.ensureUser(userId);

    // TODO: Verify purchaseToken with Google Play Billing API.
    // For now, grant goods based on productId.
    let gemsToAdd = 0;
    switch (productId) {
      case 'gems_100': gemsToAdd = 100; break;
      case 'gems_500': gemsToAdd = 500; break;
      case 'gems_1000': gemsToAdd = 1000; break;
      default: gemsToAdd = 0; break;
    }

    if (gemsToAdd > 0) {
      await storage.incUserFields(userId, { gems: gemsToAdd });
    }

    res.json({ success: true, productId, gemsGranted: gemsToAdd });
  } catch (e) {
    console.error('Purchase error:', e);
    res.status(500).json({ error: e.message });
  }
});

router.get('/user/:userId/currency', async (req, res) => {
  try {
    const user = await storage.ensureUser(req.params.userId);
    res.json({ coins: user.coins || 0, gems: user.gems || 0 });
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

module.exports = router;

