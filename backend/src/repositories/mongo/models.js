const { mongoose } = require('./connection');

const UserSchema = new mongoose.Schema({
  name: { type: String, required: true, unique: true },
  passwordHash: { type: String, default: '' },
  createdAt: { type: Date, default: Date.now },
});

const GameSchema = new mongoose.Schema({
  hostUserId: { type: String, required: true },
  status: { type: String, default: 'open' },
  maxPlayers: { type: Number, default: 2 },
  mode: { type: String, default: 'realtime' },
  players: { type: Array, default: [] },
  createdAt: { type: Date, default: Date.now },
});

const ResultSchema = new mongoose.Schema({
  gameId: { type: String, required: true },
  data: { type: Object, default: {} },
  createdAt: { type: Date, default: Date.now },
});

const SessionConfigSchema = new mongoose.Schema({
  name: { type: String },
  maxPlayers: { type: Number, default: 2 },
  mode: { type: String, default: 'realtime' },
  createdAt: { type: Date, default: Date.now },
});

const UserModel = mongoose.model('User', UserSchema);
const GameModel = mongoose.model('Game', GameSchema);
const ResultModel = mongoose.model('Result', ResultSchema);
const SessionConfigModel = mongoose.model('SessionConfig', SessionConfigSchema);

module.exports = { UserModel, GameModel, ResultModel, SessionConfigModel };
