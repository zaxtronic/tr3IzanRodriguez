## ADDED Requirements

### Requirement: Spawn de powerups de vida desde spec
El sistema SHALL cargar una especificación de powerups de vida y crear pickups en la escena `Core` usando coordenadas y parámetros definidos en dicha especificación.

#### Scenario: Carga de configuración válida
- **WHEN** se carga la escena `Core`
- **THEN** el juego crea un pickup por cada entrada válida en la spec de powerups

#### Scenario: Configuración faltante o vacía
- **WHEN** no existe el archivo de spec o no contiene powerups
- **THEN** el juego no crea pickups y registra un aviso en logs sin bloquear la partida

### Requirement: Recogida y curación
El sistema SHALL curar al jugador que entra en el área de recogida del powerup, aplicando `healAmount` al componente `Health` del jugador.

#### Scenario: Jugador recoge powerup
- **WHEN** un jugador con `Health` entra en el trigger de un pickup disponible
- **THEN** el sistema aplica curación y marca el pickup como no disponible inmediatamente

#### Scenario: Entidad sin componente Health
- **WHEN** un objeto sin `Health` entra en el trigger del pickup
- **THEN** el pickup no se consume y no se aplica curación

### Requirement: Respawn de pickup
El sistema SHALL reactivar automáticamente un pickup tras el tiempo configurado en `respawnSeconds`.

#### Scenario: Reaparición normal
- **WHEN** pasa el tiempo `respawnSeconds` después de recoger un pickup
- **THEN** el pickup vuelve a estar visible y disponible para recogida
