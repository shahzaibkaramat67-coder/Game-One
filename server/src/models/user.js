const { getDb } = require('../db');

async function getUser(userId) {
  const db = getDb();
  return db.collection('users').findOne({ _id: userId });
}

async function createUser(user) {
  const db = getDb();
  await db.collection('users').insertOne(user);
  return user;
}

module.exports = { getUser, createUser };
