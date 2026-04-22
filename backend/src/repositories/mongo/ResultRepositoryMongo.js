const { ResultRepository } = require('../interfaces');
const { ResultModel } = require('./models');

class ResultRepositoryMongo extends ResultRepository {
  async create(result) {
    const doc = await ResultModel.create(result);
    return { id: doc._id.toString(), gameId: doc.gameId, data: doc.data, createdAt: doc.createdAt };
  }

  async listByGame(gameId) {
    const docs = await ResultModel.find({ gameId });
    return docs.map(doc => ({ id: doc._id.toString(), gameId: doc.gameId, data: doc.data, createdAt: doc.createdAt }));
  }
}

module.exports = ResultRepositoryMongo;
