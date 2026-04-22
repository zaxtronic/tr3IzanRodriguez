#!/usr/bin/env bash
set -euo pipefail

pip install -e /workspace/External/ml-agents/ml-agents-envs
pip install -e /workspace/External/ml-agents/ml-agents

mlagents-learn /workspace/ML-Agents/slime.yaml "$@"
