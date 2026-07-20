# BackupsSolution

Sistema de Gestión de Copias de Seguridad y Réplicas (BACKUPS) — arquitectura hexagonal en C#/.NET 8.
Contrato de datos: OpenAPI v1.3.0 (fuente de verdad para nombres de modelos, campos y enums).

## Grupo 4 — Adaptadores de Entrada / Interfaz WinForms (Luis Ariam "Luchito", Castillejo)

### Responsabilidad

Capa de presentación (Driving Adapter) del contrato v1.3.0: formulario para configurar carpeta de origen,
destino, algoritmo de compresión y tamaño de volumen; botón de respaldo manual; historial de respaldos;
System Tray; ventana de Logs; formulario de configuración.

### Qué hace la UI

- **Formulario principal** (`FormPrincipal`): selecciona carpeta de origen (`FolderBrowserDialog`), nombre de
  copia, algoritmo (`ZIP`/`RAR`/`LZMA`), límite de volumen en MB y destino (`idDestinoConfig`). El botón
  **"Respaldar ahora"** arma un `SolicitudRespaldo` con `tipoDisparo = BOTON_MANUAL` (los literales exactos
  del contrato) y lo entrega al caso de uso del Grupo 1 (`IEjecutarRespaldoUseCase.EjecutarAsync`). Muestra el
  resultado (`backupId` + `estadoInicial`) o los errores de validación.
- **Historial**: un `DataGridView` lista los `LogRespaldo` (backupId, estado, mensaje) obtenidos vía
  `IObtenerHistorialUseCase.ObtenerHistorialAsync()`, con botón "Actualizar".
- **System Tray**: ícono de bandeja (`NotifyIcon`) con menú Mostrar/Salir; minimizar oculta la ventana.
- **Ventana de Logs** (`FormHistorialLogs`): vista ampliada del historial completo.
- **Formulario de configuración** (`FormConfiguracion`): edita la carpeta de origen por defecto y administra
  la lista de destinos disponibles para la sesión actual.

La UI **no comprime ni transmite nada**: solo construye la solicitud y la entrega al Grupo 1, que orquesta
compresión (Grupo 2) y transmisión (Grupo 3).

### Cómo ejecutarla

```powershell
dotnet run --project src/Backups.Adapters.UI.WinForms/Backups.Adapters.UI.WinForms.csproj
```

Los valores por defecto (carpeta de origen, límite de volumen, destinos disponibles) se leen de
`src/Backups.Adapters.UI.WinForms/appsettings.json` — nada hardcodeado en el código.

### Cómo se conecta con el Grupo 1

La UI referencia únicamente `Backups.Core` (Domain + Ports) e inyecta por DI (`Microsoft.Extensions.
DependencyInjection`) las interfaces `IEjecutarRespaldoUseCase` e `IObtenerHistorialUseCase` que expone el
Grupo 1. **Importante:** a la fecha de esta rama, la implementación real del Grupo 1
(`OrquestadorRespaldos`) todavía no está mergeada a `main` — se usa un adaptador temporal propio
(`Adapters/AdaptadorPendienteIntegracionGrupo1.cs`, marcado `PROPUESTO POR GRUPO 4 - NO MERGEAR SIN
REVISIÓN`) que permite compilar, testear y demostrar la UI de forma aislada. Ver `CHANGELOG.md` para el
detalle completo de esta y otras decisiones tomadas en esta rama.

### Tests

`tests/Backups.Adapters.UI.WinForms.Tests` (xUnit + NSubstitute + coverlet). La lógica de UI vive separada de
los `Form` (namespace `Backups.Adapters.UI.WinForms.Logica`) para poder testearla sin abrir ventanas reales.
