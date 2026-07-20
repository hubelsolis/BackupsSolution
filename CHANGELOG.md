# CHANGELOG

## Grupo 4 — Adaptadores de Entrada / Interfaz WinForms (Luis Ariam "Luchito", Castillejo)

### Decisiones y supuestos (leer antes de mergear)

1. **`Backups.Core` (Domain + Ports) se trajo tal cual desde `origin/nucleo-de-control`** (commit del Grupo 1,
   51 tests, 94.9% cobertura), no desde el stub obsoleto que había en `main`. El stub de `main` tenía
   `IEjecutarRespaldoUseCase.Ejecutar(SolicitudRespaldo)` síncrono, sin puerto de historial y con
   `TipoDisparo`/`AlgoritmoCompresion` como `string` sueltos — no coincide con el contrato v1.3.0 real que ya
   implementó el Grupo 1 (`EjecutarAsync`, enums, `IObtenerHistorialUseCase`, `LogRespaldo`,
   `RespuestaEjecucion`). Solo se copiaron `Domain/` y `Ports/` (el contrato congelado); **no** se trajeron
   `OrquestadorRespaldos.cs`, `ProcesadorPila.cs`, `ServicioTemporizador.cs` ni `Backups.Adapters.Infrastructure`
   real del Grupo 1 (implementación interna, no es lo que consume la UI).
   Se retargeteó únicamente el `TargetFramework` de `Backups.Core.csproj` a `net8.0` (sin tocar ninguna firma).
2. **No hay implementación real de `IEjecutarRespaldoUseCase`/`IObtenerHistorialUseCase` disponible en `main`**
   (el `OrquestadorRespaldos` real del Grupo 1 no está mergeado). Se agregó
   `Adapters/AdaptadorPendienteIntegracionGrupo1.cs`, marcado `// PROPUESTO POR GRUPO 4 - NO MERGEAR SIN
   REVISIÓN`: es un eco en memoria que permite compilar, testear y demostrar la UI de forma aislada. **Debe
   eliminarse** y reemplazarse por el registro DI del `OrquestadorRespaldos` real en `Program.cs` cuando
   `nucleo-de-control` se mergee a `main`.
3. **Dos proyectos de UI estaban presentes en `main`**: `Backups.Adapters.UI.WinForms` (wireado en el
   `.slnx`) y `Backups.UI` (huérfano, no está en el `.slnx`). Se usó `Backups.Adapters.UI.WinForms` como
   proyecto real del Grupo 4 (consistente con la convención `Backups.Adapters.*` de los otros grupos y con el
   nombre de la rama). `Backups.UI` se dejó intacto, sin tocar; el administrador del repo debería decidir si
   se elimina.
4. **La rama remota `tests-grupo4-ui` NO estaba vacía**: tenía un commit de Luchito (2026-07-11) con un
   borrador funcional (Core propio + `FormConfig` + tests). Por decisión explícita del usuario, se descartó
   ese commit y se partió de `main` limpio; el push final con `--force-with-lease` lo reemplaza a propósito.
5. **No se tocó** `Backups.Domain`, `Backups.Ports`, `Backups.Adapters.Infrastructure`,
   `Backups.Infrastructure.Compression`, `Backups.Infrastructure.Network` ni `Backups.UI`. Siguen en `net10.0`
   — fuera de alcance del Grupo 4.
6. **Historial**: la UI no parsea el TXT directamente. Lee `List<LogRespaldo>` a través de
   `IObtenerHistorialUseCase.ObtenerHistorialAsync()`, el puerto que el propio Grupo 1 definió para este fin
   (implementado por su `LogTxtAdapter`, fuera del alcance del Grupo 4).
7. **`ultimoHashConocido`** es opcional según el contrato; la UI no lo captura (lo resuelve el Grupo 1
   internamente contra el último hash confirmado del log).
8. Proyecto de tests propio (`tests/Backups.Adapters.UI.WinForms.Tests`) en vez de sumarse al
   `Backups.Adapters.Tests` compartido, para evitar conflictos "added by both" con los `.csproj` de los otros
   grupos al mergear.
