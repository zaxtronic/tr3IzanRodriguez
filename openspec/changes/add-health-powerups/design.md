## Context

El proyecto Unity ya tiene un sistema de vida (`Combat.Health`) y escenas de juego donde se mueve el jugador. No existe un sistema de pickups de curación en mapa configurable por datos. Se requiere una forma simple de añadir, mover y balancear puntos de curación sin reescribir lógica.

## Goals / Non-Goals

**Goals:**
- Cargar configuración de powerups de vida desde un archivo OpenSpec en `Resources`.
- Spawnear pickups en `Core` automáticamente en runtime.
- Al recoger un pickup, curar al jugador usando `Health.Heal`.
- Soportar respawn configurable por pickup.

**Non-Goals:**
- Sincronización de estado de pickups por red (coherencia host/cliente de disponibilidad).
- UI nueva de vida o efectos visuales avanzados.
- Editor tools personalizados para posicionar pickups.

## Decisions

1. **Config por JSON en `Resources/OpenSpec/powerups.json`**
- Razón: permite cambios rápidos de balance sin recompilar lógica.
- Alternativa descartada: ScriptableObject por pickup (más pesado para iterar y menos portable con spec-driven).

2. **Spawner automático por `RuntimeInitializeOnLoadMethod` en escena `Core`**
- Razón: no depende de wiring manual en escenas/prefabs y evita olvidos.
- Alternativa descartada: prefab manager en escena (más propenso a desincronización entre escenas).

3. **Aplicar curación usando `Combat.Health` existente**
- Razón: reutiliza el contrato actual y evita duplicar estado de vida.
- Alternativa descartada: sistema de HP paralelo (riesgo de inconsistencias).

4. **Respawn local del pickup con ocultación temporal**
- Razón: implementación mínima funcional para gameplay inmediato.
- Alternativa descartada: pooling y autoridad de servidor (más complejo, se deja para iteración siguiente).

## Risks / Trade-offs

- **[Riesgo]** Posiciones de pickups no alineadas al mapa real.  
  **Mitigación:** editar coordenadas en `powerups.json` y re-test rápido.

- **[Riesgo]** En multiplayer cada cliente puede gestionar respawn local distinto.  
  **Mitigación:** documentar limitación y planear sincronización WS en cambio futuro.

- **[Trade-off]** Sprite generado por código en lugar de arte final.  
  **Mitigación:** permitir reemplazo posterior con sprite asset sin tocar lógica.
