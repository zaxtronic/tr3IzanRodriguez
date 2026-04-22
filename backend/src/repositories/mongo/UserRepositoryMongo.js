const { UserRepository } = require('../interfaces');
const { UserModel } = require('./models');

class UserRepositoryMongo extends UserRepository {
  async create(user) {
    const doc = await UserModel.create(user);
    return { id: doc._id.toString(), name: doc.name, createdAt: doc.createdAt };
  }

  async getById(id) {
    const doc = await UserModel.findById(id);
    if (!doc) return null;
    return { id: doc._id.toString(), name: doc.name, createdAt: doc.createdAt };
  }

  async getByName(name) {
    const doc = await UserModel.findOne({ name });
    if (!doc) return null;
    return { id: doc._id.toString(), name: doc.name, createdAt: doc.createdAt };
  }
}

module.exports = UserRepositoryMongo;
