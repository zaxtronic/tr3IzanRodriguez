async function saveResult(req, res) {
  const { gameId, data } = req.body;
  try {
    const result = await req.services.resultService.saveResult(gameId, data);
    res.json(result);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

async function listResults(req, res) {
  const { gameId } = req.params;
  try {
    const results = await req.services.resultService.listByGame(gameId);
    res.json(results);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

module.exports = { saveResult, listResults };
