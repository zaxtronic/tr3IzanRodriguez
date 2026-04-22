const { SessionConfigRepository } = require('../interfaces');
const { SessionConfigModel } = require('./models');

class SessionConfigRepositoryMongo extends SessionConfigRepository {
  async create(config) {
    const doc = await SessionConfigModel.create(config);
    return { id: doc._id.toString(), name: doc.name, maxPlayers: doc.maxPlayers, mode: doc.mode, createdAt: doc.createdAt };
  }

  async getById(id) {
    const doc = await SessionConfigModel.findById(id);
    if (!doc) return null;
    return { id: doc._id.toString(), name: doc.name, maxPlayers: doc.maxPlayers, mode: doc.mode, createdAt: doc.createdAt };
  }
}

module.exports = SessionConfigRepositoryMongo;
