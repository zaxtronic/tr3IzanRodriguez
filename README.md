# tr3IzanRodriguez (pruebatr)

Proyecto de videojuego 2D en Unity con:
- Juego base estilo farming/combat.
- Módulos de red en tiempo real (cliente Unity + backend Node.js).
- Integración con ML-Agents para entrenamiento de agentes.
- Especificación funcional incremental con OpenSpec.

## 1) Resumen técnico

- Motor principal: Unity (carpeta `Assets/`, `Packages/`, `ProjectSettings/`).
- Lógica de juego: C# en `Assets/Scripts`.
- Backend: Node.js en `backend/` con API HTTP, gateway y servidor de juego.
- Persistencia backend: repositorios en memoria y MongoDB (según configuración).
- IA/Entrenamiento: integración de ML-Agents (`Assets/Scripts/MLAgents`, `results/`, `ML-Agents/`, `External/ml-agents`).

## 2) Estructura del repositorio

### Raíz

- `Assets/`: contenido principal Unity (escenas, prefabs, scripts, recursos).
- `Packages/`: dependencias de Unity Package Manager.
- `ProjectSettings/`: configuración del proyecto Unity.
- `backend/`: servicios backend en Node.js.
- `Builds/`: builds exportadas (Linux/Windows).
- `ML-Agents/`: configuración/utilidades relacionadas con entrenamiento.
- `results/`: resultados de entrenamiento (runs, checkpoints, onnx, logs).
- `openspec/`: cambios, propuestas y especificaciones funcionales.
- `.gitignore`: exclusión de carpetas generadas de Unity y artefactos de entorno.

### Assets relevantes

- `Assets/Scenes/`
- Escenas principales detectadas: `Core.unity`, `StartMenu.unity`, `SampleScene.unity`.

- `Assets/Scripts/` (módulos)
- `Action/`: sistema de acciones reutilizables (cambio de escena, audio, warp, etc.).
- `Camera/`: cámara pixel-perfect, confinamiento y warp sync.
- `Combat/`: daño, salud, enemigos, efectos al recibir daño.
- `Event/`: arquitectura basada en eventos scriptables y listeners.
- `Item/`: inventario, acciones de ítems, drop/loot.
- `Main Menu/`: gestión de slots, UI de selección y carga.
- `Network/`: cliente API/WS, bootstrap, sincronización de estado, HUD de red.
- `MLAgents/`: agentes de entrenamiento (por ejemplo `SlimeAgent`).
- `OpenSpec/`: implementación de cambios definidos por especificación (powerups de vida).
- `System/`: sistemas globales (input, interacción, guardado, tiempo, warp, clima).
- `World/`: grid, cultivos, warp entre escenas/localizaciones.
- `User Interface/`, `Utility/`, `Weather/`, `TileMap/`: soporte visual y de gameplay.

## 3) Backend (`backend/`)

### Objetivo

El backend separa responsabilidades en tres procesos:
- API HTTP (`src/api-server.js`): operaciones CRUD y endpoints de aplicación.
- Game server (`src/game-server.js`): lógica para tiempo real (partidas/sesiones).
- Gateway (`src/gateway-server.js`): entrada unificada/proxy para cliente.

### Scripts disponibles

Desde `backend/package.json`:
- `npm run start:api`
- `npm run start:game`
- `npm run start:gateway`
- `npm test`

### Arquitectura interna

- `controllers/`: reciben requests y coordinan casos de uso.
- `routes/`: definición de rutas HTTP.
- `services/`: lógica de negocio por dominio.
- `repositories/`
- `memory/`: implementación en memoria.
- `mongo/`: implementación MongoDB (`connection.js`, `models.js`).
- `utils/password.js`: utilidades de hash/verificación de credenciales.

## 4) Red en Unity (cliente)

En `Assets/Scripts/Network/`:
- `NetworkBootstrap.cs`: inicialización de dependencias de red.
- `ApiClient.cs`: comunicación HTTP con backend.
- `WsClient.cs` y `RealtimeClient.cs`: comunicación WebSocket y tiempo real.
- `SharedStateManager.cs`: sincronización/estado compartido.
- `NetworkActionSender.cs`: envío de acciones del jugador.
- `RemotePlayerView.cs`: representación de jugadores remotos.
- `ResultReporter.cs`: reporte de resultados al backend.
- `NetworkHud.cs`: capa UI para diagnóstico/estado de conexión.

## 5) ML-Agents y entrenamiento

Componentes detectados:
- Scripts runtime: `Assets/Scripts/MLAgents/*`.
- Resultados de entrenamiento: `results/slime*`.
- Integración externa: `External/ml-agents`.

Tipos de artefactos observados:
- modelos ONNX (`*.onnx`), checkpoints (`*.pt`), métricas/logs de entrenamiento.

## 6) OpenSpec (cambios funcionales)

`openspec/` contiene especificaciones de cambios:
- propuesta (`proposal.md`)
- diseño (`design.md`)
- tareas (`tasks.md`)
- spec por capacidad (`specs/.../spec.md`)

Ejemplo detectado:
- `openspec/changes/add-health-powerups/` (powerups de vida)

## 7) Cómo ejecutar localmente

### Requisitos

- Unity Hub + versión compatible con el proyecto.
- Node.js LTS (recomendado >= 18) para `backend/`.
- npm.
- (Opcional) MongoDB si se usa repositorio persistente.

### Ejecutar juego en Unity

1. Abrir el proyecto en Unity (`/home/zaxtronic/pruebatr`).
2. Esperar importación inicial de assets.
3. Abrir escena de entrada (`Assets/Scenes/StartMenu.unity` o flujo definido por `Core.unity`).
4. Ejecutar Play Mode.

### Ejecutar backend

1. Ir a `backend/`.
2. Instalar dependencias: `npm install`.
3. Iniciar servicios (en terminales separadas):
- `npm run start:api`
- `npm run start:game`
- `npm run start:gateway`
4. Ejecutar tests rápidos de repositorios: `npm test`.

## 8) Estado actual del repo y notas prácticas

- El repositorio fue limpiado para no depender de archivos generados de Unity (`Library`, `Logs`, etc.) en futuras subidas.
- `Builds/` y `results/` están presentes en el historial actual; son útiles como evidencia de ejecución/entrenamiento, pero pueden crecer rápido.
- Existe un archivo grande de crash dump: `mono_crash.mem.10662.1.blob`.

## 9) Recomendaciones de mantenimiento

- Añadir exclusión de `backend/node_modules/` en `.gitignore` para evitar versionar dependencias instaladas.
- Evaluar si `results/` y dumps de crash deben quedar fuera del repo principal o moverse a almacenamiento externo.
- Definir en `.env` y documentar puertos/URL de API, game server y gateway para estandarizar entornos.
- Añadir diagramas simples (cliente Unity -> gateway -> servicios) para onboarding más rápido.

## 10) Licencia

No se detectó archivo de licencia global en la raíz. Si el proyecto se va a compartir públicamente, conviene añadir `LICENSE`.
