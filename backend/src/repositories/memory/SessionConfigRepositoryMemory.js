const { SessionConfigRepository } = require('../interfaces');
const { randomUUID } = require('crypto');

class SessionConfigRepositoryMemory extends SessionConfigRepository {
  constructor() {
    super();
    this.configs = new Map();
  }

  async create(config) {
    const id = randomUUID();
    const record = { id, createdAt: new Date().toISOString(), ...config };
    this.configs.set(id, record);
    return record;
  }

  async getById(id) {
    return this.configs.get(id) || null;
  }
}

module.exports = SessionConfigRepositoryMemory;
