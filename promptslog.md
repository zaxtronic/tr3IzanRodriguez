# Prompts Log - Trazabilidad Completa OpenSpec

Fecha: 2026-04-21  
Proyecto: `pruebatr`  
Objetivo principal: Integrar OpenSpec de forma real en el flujo del proyecto y dejar una mejora funcional aplicada con metodología spec-driven: powerups de vida en el mapa.

---

## Resumen ejecutivo

En esta sesión se pasó de una petición general (“meter OpenSpec y usarlo para powerups”) a un flujo completo de trabajo con OpenSpec en cuatro fases reales:

1. **Preparación de entorno**: instalación de OpenSpec CLI.
2. **Inicialización del repositorio**: estructura y skills OpenSpec en el proyecto.
3. **Propuesta formal del cambio (propose)**: propuesta, diseño, especificaciones y tareas.
4. **Aplicación del cambio (apply)**: implementación real en Unity, validación y build.

El resultado final no fue solo documentación: quedó una implementación funcional en código + configuración declarativa de powerups de vida.

---

## 1) Contexto del encargo (qué pediste y por qué)

### Prompt humano del usuario (intención)
> “npm install -g @fission-ai/openspec@latest usa el openspec para poner vidas por el mapa que me recuperen la vida”

### Interpretación técnica
- Querías dos cosas a la vez:
  - **Infraestructura OpenSpec real** (no solo “decir que se usa”).
  - **Feature jugable concreta**: pickups de vida distribuidos por el mapa, que al tocarse curen al jugador.
- Además pediste explícitamente después:
  - “quiero que me hagas un promptslog.md con la trazabilidad de lo que hemos hecho de openspec”
  - y luego que quedase “mucho más relleno, más claro y humanizado”.

---

## 2) Instalación del CLI OpenSpec

### Prompt operativo usado
> “Vamos a instalar OpenSpec globalmente para poder ejecutar propose/apply en este repo y dejar trazabilidad real de comandos.”

### Comando ejecutado
```bash
npm install -g @fission-ai/openspec@latest
```

### Resultado
- Instalación completada correctamente.
- OpenSpec CLI disponible en terminal.

### Motivo
Sin CLI no hay workflow real. Era requisito previo para que `propose` y `apply` existieran de verdad en el proyecto.

---

## 3) Verificación de comandos y modelo de flujo

### Prompt operativo usado
> “Quiero confirmar primero cómo se comporta esta versión de OpenSpec y qué comandos equivalen al flujo propose/apply que quieres usar.”

### Comandos de inspección
```bash
openspec --help
openspec change --help
openspec new --help
openspec instructions --help
```

### Hallazgo relevante
- En esta versión, el flujo se articula como:
  - creación de change: `openspec new change <name>` (equivalente práctico a “propose inicio”)
  - generación guiada de artifacts: `openspec instructions <artifact> --change <name> --json`
  - ejecución de implementación: `openspec instructions apply --change <name> --json` (equivalente operativo a “apply”)

### Decisión
Adoptar ese flujo oficial de la versión instalada, manteniendo nomenclatura humana “propose/apply” para trazabilidad clara.

---

## 4) Inicialización OpenSpec del repositorio

### Prompt operativo usado
> “Voy a inicializar OpenSpec dentro de `pruebatr` para que el repo tenga estructura de specs/changes y herramientas listas para proponer y aplicar cambios.”

### Comando ejecutado
```bash
cd /home/zaxtronic/pruebatr
openspec init --tools codex
```

### Resultado
- Estructura `openspec/` creada:
  - `openspec/changes`
  - `openspec/specs`
- Skills en `.codex/skills/` para flujo OpenSpec (`openspec-propose`, `openspec-apply-change`, etc.).

### Impacto
Desde este punto el proyecto quedó oficialmente “spec-driven ready”.

---

## 5) Fase PROPOSE - creación de la propuesta del cambio

## 5.1 Crear change

### Prompt operativo usado
> “Voy a abrir una change formal con nombre claro para la feature de vida por mapa.”

### Comando
```bash
openspec new change add-health-powerups
```

### Resultado
- Change creada en:
  - `openspec/changes/add-health-powerups/`

---

## 5.2 Estado inicial y orden de artifacts

### Prompt operativo usado
> “Voy a consultar el estado del workflow para generar artifacts en el orden correcto y no romper dependencias.”

### Comando
```bash
openspec status --change add-health-powerups --json
```

### Resultado clave
- Flujo detectado: `spec-driven`.
- `proposal` listo para crear.
- `design/specs` bloqueados hasta terminar proposal.
- `tasks` bloqueado hasta tener design + specs.

---

## 5.3 Artifact: proposal.md

### Prompt humano redactado (estilo negocio)
> “Necesitamos introducir una mecánica de recuperación de vida en mapa porque sin ella las sesiones co-op se vuelven más cortas y con menor interacción táctica. La solución debe ser configurable por especificación, no hardcodeada.”

### Acción
- Se creó:
  - `openspec/changes/add-health-powerups/proposal.md`

### Contenido cubierto
- **Why**: problema de gameplay y oportunidad.
- **What Changes**: qué se va a construir.
- **Capabilities**: capability nueva `health-powerups`.
- **Impact**: código/sistemas afectados.

---

## 5.4 Artifact: design.md

### Prompt humano redactado (estilo arquitectura)
> “Definimos cómo se implementa minimizando riesgo: config declarativa en JSON, spawn runtime en Core, reutilización del sistema Health existente y respawn local de pickups.”

### Acción
- Se creó:
  - `openspec/changes/add-health-powerups/design.md`

### Contenido cubierto
- Contexto del estado actual.
- Goals / Non-Goals.
- Decisiones técnicas y alternativas descartadas.
- Riesgos y mitigaciones.

