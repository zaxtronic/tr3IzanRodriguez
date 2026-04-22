# Document PDF - Ús d'IA amb desenvolupament guiat per especificació (OpenSpec)

Projecte: `tr3IzanRodriguez`  
Data: 22/04/2026  
Canvi treballat: `add-health-powerups`

## 1) Explicació de la funcionalitat

La funcionalitat implementada és un sistema de **powerups de vida** al mapa principal del joc (`Core`).

En la pràctica, això significa que:
- es defineixen punts de curació en una configuració declarativa,
- el joc genera aquests pickups automàticament en carregar l'escena,
- quan el jugador entra al trigger del pickup i té component `Health`, recupera vida,
- el pickup es desactiva temporalment i reapareix amb un temps de respawn configurable.

Aquesta decisió és important perquè evita hardcodejar posicions i valors en codi: el balanç de gameplay (quant cura cada pickup, on apareix i cada quant torna) es pot ajustar des de fitxer.

## 2) Procés seguit amb la IA

S'ha treballat seguint metodologia Spec-Driven real, no només implementació directa.

### 2.1 Definició (abans de programar)
Es va crear una change OpenSpec i es va completar l'especificació en ordre:
- `proposal.md`: motiu i abast de la millora.
- `design.md`: decisions tècniques, riscos i límits.
- `spec.md`: requisits verificables (spawn, curació, respawn).
- `tasks.md`: pla d'implementació pas a pas.

### 2.2 Implementació guiada
Amb la spec tancada, la IA va ajudar a generar i ajustar:
- `Assets/Scripts/OpenSpec/OpenSpecPowerupSpec.cs`
- `Assets/Scripts/OpenSpec/HealthPowerupPickup.cs`
- `Assets/Scripts/OpenSpec/OpenSpecPowerupSpawner.cs`
- `Assets/Resources/OpenSpec/powerups.json`

### 2.3 Validació
Després de la implementació:
- es van revisar les tasques de la change,
- es va validar que el comportament implementat encaixava amb la spec,
- i es va deixar traçabilitat del procés a `docs/prompts-log.md`.

## 3) Principals problemes trobats

### Problema A: tendència inicial a implementar massa aviat
La IA (i el flux de treball) tendeix a anar directament a codi si no es força una fase de definició formal. Això pot donar resultats funcionals, però difícils d'avaluar metodològicament.

### Problema B: discrepància entre “funciona” i “compleix la spec”
Que una funcionalitat sembli correcta en joc no garanteix que compleixi tots els casos esperats (per exemple, comportament amb objectes sense `Health` o respawn en condicions concretes).

### Problema C: traçabilitat insuficient en primeres iteracions
Sense un registre clar de prompts i refinaments, costa justificar quines decisions es van prendre i per què. Aquest punt és crític per l'avaluació.

## 4) Decisions preses (canvis en prompts o spec)

### Decisió 1: forçar estructura de treball
Es va canviar l'enfocament de prompt de "implementa això" a "defineix primer proposta, disseny, spec i pla".

Impacte:
- menys improvisació,
- més coherència entre objectiu i resultat,
- millor auditabilitat.

### Decisió 2: afegir prompts de verificació explícita
Es van introduir prompts específics per revisar desviacions respecte als requisits.

Impacte:
- detecció més ràpida de inconsistències,
- menys risc de donar per tancat un comportament incomplet.

### Decisió 3: reforçar la traçabilitat com a part de la implementació
No s'ha tractat la traçabilitat com un annex final, sinó com una peça obligatòria del procés.

Impacte:
- cada error important queda associat a una correcció concreta,
- es pot explicar clarament què es va canviar i per quin motiu.

## 5) Valoració crítica real (no superficial)

L'IA ha estat molt útil per accelerar redacció tècnica i implementació, però no substitueix criteri d'enginyeria.

El que millor ha funcionat:
- traduir objectius de gameplay en requisits verificables,
- passar de requisits a codi funcional en poc temps,
- iterar ràpidament quan la direcció estava clara.

El que ha costat més:
- mantenir consistència metodològica sense prompts estrictes,
- evitar respostes massa optimistes quan faltava verificació,
- separar "solució que compila" de "solució que compleix exactament la spec".

Conclusió:
- El resultat final és bo i coherent amb el projecte,
- però la part clau de la pràctica no és només el codi, sinó la qualitat del procés.
- Quan es treballa amb especificació formal, prompts controlats i traçabilitat real, la IA aporta valor. Sense això, el risc de desviació és alt.

## Annex breu d'evidències

- Especificació OpenSpec utilitzada:
  - `openspec/changes/add-health-powerups/proposal.md`
  - `openspec/changes/add-health-powerups/design.md`
  - `openspec/changes/add-health-powerups/specs/health-powerups/spec.md`
  - `openspec/changes/add-health-powerups/tasks.md`

- Implementació principal:
  - `Assets/Scripts/OpenSpec/OpenSpecPowerupSpec.cs`
  - `Assets/Scripts/OpenSpec/HealthPowerupPickup.cs`
  - `Assets/Scripts/OpenSpec/OpenSpecPowerupSpawner.cs`
  - `Assets/Resources/OpenSpec/powerups.json`

- Traçabilitat del procés:
  - `docs/prompts-log.md`
