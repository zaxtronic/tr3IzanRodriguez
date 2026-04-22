class UserRepository {
  async create(user) { throw new Error('Not implemented'); }
  async getById(id) { throw new Error('Not implemented'); }
  async getByName(name) { throw new Error('Not implemented'); }
}

class GameRepository {
  async create(game) { throw new Error('Not implemented'); }
  async getById(id) { throw new Error('Not implemented'); }
  async listOpen() { throw new Error('Not implemented'); }
  async addPlayer(gameId, player) { throw new Error('Not implemented'); }
  async updateStatus(gameId, status) { throw new Error('Not implemented'); }
}

class ResultRepository {
  async create(result) { throw new Error('Not implemented'); }
  async listByGame(gameId) { throw new Error('Not implemented'); }
}

class SessionConfigRepository {
  async create(config) { throw new Error('Not implemented'); }
  async getById(id) { throw new Error('Not implemented'); }
}

module.exports = {
  UserRepository,
  GameRepository,
  ResultRepository,
  SessionConfigRepository,
};
