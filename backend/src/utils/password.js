const bcrypt = require('bcryptjs');

const SALT_ROUNDS = 10;

async function hashPassword(plain) {
  if (typeof plain !== 'string' || plain.length < 4) {
    throw new Error('invalid password');
  }
  return bcrypt.hash(plain, SALT_ROUNDS);
}

async function verifyPassword(plain, hash) {
  if (!hash) return false;
  return bcrypt.compare(plain, hash);
}

module.exports = { hashPassword, verifyPassword };
