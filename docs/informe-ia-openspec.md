# Memòria de Pràctica - Ús d'IA amb Spec-Driven Development (OpenSpec)

Projecte: `tr3IzanRodriguez`  
Data: 22/04/2026  
Feature treballada: `add-health-powerups`

## 1) Explicació de la funcionalitat

La funcionalitat implementada és un sistema de **powerups de vida** distribuïts pel mapa principal (`Core`).

Quan el jugador passa per sobre d'un pickup:
- si té component `Health`, rep curació,
- el pickup desapareix temporalment,
- i torna a aparèixer passat un temps de respawn configurat.

La part important és que no està hardcodejada: la posició i valors de cada pickup es defineixen en una especificació/configuració declarativa (`powerups.json`), de manera que podem ajustar el balanç del joc sense tocar la lògica C#.

## 2) Procés seguit amb la IA

He seguit un flux Spec-Driven real, no improvisat:

1. Preparació d'entorn
- Instal·lació i validació d'OpenSpec CLI.
- Inicialització d'OpenSpec al repositori.

2. Definició formal abans de programar
- `proposal.md`: per què calia la funcionalitat i quin impacte tindria.
- `design.md`: decisions tècniques, riscos i límits.
- `spec.md`: requisits verificables (spawn, curació, respawn).
- `tasks.md`: pla d'implementació en passos petits.

3. Implementació guiada per la spec
- Càrrega de la spec (`OpenSpecPowerupSpec.cs`).
- Comportament del pickup (`HealthPowerupPickup.cs`).
- Spawn runtime a `Core` (`OpenSpecPowerupSpawner.cs`).
- Config inicial (`Assets/Resources/OpenSpec/powerups.json`).

4. Validació
- Tancament de tasques i validació de la change.
- Comprovació de build per assegurar que no era només teoria.

## 3) Principals problemes trobats

### Problema 1: risc de “fer codi massa aviat”
Al principi era fàcil caure en el típic flux de provar prompts i anar tocant codi sense tenir la definició tancada.

### Problema 2: traçabilitat insuficient
Tot i que la funcionalitat funcionava, sense evidència ordenada de prompts, errors i correccions la pràctica quedava fluixa a nivell d'avaluació.

### Problema 3: tendència de la IA a donar solucions massa “optimistes”
La IA sovint assumeix que una implementació és bona si compila o sembla coherent, però això no garanteix que estigui completament alineada amb el comportament definit al spec.

## 4) Decisions preses (canvis en prompts o spec)

### Decisió A: imposar ordre metodològic
Canvi de prompt:
- de: "implementa els powerups"
- a: "primer proposal/design/spec/tasks, després apply"

Resultat:
- millor control de requisits,
- menys desviacions,
- procés més auditable.

### Decisió B: forçar validació explícita
Canvi de prompt:
- de: "ja està implementat"
- a: "valida formalment la change i deixa evidència"

Resultat:
- no dependre de percepcions,
- tenir comprovació objectiva.

### Decisió C: reforçar traçabilitat
Canvi de prompt:
- de: "fes un resum"
- a: "fes log cronològic amb error -> correcció -> impacte"

Resultat:
- es veu clarament què va fallar,
- què es va canviar,
- i per què es va canviar.

## 5) Valoració crítica real (no superficial)

En aquesta pràctica, la IA ha estat útil de veritat, però **només** quan s'ha treballat amb disciplina.

Punts forts:
- accelera molt la redacció inicial d'especificacions,
- ajuda a transformar requisits en tasques concretes,
- facilita iterar ràpidament en implementació.

Punts febles observats:
- si el prompt és ambigu, la IA omple buits amb suposicions,
- tendeix a avançar massa ràpid cap a implementació,
- pot donar una falsa sensació de "ja està" sense prou verificació.

Conclusió personal:
- El valor no és "deixar que la IA programi sola".
- El valor és saber **dirigir-la amb una especificació formal**, revisar el resultat i corregir desviacions amb criteri.
- Quan això es fa bé, el resultat és consistent, defensable i avaluable.

## 6) Traçabilitat i anàlisi del procés (checklist)

Per a cada criteri:
- ✔️ `Compleix el criteri`
- ❌ `No compleix el criteri`

- ✔️ He registrat tots els prompts en un `prompts-log.md`.
- ✔️ He documentat errors detectats durant el procés.
- ✔️ He explicat com he corregit cada error.
- ✔️ He relacionat cada problema amb el canvi en el prompt o en l'especificació.
- ✔️ He fet una reflexió crítica sobre el resultat final i el comportament de la IA.

## 7) Evidències i fitxers entregables

- OpenSpec change:
  - `openspec/changes/add-health-powerups/proposal.md`
  - `openspec/changes/add-health-powerups/design.md`
  - `openspec/changes/add-health-powerups/specs/health-powerups/spec.md`
  - `openspec/changes/add-health-powerups/tasks.md`

- Implementació:
  - `Assets/Scripts/OpenSpec/OpenSpecPowerupSpec.cs`
  - `Assets/Scripts/OpenSpec/HealthPowerupPickup.cs`
  - `Assets/Scripts/OpenSpec/OpenSpecPowerupSpawner.cs`
  - `Assets/Resources/OpenSpec/powerups.json`

- Traçabilitat:
  - `docs/prompts-log.md`

