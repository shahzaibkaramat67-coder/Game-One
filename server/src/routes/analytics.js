const express = require('express');

const router = express.Router();

router.post('/track', (req, res) => {
  const body = req.body || {};
  const eventName = body.eventName || body.event;
  const userId = body.userId;
  const propertiesJson = body.propertiesJson || body.properties;

  if (!eventName) return res.status(400).json({ error: 'Missing eventName' });

  console.log(`[ANALYTICS] ${eventName} | User: ${userId || 'unknown'} | Props:`, propertiesJson);

  if (['install', 'first_purchase', 'retention_d1', 'uninstall'].includes(eventName)) {
    console.log(`!!! CRITICAL EVENT: ${eventName}`);
  }

  res.json({ logged: true });
});

router.post('/batch', (req, res) => {
  const { events } = req.body || {};
  if (!Array.isArray(events)) {
    return res.status(400).json({ error: 'events must be array' });
  }

  events.forEach(e => {
    const eventName = e.eventName || e.event;
    console.log(`[ANALYTICS] ${eventName} | User: ${e.userId}`);
  });

  res.json({ count: events.length });
});

module.exports = router;

