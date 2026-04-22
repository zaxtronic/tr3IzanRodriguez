const { ResultRepository } = require('../interfaces');
const { randomUUID } = require('crypto');

class ResultRepositoryMemory extends ResultRepository {
  constructor() {
    super();
    this.results = [];
  }

  async create(result) {
    const record = { id: randomUUID(), createdAt: new Date().toISOString(), ...result };
    this.results.push(record);
    return record;
  }

  async listByGame(gameId) {
    return this.results.filter(r => r.gameId === gameId);
  }
}

module.exports = ResultRepositoryMemory;