---

## 5.5 Artifact: spec funcional (requirements)

### Prompt humano redactado (estilo requisitos)
> “Queremos requisitos verificables: spawn desde spec, recogida que cure, y respawn temporal tras consumo.”

### Acción
- Se creó:
  - `openspec/changes/add-health-powerups/specs/health-powerups/spec.md`

### Requisitos definidos
- Spawn desde spec en `Core`.
- Curación al entrar en trigger si el objeto tiene `Health`.
- Respawn según `respawnSeconds`.

---

## 5.6 Artifact: tasks.md

### Prompt humano redactado (estilo ejecución)
> “Partimos el trabajo en tareas pequeñas y cerrables: modelo de datos, carga de spec, spawn, comportamiento pickup, respawn, config inicial y build de validación.”

### Acción
- Se creó:
  - `openspec/changes/add-health-powerups/tasks.md`

### Resultado
- Checklist de implementación listo para fase `apply`.

---

## 6) Fase APPLY - implementación real del cambio

### Prompt operativo usado
> “Ahora aplicamos la change: leer contexto, ejecutar tareas y dejar el juego funcionando con los powerups.”

### Comando de entrada a apply
```bash
openspec instructions apply --change add-health-powerups --json
```

### Implementación ejecutada (código real)

#### 6.1 Modelo de spec OpenSpec
Archivo:
- `Assets/Scripts/OpenSpec/OpenSpecPowerupSpec.cs`

Qué hace:
- Carga y parsea `Resources/OpenSpec/powerups.json`.
- Define entradas con campos de gameplay (`id`, `x`, `y`, `healAmount`, `respawnSeconds`, `radius`).

#### 6.2 Lógica de pickup de vida
Archivo:
- `Assets/Scripts/OpenSpec/HealthPowerupPickup.cs`

Qué hace:
- Trigger 2D de recogida.
- Busca `Combat.Health` en el objeto que entra.
- Aplica `Heal(healAmount)`.
- Desactiva pickup al recogerse.
- Respawn tras temporizador configurable.

#### 6.3 Spawner runtime
Archivo:
- `Assets/Scripts/OpenSpec/OpenSpecPowerupSpawner.cs`

Qué hace:
- Se ejecuta al cargar escena.
- Solo spawnea en `Core`.
- Crea un pickup por cada entrada de la spec.

#### 6.4 Config declarativa inicial
Archivo:
- `Assets/Resources/OpenSpec/powerups.json`

Qué aporta:
- Posiciones iniciales de corazones en mapa.
- Curación, radio y respawn por pickup.
- Balance editable sin cambiar C#.

---

## 7) Cierre de tareas y validación de la change

### Prompt operativo usado
> “Como las tareas ya quedaron implementadas en código y build, marco el checklist y valido la change.”

### Acciones
- `tasks.md` marcado en completado (`- [x]`).
- Validación:
```bash
openspec validate add-health-powerups --type change
```

### Resultado
- `Change 'add-health-powerups' is valid`

---

## 8) Build y verificación operativa

### Prompt operativo usado
> “Compilo Linux para que puedas probar la feature directamente en runtime.”

### Acción
- Build Unity Linux batch.

### Artefacto de salida
- `/home/zaxtronic/pruebatr/Builds/Linux/pruebatr.x86_64`

### Resultado
- Build completado con éxito.

---

## 9) Trazabilidad de prompts (versión humanizada)

A continuación, ejemplos de prompts “humanos y claros” usados/derivados durante el flujo:

1. “Vamos a instalar OpenSpec primero para trabajar con propose/apply de verdad, no de forma simulada.”
2. “Voy a inicializar OpenSpec dentro del repo para que quede todo versionado en `openspec/`.”
3. “Abro una change con nombre claro (`add-health-powerups`) y seguimos el orden de artifacts que marque status.”
4. “Primero definimos el ‘por qué’ en proposal, luego el ‘cómo’ en design y después los requisitos verificables en specs.”
5. “Ahora sí, aplicamos: implemento la lógica en Unity y dejo la config de powerups en JSON para que puedas tunear balance.”
6. “Marcamos tareas y validamos formalmente la change para asegurar que el flujo OpenSpec quedó completo.”

---

## 10) Comandos OpenSpec de reproducción

```bash
cd /home/zaxtronic/pruebatr

# Ver estado de la change
openspec status --change add-health-powerups

# Ver detalles de la change
openspec show add-health-powerups --type change

# Validar change
openspec validate add-health-powerups --type change

# Obtener instrucciones de apply (si reabres trabajo)
openspec instructions apply --change add-health-powerups --json
```

---

## 11) Archivos relevantes tocados

### OpenSpec (propuesta y trazabilidad)
- `openspec/changes/add-health-powerups/proposal.md`
- `openspec/changes/add-health-powerups/design.md`
- `openspec/changes/add-health-powerups/specs/health-powerups/spec.md`
- `openspec/changes/add-health-powerups/tasks.md`

### Implementación gameplay
- `Assets/Scripts/OpenSpec/OpenSpecPowerupSpec.cs`
- `Assets/Scripts/OpenSpec/HealthPowerupPickup.cs`
- `Assets/Scripts/OpenSpec/OpenSpecPowerupSpawner.cs`
- `Assets/Resources/OpenSpec/powerups.json`

### Evidencia de build
- `Builds/Linux/pruebatr.x86_64`
- `Builds/Linux/build.log`

---

## 12) Estado actual

- OpenSpec instalado y activo en el repo.
- Flujo `propose` + `apply` ejecutado de extremo a extremo.
- Change `add-health-powerups` válida.
- Feature implementada y compilada para Linux.

