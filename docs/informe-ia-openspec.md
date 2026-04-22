# Memòria - Ús d'IA amb Spec-Driven Development (OpenSpec)

## 1. Funcionalitat escollida

### Feature
Implementació de **powerups de vida** distribuïts pel mapa principal del joc (`Core`), configurats per especificació i no per valors hardcodejats.

### Justificació
El joc no disposava d'un mecanisme clar de recuperació de vida en exploració/combat. Això reduïa la durada de les sessions i la qualitat del loop de joc. La feature és acotada, verificable i adequada per aplicar SDD amb IA.

## 2. Especificació (OpenSpec)

La funcionalitat s'ha definit amb OpenSpec a la change:
- `openspec/changes/add-health-powerups/`

Documents de definició:
- `proposal.md` (equivalent a context/objectiu/restriccions)
- `design.md` (decisions tècniques, riscos i mitigacions)
- `specs/health-powerups/spec.md` (comportament esperat)
- `tasks.md` (estratègia d'implementació)

### Correspondència amb el format demanat
- `foundations.md` -> cobert per `proposal.md` + seccions de context de `design.md`
- `spec.md` -> cobert per `specs/health-powerups/spec.md`
- `plan.md` -> cobert per `tasks.md`

## 3. Procés seguit amb IA

### Fase PROPOSE
1. Inicialització d'OpenSpec al repositori.
2. Creació de la change `add-health-powerups`.
3. Redacció guiada per IA de proposta, disseny, requisits i pla de tasques.

### Fase APPLY
1. Implementació de model de configuració (`OpenSpecPowerupSpec.cs`).
2. Implementació de pickup i curació (`HealthPowerupPickup.cs`).
3. Implementació de spawner runtime (`OpenSpecPowerupSpawner.cs`).
4. Config inicial declarativa (`Assets/Resources/OpenSpec/powerups.json`).
5. Validació de change i comprovació de build.

## 4. Resultat funcional

La feature implementada compleix el comportament principal definit:
- Spawn automàtic en escena `Core` segons spec.
- Curació del jugador quan entra al trigger del pickup.
- No consum del pickup si l'entitat no té component `Health`.
- Respawn automàtic segons `respawnSeconds`.

## 5. Problemes trobats i decisions preses

### Problema 1: risc de saltar directament a implementació
- Risc: perdre traçabilitat i coherència amb la metodologia.
- Decisió: forçar ordre spec-driven (`proposal -> design -> spec -> tasks -> apply`).
- Impacte: millor control de requisits i verificació.

### Problema 2: evidència insuficient de validació
- Risc: no demostrar ús real d'IA guiat per especificació.
- Decisió: registrar prompts en log cronològic i validar change formalment.
- Impacte: entrega auditable.

## 6. Anàlisi crítica (reflexió)

### L'agent ha seguit realment l'especificació?
Sí, de manera majoritària. La implementació final respecta els tres requisits clau de la spec (spawn, curació, respawn).

### Quantes iteracions han estat necessàries?
7 iteracions significatives (3 definició, 2 implementació, 2 refinament/correcció).

### On falla més la IA?
En consistència metodològica si no se li imposa ordre de treball estricte. També tendeix a donar per bona una implementació sense prou evidència formal si no es demana explícitament validació i traçabilitat.

### S'ha hagut de modificar l'especificació o només prompts?
Principalment prompts i forma d'execució. La base de l'especificació s'ha mantingut estable; els canvis més rellevants han estat de control del procés i de validació.

## 7. Traçabilitat requerida

La traçabilitat completa està documentada a:
- `docs/prompts-log.md`

Inclou:
- prompts de definició,
- prompts d'implementació,
- prompts de correcció/refinament,
- errors detectats,
- relació entre problema i canvi de prompt.

## 8. Valoració final

L'ús d'IA ha estat útil per accelerar l'escriptura d'especificacions i l'execució tècnica, però el resultat només és fiable quan hi ha control explícit del procés (ordre, criteris de validació i evidència de traçabilitat). El valor real no ha estat "que la IA ho faci sola", sinó dirigir-la amb una especificació formal i revisar cada iteració.
