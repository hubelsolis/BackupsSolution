# CHANGELOG

## Grupo 3 — Infraestructura de Transmisión (feature/grupo3-transferencia)

Riesgos de conflicto a revisar al mergear con Grupo 1 (`nucleo-de-control`) y Grupo 2
(`motor-de-compresion`), que se desarrollaron en ramas separadas partiendo también de `main`:

- **`DestinoConfig` / `Protocolo` duplicados**: `Backups.Core` en `main` todavía no expone
  estas entidades del contrato (viven en la rama `nucleo-de-control`, no mergeada). Grupo 3
  creó copias propias en `Backups.Infrastructure.Network.Domain` (marcadas
  `// PROPUESTO POR GRUPO 3 - NO MERGEAR SIN REVISIÓN`) solo para poder compilar y probar de
  forma aislada. Al mergear, estas copias deben eliminarse y reemplazarse por las de
  `Backups.Core.Domain` / `Backups.Core.Domain.Entities` del Grupo 1.
- **`IOpcionesTransmision`**: abstracción propia de configuración (análoga a
  `IOpcionesCompresion` del Grupo 2) porque no existe un `IConfiguracionPort` central en
  `main` ni un endpoint para resolver `idDestinoConfig` (el contrato solo define
  `POST /config/destinos`). Puerto/usuario/contraseña/ruta remota que el contrato NO define
  en `DestinoConfig` se resuelven aquí vía JSON de configuración (`OpcionesTransmisionJson`,
  variable de entorno `BACKUPS_TRANSMISION_RUTA_CONFIG`).
- **Sin gestión central de paquetes (`Directory.Packages.props`)**: se evitó a propósito para
  no tener que tocar `tests/Backups.Adapters.Tests.csproj` y `tests/Backups.Domain.Tests.csproj`
  (proyectos compartidos, fuera del alcance de Grupo 3). `Backups.Infrastructure.Network.csproj`
  y `tests/Backups.Network.Tests.csproj` pinean versiones explícitas. Si Grupo 1 introduce
  `ManagePackageVersionsCentrally` al mergear, hay que migrar estas dos versiones a
  `PackageVersion` en ese momento.
- **`ITransmissionService`**: firma NO tocada (`bool EnviarArchivos(List<string>, string)`),
  confirmada también como consumida por `ProcesadorPila` en la rama `nucleo-de-control`.
- **`Backups.Core.csproj` → `net8.0`**: único cambio a un archivo fuera de
  `Backups.Infrastructure.Network`. Sin este cambio, `Backups.Infrastructure.Network` (net8.0)
  no puede referenciar `Backups.Core` (net10.0 en `main`). Se verificó que Grupo 1
  (`nucleo-de-control`) y Grupo 2 (`motor-de-compresion`) hicieron exactamente el mismo cambio
  mínimo de forma independiente en sus ramas — no debería generar conflicto real al mergear,
  las tres ramas convergen al mismo valor. No se tocó ninguna entidad, puerto ni lógica de
  Core.
