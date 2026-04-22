const { UserRepository } = require('../interfaces');
const { randomUUID } = require('crypto');

class UserRepositoryMemory extends UserRepository {
  constructor() {
    super();
    this.users = new Map();
  }

  async create(user) {
    const id = randomUUID();
    const record = { id, ...user, createdAt: new Date().toISOString() };
    this.users.set(id, record);
    return record;
  }

  async getById(id) {
    return this.users.get(id) || null;
  }

  async getByName(name) {
    for (const u of this.users.values()) {
      if (u.name === name) return u;
    }
    return null;
  }
}

module.exports = UserRepositoryMemory;
