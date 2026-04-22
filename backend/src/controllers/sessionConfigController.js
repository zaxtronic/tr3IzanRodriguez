async function createSessionConfig(req, res) {
  try {
    const config = await req.services.sessionConfigService.createConfig(req.body || {});
    res.json(config);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

async function getSessionConfig(req, res) {
  const { id } = req.params;
  try {
    const config = await req.services.sessionConfigService.getById(id);
    if (!config) return res.status(404).json({ error: 'Config not found' });
    res.json(config);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

module.exports = { createSessionConfig, getSessionConfig };
