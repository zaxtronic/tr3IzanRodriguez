const assert = require('assert');
const UserRepositoryMemory = require('../src/repositories/memory/UserRepositoryMemory');
const GameRepositoryMemory = require('../src/repositories/memory/GameRepositoryMemory');
const ResultRepositoryMemory = require('../src/repositories/memory/ResultRepositoryMemory');
const SessionConfigRepositoryMemory = require('../src/repositories/memory/SessionConfigRepositoryMemory');

async function testUserRepo() {
  const repo = new UserRepositoryMemory();
  const u = await repo.create({ name: 'Alice' });
  assert.ok(u.id);
  assert.equal(u.name, 'Alice');

  const byId = await repo.getById(u.id);
  assert.equal(byId.id, u.id);

  const byName = await repo.getByName('Alice');
  assert.equal(byName.id, u.id);
}

async function testGameRepo() {
  const repo = new GameRepositoryMemory();
  const g = await repo.create({ hostUserId: 'u1', maxPlayers: 2, mode: 'realtime' });
  assert.ok(g.id);
  assert.equal(g.status, 'open');

  const open = await repo.listOpen();
  assert.ok(open.find(x => x.id === g.id));

  const joined = await repo.addPlayer(g.id, { userId: 'u1', name: 'Alice' });
  assert.equal(joined.players.length, 1);

  const updated = await repo.updateStatus(g.id, 'running');
  assert.equal(updated.status, 'running');
}

async function testResultRepo() {
  const repo = new ResultRepositoryMemory();
  const r = await repo.create({ gameId: 'g1', data: { score: 10 } });
  assert.ok(r.id);
  const list = await repo.listByGame('g1');
  assert.equal(list.length, 1);
}

async function testSessionConfigRepo() {
  const repo = new SessionConfigRepositoryMemory();
  const c = await repo.create({ name: 'Test', maxPlayers: 2, mode: 'realtime' });
  assert.ok(c.id);
  const byId = await repo.getById(c.id);
  assert.equal(byId.id, c.id);
}

(async () => {
  await testUserRepo();
  await testGameRepo();
  await testResultRepo();
  await testSessionConfigRepo();
  console.log('Repository tests OK');
})().catch(err => {
  console.error(err);
  process.exit(1);
});
