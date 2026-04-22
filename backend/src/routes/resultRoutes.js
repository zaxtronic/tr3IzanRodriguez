const express = require('express');
const { saveResult, listResults } = require('../controllers/resultController');

const router = express.Router();
router.post('/', saveResult);
router.get('/:gameId', listResults);

module.exports = router;
