## Why

El juego co-op ya permite entrar a la misma partida, pero no tiene una mecánica clara de recuperación de vida en mapa. Eso limita la duración de las sesiones y reduce interacción entre jugadores en exploración/combate.

## What Changes

- Añadir powerups de vida en el mapa principal (`Core`) cargados desde una especificación OpenSpec.
- Definir una spec de powerups con: posición, curación, radio de recogida y tiempo de respawn.
- Spawnear pickups al cargar `Core` y aplicar curación al entrar en su trigger.
- Hacer que el pickup desaparezca temporalmente tras recogida y reaparezca según su `respawnSeconds`.
- Dejar la configuración en un archivo declarativo para editar balance sin tocar código.

## Capabilities

### New Capabilities
- `health-powerups`: Spawn de pickups de vida configurables por spec OpenSpec y aplicación de curación al jugador que los recoge.

### Modified Capabilities
- Ninguna.

## Impact

- Código Unity de runtime en `Assets/Scripts/OpenSpec/`.
- Configuración en recursos `Assets/Resources/OpenSpec/powerups.json`.
- Uso del sistema existente `Combat.Health` para curación.
- No rompe API de backend ni contratos WS actuales.
