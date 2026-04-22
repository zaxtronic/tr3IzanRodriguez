# Prompts Log - Traçabilitat cronològica (OpenSpec + IA)

Data principal de treball: 2026-04-21  
Projecte: `pruebatr`  
Feature treballada: `add-health-powerups`

## 0) Objectiu de la sessió

Aplicar metodologia Spec-Driven Development amb IA per implementar una funcionalitat concreta: powerups de vida en mapa, configurables per especificació.

## 1) Prompts de definició (fase PROPOSE)

### Iteració P1
- Prompt: "Instal·la OpenSpec i deixa el repo preparat per propose/apply."
- Acció: instal·lació CLI + inicialització (`openspec init --tools codex`).
- Resultat: estructura `openspec/` creada i operativa.

### Iteració P2
- Prompt: "Crea una change formal per afegir powerups de vida al mapa."
- Acció: `openspec new change add-health-powerups`.
- Resultat: change creada amb carpeta pròpia.

### Iteració P3
- Prompt: "Defineix proposta, disseny i spec verificable abans de tocar codi."
- Acció: redacció de:
  - `openspec/changes/add-health-powerups/proposal.md`
  - `openspec/changes/add-health-powerups/design.md`
  - `openspec/changes/add-health-powerups/specs/health-powerups/spec.md`
  - `openspec/changes/add-health-powerups/tasks.md`
- Resultat: especificació completa i ordenada.

## 2) Prompts d'implementació (fase APPLY)

### Iteració A1
- Prompt: "Aplica la change seguint tasks i sense trencar sistemes existents."
- Acció:
  - Model de configuració: `OpenSpecPowerupSpec.cs`
  - Lògica pickup: `HealthPowerupPickup.cs`
  - Spawner runtime: `OpenSpecPowerupSpawner.cs`
  - Config declarativa: `Assets/Resources/OpenSpec/powerups.json`
- Resultat: funcionalitat implementada.

### Iteració A2
- Prompt: "Valida que el comportament coincideix amb la spec (spawn, curació, respawn)."
- Acció: validació de tasks i `openspec validate ...`.
- Resultat: change validada.

## 3) Prompts de correcció i refinament

### Refinament R1
- Problema detectat: calia demostrar que la feature era executable i no només documentada.
- Prompt de correcció: "Compila build Linux i comprova que els pickups apareixen i curen en runtime."
- Acció: build de Unity Linux.
- Resultat: build generada correctament.

### Refinament R2
- Problema detectat: traçabilitat insuficient per avaluació docent.
- Prompt de correcció: "Genera un prompts-log cronològic amb relació entre error, correcció i decisió."
- Acció: ampliació de logs de prompts i decisions.
- Resultat: traçabilitat completa i auditables.

## 4) Evidència d'errors i relació problema -> canvi de prompt

### Error/Desviació E1
- Problema: risc d'implementar sense ordre metodològic (fer codi abans de spec completa).
- Impacte potencial: manca de traçabilitat i desviacions de comportament.
- Canvi de prompt aplicat: prioritzar `proposal -> design -> spec -> tasks` abans de `apply`.
- Correcció: procés reordenat i validat.

### Error/Desviació E2
- Problema: possible discrepància entre funcionalitat esperada i verificació formal.
- Impacte potencial: funcionalitat "aparentment correcta" però no auditable.
- Canvi de prompt aplicat: afegir pas explícit de validació OpenSpec + build.
- Correcció: evidència de validació i entrega executable.

## 5) Resum d'iteracions

- Iteracions de definició: 3
- Iteracions d'implementació: 2
- Iteracions de refinament/correcció: 2
- Total iteracions significatives: 7

## 6) Fitxers relacionats

- `openspec/changes/add-health-powerups/proposal.md`
- `openspec/changes/add-health-powerups/design.md`
- `openspec/changes/add-health-powerups/specs/health-powerups/spec.md`
- `openspec/changes/add-health-powerups/tasks.md`
- `Assets/Scripts/OpenSpec/OpenSpecPowerupSpec.cs`
- `Assets/Scripts/OpenSpec/OpenSpecPowerupSpawner.cs`
- `Assets/Scripts/OpenSpec/HealthPowerupPickup.cs`
- `Assets/Resources/OpenSpec/powerups.json`
