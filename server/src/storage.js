const { getDb } = require('./db');

function tryGetDb() {
  try {
    return getDb();
  } catch {
    return null;
  }
}

const memoryUsers = new Map();

function getMemoryUser(userId) {
  if (!memoryUsers.has(userId)) {
    memoryUsers.set(userId, {
      _id: userId,
      displayName: userId,
      rating: 0,
      wins: 0,
      losses: 0,
      coins: 0,
      gems: 0,
    });
  }
  return memoryUsers.get(userId);
}

async function getUser(userId) {
  const db = tryGetDb();
  if (db) return db.collection('users').findOne({ _id: userId });
  return memoryUsers.get(userId) || null;
}

async function ensureUser(userId) {
  const db = tryGetDb();
  if (db) {
    await db.collection('users').updateOne(
      { _id: userId },
      { $setOnInsert: { _id: userId, displayName: userId, rating: 0, wins: 0, losses: 0, coins: 0, gems: 0 } },
      { upsert: true }
    );
    return db.collection('users').findOne({ _id: userId });
  }
  return getMemoryUser(userId);
}

async function incUserFields(userId, inc) {
  const db = tryGetDb();
  if (db) {
    await ensureUser(userId);
    await db.collection('users').updateOne({ _id: userId }, { $inc: inc });
    return db.collection('users').findOne({ _id: userId });
  }

  const user = getMemoryUser(userId);
  for (const [k, v] of Object.entries(inc)) {
    user[k] = (user[k] || 0) + v;
  }
  return user;
}

async function topUsers(limit) {
  const db = tryGetDb();
  if (db) {
    return db.collection('users')
      .find({}, { projection: { _id: 1, displayName: 1, rating: 1, wins: 1, losses: 1 } })
      .sort({ rating: -1 })
      .limit(limit)
      .toArray();
  }

  return [...memoryUsers.values()]
    .sort((a, b) => (b.rating || 0) - (a.rating || 0))
    .slice(0, limit)
    .map(u => ({ _id: u._id, displayName: u.displayName, rating: u.rating, wins: u.wins, losses: u.losses }));
}

async function countHigherRatingThan(rating) {
  const db = tryGetDb();
  if (db) return db.collection('users').countDocuments({ rating: { $gt: rating } });

  let count = 0;
  for (const u of memoryUsers.values()) {
    if ((u.rating || 0) > rating) count++;
  }
  return count;
}

module.exports = {
  tryGetDb,
  getUser,
  ensureUser,
  incUserFields,
  topUsers,
  countHigherRatingThan,
};

