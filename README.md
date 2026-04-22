# Nom del projecte

`tr3IzanRodriguez`

## Integrants

- Izan Rodriguez

## Petita descripció

Projecte de videojoc 2D desenvolupat amb Unity, amb backend Node.js per funcionalitats de xarxa, serveis de joc i suport de temps real. També inclou integració amb ML-Agents per entrenament d'agents.

## Gestor de tasques

- Pendent d'afegir (Taiga/Jira/Trello): `URL`

## Prototip gràfic

- Pendent d'afegir (Penpot/Figma/Moqups): `URL`

## URL de producció

- Pendent de desplegament: `URL`

## Estat

El projecte està en desenvolupament actiu.
- Joc base i estructura principal disponibles.
- Components de xarxa (client Unity + backend) implementats.
- Integració amb entrenament ML-Agents present.
- Falta consolidar URLs de gestió/prototip i entorn de producció.

---

## 1) Resum tècnic del projecte

- Motor principal: Unity (`Assets/`, `Packages/`, `ProjectSettings/`).
- Lògica de joc: C# a `Assets/Scripts`.
- Backend: Node.js a `backend/` amb API HTTP, game server i gateway.
- Persistència backend: repositoris en memòria i MongoDB.
- IA/Entrenament: ML-Agents (`Assets/Scripts/MLAgents`, `results/`, `ML-Agents/`, `External/ml-agents`).

## 2) Estructura del repositori

### Arrel

- `Assets/`: contingut principal Unity (escenes, prefabs, scripts, recursos).
- `Packages/`: dependències de Unity Package Manager.
- `ProjectSettings/`: configuració del projecte Unity.
- `backend/`: serveis backend Node.js.
- `Builds/`: builds exportades (Linux/Windows).
- `ML-Agents/`: utilitats/configuracions de training.
- `results/`: resultats d'entrenament (runs, checkpoints, ONNX, logs).
- `openspec/`: propostes, dissenys i tasques de canvis funcionals.
- `.gitignore`: exclusió de fitxers/carpetes generats.

### Unity (`Assets/`)

Escenes principals detectades:
- `Assets/Scenes/Core.unity`
- `Assets/Scenes/StartMenu.unity`
- `Assets/Scenes/SampleScene.unity`

Mòduls principals de scripts (`Assets/Scripts/`):
- `Action/`: accions reutilitzables (canvi escena, àudio, warp, etc.).
- `Camera/`: càmera pixel-perfect, confinament i ajust en warps.
- `Combat/`: salut, dany, enemics i efectes de combat.
- `Event/`: sistema d'esdeveniments scriptables i listeners.
- `Item/`: inventari, accions d'ítems, drop/loot.
- `Main Menu/`: slots de guardat i UI de menú inicial.
- `Network/`: clients HTTP/WS, bootstrap i sincronització multijugador.
- `MLAgents/`: agents i runtime d'entrenament.
- `OpenSpec/`: implementacions derivades d'especificacions.
- `System/`: sistemes globals (input, guardat, temps, clima, warp).
- `World/`: grid, objectes de món, cultius i localitzacions.
- `User Interface/`, `Utility/`, `Weather/`, `TileMap/`: suport transversal de joc i render.

## 3) Backend (`backend/`)

### Objectiu

Separa responsabilitats en tres processos:
- API HTTP (`src/api-server.js`).
- Game server (`src/game-server.js`).
- Gateway (`src/gateway-server.js`).

### Scripts disponibles

Segons `backend/package.json`:
- `npm run start:api`
- `npm run start:game`
- `npm run start:gateway`
- `npm test`

### Arquitectura interna

- `controllers/`: entrada de requests.
- `routes/`: rutes HTTP.
- `services/`: lògica de negoci.
- `repositories/memory/`: implementació en memòria.
- `repositories/mongo/`: implementació MongoDB (`connection.js`, `models.js`).
- `utils/password.js`: hash/validació de contrasenyes.

## 4) Xarxa al client Unity

A `Assets/Scripts/Network/`:
- `NetworkBootstrap.cs`: arrencada de xarxa.
- `ApiClient.cs`: comunicació REST.
- `WsClient.cs` + `RealtimeClient.cs`: comunicació en temps real (WS).
- `SharedStateManager.cs`: sincronització d'estat compartit.
- `NetworkActionSender.cs`: enviament d'accions del jugador.
- `RemotePlayerView.cs`: representació de jugadors remots.
- `ResultReporter.cs`: report de resultats cap al backend.
- `NetworkHud.cs`: HUD de diagnòstic/estat de connexió.

## 5) ML-Agents

Elements detectats:
- Runtime/scripts: `Assets/Scripts/MLAgents/*`.
- Integració externa: `External/ml-agents`.
- Resultats de training: `results/slime*`.

Artefactes presents:
- Models ONNX (`*.onnx`).
- Checkpoints (`*.pt`).
- Logs i mètriques de sessions d'entrenament.

## 6) OpenSpec i canvis funcionals

A `openspec/` es documenten canvis amb:
- `proposal.md`
- `design.md`
- `tasks.md`
- specs per capacitat a `specs/.../spec.md`

Canvi detectat:
- `openspec/changes/add-health-powerups/`.

## 7) Execució local

### Requisits

- Unity Hub + versió compatible del projecte.
- Node.js LTS (recomanat >= 18) per al backend.
- npm.
- (Opcional) MongoDB per persistència real.

### Executar el joc

1. Obrir el projecte Unity a `/home/zaxtronic/pruebatr`.
2. Esperar la importació inicial.
3. Obrir escena inicial (`StartMenu.unity` o flux `Core.unity`).
4. Executar en Play Mode.

### Executar backend

1. Entrar a `backend/`.
2. Instal·lar dependències: `npm install`.
3. En terminals separades:
- `npm run start:api`
- `npm run start:game`
- `npm run start:gateway`
4. Tests ràpids de repositoris: `npm test`.

## 8) Estat actual del repositori

- El repositori es va netejar per evitar carpetes generades de Unity (`Library`, `Logs`, etc.) en commits futurs.
- `Builds/` i `results/` estan presents i poden créixer ràpid.
- Existeix un dump de crash gran: `mono_crash.mem.10662.1.blob`.

## 9) Recomanacions de manteniment

- Afegir `backend/node_modules/` a `.gitignore`.
- Valorar moure `results/` i dumps pesats a emmagatzematge extern.
- Definir i documentar variables d'entorn (`.env`) per ports i URLs.
- Afegir un diagrama de flux (Unity client -> gateway -> serveis backend).

## 10) Llicència

No s'ha detectat fitxer `LICENSE` global a l'arrel.
Si el projecte continuarà públic, és recomanable afegir-ne un.
