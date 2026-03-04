const express = require('express');
const storage = require('../storage');

const router = express.Router();

router.get('/top/:limit', async (req, res) => {
  const limit = Math.min(parseInt(req.params.limit, 10) || 10, 100);
  try {
    const top = await storage.topUsers(limit);
    res.json(top.map(u => ({
      userId: u._id,
      displayName: u.displayName,
      rating: u.rating || 0,
      wins: u.wins || 0,
      losses: u.losses || 0,
    })));
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.get('/rank/:userId', async (req, res) => {
  try {
    const userId = req.params.userId;
    const user = await storage.ensureUser(userId);
    const rating = user.rating || 0;
    const higher = await storage.countHigherRatingThan(rating);
    res.json({
      userId,
      rank: higher + 1,
      rating,
      wins: user.wins || 0,
      losses: user.losses || 0,
    });
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

router.post('/update-rating', async (req, res) => {
  const { userId, opponent, won } = req.body || {};
  if (!userId || opponent === undefined || won === undefined) {
    return res.status(400).json({ error: 'Missing fields' });
  }

  try {
    const ratingChange = won ? 25 : -10;
    const updated = await storage.incUserFields(userId, {
      rating: ratingChange,
      wins: won ? 1 : 0,
      losses: won ? 0 : 1,
    });

    res.json({ success: true, ratingChange, rating: updated.rating || 0 });
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

module.exports = router;

