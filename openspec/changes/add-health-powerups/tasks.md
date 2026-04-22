## 1. Spec y carga de configuración

- [x] 1.1 Definir el modelo de spec de powerups (id, posición, curación, respawn y radio)
- [x] 1.2 Cargar la spec desde `Resources/OpenSpec/powerups.json` con manejo de error de parseo

## 2. Spawn y comportamiento de pickups

- [x] 2.1 Crear spawner runtime que instancie pickups en la escena `Core`
- [x] 2.2 Implementar pickup de vida con trigger, curación y desactivación tras recogida
- [x] 2.3 Implementar respawn por tiempo configurable para cada pickup

## 3. Config inicial y validación

- [x] 3.1 Añadir configuración inicial de powerups de vida en `Assets/Resources/OpenSpec/powerups.json`
- [x] 3.2 Compilar build Linux y validar que los powerups aparecen y curan en runtime
