async function createGame(req, res) {
  try {
    const game = await req.services.gameService.createGame(req.body);
    res.json(game);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

async function listGames(req, res) {
  try {
    const games = await req.services.gameService.listOpen();
    res.json(games);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
}

async function joinGame(req, res) {
  const { id } = req.params;
  try {
    const game = await req.services.gameService.joinGame(id, req.body);
    if (!game) return res.status(404).json({ error: 'Game not found' });
    res.json(game);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

async function updateStatus(req, res) {
  const { id } = req.params;
  const { status } = req.body;
  try {
    const game = await req.services.gameService.updateStatus(id, status);
    if (!game) return res.status(404).json({ error: 'Game not found' });
    res.json(game);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

module.exports = { createGame, listGames, joinGame, updateStatus };
