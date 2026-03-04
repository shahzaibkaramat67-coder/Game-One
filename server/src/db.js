const { MongoClient } = require('mongodb');
const dotenv = require('dotenv');
dotenv.config();

const uri = process.env.MONGODB_URI || 'mongodb://127.0.0.1:27017/duelrush';
let client;
let db;

async function connect() {
  if (db) return db;
  const serverSelectionTimeoutMS = parseInt(process.env.MONGO_TIMEOUT_MS || '2000', 10);
  client = new MongoClient(uri, { serverSelectionTimeoutMS });
  await client.connect();
  db = client.db();
  console.log('MongoDB connected to', uri);
  return db;
}

function getDb() {
  if (!db) throw new Error('DB not connected. Call connect() first.');
  return db;
}

module.exports = { connect, getDb };
