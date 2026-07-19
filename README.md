# BackupsSolution

Sistema de Gestión de Copias de Seguridad y Réplicas — motor de respaldos asíncronos, multi-protocolo y resiliente en C# .NET 8.0 WinForms.

Arquitectura **Hexagonal (Ports & Adapters)** con separación por grupos de trabajo.

---

## Estado actual

| Grupo | Responsabilidad | Estado |
|---|---|---|
| **Grupo 1** — Núcleo de Control | Orquestación, colas LIFO, Timer, validación SHA256, log TXT | ✅ Completo (51 tests) |
| **Grupo 2** — Compresión | ZIP/RAR/LZMA + segmentación por volumen | ⏳ Stub (pendiente integración) |
| **Grupo 3** — Transmisión | FTP/SFTP/SSH | ⏳ Stub (pendiente integración) |
| **Grupo 4** — UI WinForms | Formulario de configuración y envío | ⏳ Esqueleto |
| **CI/CD** | Build + Test + SonarCloud | ✅ Configurado |

---

## Cambios recientes (respecto a la versión anterior)

- **Migración .NET 10 → .NET 8** (decisión del equipo)
- **Reestructuración completa del Core**: de 4 archivos a 27 con arquitectura hexagonal limpia
- **51 tests** con cobertura: Core 94.9% / Adapters 85.1%
- **Puertos asíncronos** (async/await en todos los puertos de entrada y salida)
- **Log en TXT** (append-only con concurrencia) en vez de JSON
- **Timer periódico** con anti-solapamiento (SemaphoreSlim)
- **Cola LIFO** para gestión de tareas de envío
- **Central Package Management** (CPM) para versiones de NuGet
- **CI/CD** con GitHub Actions + SonarCloud
- **Docker** con SQL Server 2019 + Pure-FTPD + SFTP

---

## Estructura del proyecto

```
BackupsSolution/
├── .github/workflows/build.yml        CI: build + test + SonarCloud
├── BackupsSolution.slnx               Solution file (.slnx)
├── Directory.Build.props              Props globales: LangVersion=latest
├── Directory.Packages.props           Central Package Management
├── global.json                        SDK pin: 8.0.10
├── sonar-project.properties           Config SonarCloud
├── .gitignore
│
├── config/config.json                 Config de la app
├── data/backups/.gitkeep              Carpeta de respaldos
├── data/logs/.gitkeep                 Carpeta de logs
├── docker/                            SQL Server + FTP + SFTP
├── doc/                               Contrato SDD + Requerimientos
│
├── src/
│   ├── Backups.Core/                  Núcleo (Grupo 1)
│   ├── Backups.Adapters.Infrastructure/  Adaptadores driven (Grupo 1)
│   ├── Backups.Infrastructure.Compression/ Stub (Grupo 2)
│   ├── Backups.Infrastructure.Network/    Stub (Grupo 3)
│   ├── Backups.UI/                    UI WinForms nueva (Grupo 4)
│   ├── Backups.Adapters.UI.WinForms/ UI WinForms original (Grupo 4)
│   ├── Backups.Domain/                Placeholder (vacío)
│   └── Backups.Ports/                 Placeholder (vacío)
│
├── tests/
│   ├── Backups.Domain.Tests/          Tests de dominio + arquitectura
│   └── Backups.Adapters.Tests/        Tests de adaptadores + orquestador
│
└── control-nucleo-test/               Directorio legacy (vacío)
```

---

## Backups.Core — Núcleo de Control (Grupo 1)

### Orquestador

| Archivo | Función |
|---|---|
| `OrquestadorRespaldos.cs` | Caso de uso principal: valida solicitud, compara hash SHA256, apila tarea o la descarta como "sin cambios" |
| `ProcesadorPila.cs` | Drena la pila LIFO de tareas y delega compresión + transmisión con transiciones de estado |
| `ServicioTemporizador.cs` | Bucle de ticks con anti-solapamiento (SemaphoreSlim), ejecuta acción por tick en background |

### Dominio

| Archivo | Función |
|---|---|
| `Domain/Enums.cs` | Enums serializables como JSON string: `TipoDisparo`, `AlgoritmoCompresion`, `Protocolo`, `EstadoRespaldo` |
| `Domain/MensajesContrato.cs` | Constantes de texto del contrato |
| `Domain/Entities/SolicitudRespaldo.cs` | Record inmutable con solicitud de respaldo y método `Validar()` |
| `Domain/Entities/TareaRespaldo.cs` | Record ligero (BackupId, Solicitud, HashActual) apilado en la pila |
| `Domain/Entities/RespuestaEjecucion.cs` | DTO de respuesta: BackupId + EstadoInicial |
| `Domain/Entities/RegistroFisico.cs` | Record de una línea del log TXT |
| `Domain/Entities/LogRespaldo.cs` | DTO delgado para el historial |
| `Domain/Entities/DestinoConfig.cs` | Record de configuración de destino de red |

### Puertos

| Puerto | Dirección | Función |
|---|---|---|
| `IEjecutarRespaldoUseCase` | In | `EjecutarAsync(SolicitudRespaldo, CancellationToken)` |
| `IObtenerHistorialUseCase` | In | `ObtenerHistorialAsync(CancellationToken)` |
| `IArchivoPort` | Out | Existe, ObtenerÚltimaModificación, ObtenerTamaño, AbrirLectura |
| `ICalculadoraHashPort` | Out | `CalcularSha256Async(ruta, ct)` |
| `ICompressionService` | Out | `ComprimirYSegmentar(ruta, algoritmo, límiteMb)` |
| `IConfiguracionPort` | Out | ObtenerRutaLogTxt, ObtenerIntervaloTimerSegundos, ObtenerAlgoritmoHashDefault |
| `IPilaEnviosPort` | Out | Pila LIFO FILO: Apilar, TryDesapilar, Cantidad |
| `IRegistroLogPort` | Out | EscribirAsync, LeerHistorialAsync, ObtenerÚltimoHashConfirmadoAsync |
| `ITemporizadorPort` | Out | `IAsyncEnumerable<DateTimeOffset> TicksAsync` + `IAsyncDisposable` |
| `ITransmissionService` | Out | `EnviarArchivos(List<string>, string idDestino)` |

---

## Backups.Adapters.Infrastructure — Adaptadores (Grupo 1)

| Archivo | Implementa | Función |
|---|---|---|
| `ArchivoAdapter.cs` | `IArchivoPort` | Passthrough sobre `System.IO.File` |
| `ConfiguracionAdapter.cs` | `IConfiguracionPort` | Lee `config.json` via `Microsoft.Extensions.Configuration` |
| `LogTxtAdapter.cs` | `IRegistroLogPort` | Log append-only en TXT, serializado con pipe, concurrencia por `SemaphoreSlim` |
| `PilaEnviosMemoria.cs` | `IPilaEnviosPort` | Pila LIFO sobre `ConcurrentStack<TareaRespaldo>` |
| `Sha256HashAdapter.cs` | `ICalculadoraHashPort` | SHA-256 en streaming via `SHA256.HashDataAsync` (.NET 8) |
| `TemporizadorPeriodico.cs` | `ITemporizadorPort` | `PeriodicTimer` (.NET 8) expuesto como `IAsyncEnumerable` |

---

## Otros proyectos

### Backups.Infrastructure.Compression (Grupo 2 — Stub)

`CompressionAdapter.cs` — Stub que imprime por consola y retorna lista fake. Pendiente de implementar ZIP/RAR/LZMA real.

### Backups.Infrastructure.Network (Grupo 3 — Stub)

`NetworkTransmissionAdapter.cs` — Stub que imprime por consola y retorna `true`. Pendiente de implementar FTP/SFTP/SSH real.

### Backups.UI / Backups.Adapters.UI.WinForms (Grupo 4)

Dos formularios WinForms: `FormConfig` (nueva, con botón "Enviar Respaldo") y `Form1` (original, esqueleto con menú).

---

## Tests (51 pruebas)

### tests/Backups.Domain.Tests/ (3 archivos)

| Archivo | Qué testea |
|---|---|
| `SolicitudRespaldoTests.cs` | Validación de campos requeridos, tipos de disparo, algoritmos |
| `EnumSerializacionTests.cs` | Serialización JSON de enums al literal exacto del contrato |
| `ArquitecturaTests.cs` | Reglas arquitectónicas: Core.Domain y Core.Ports no dependen de Adapters |

### tests/Backups.Adapters.Tests/ (7 archivos)

| Archivo | Qué testea |
|---|---|
| `OrquestadorRespaldosTests.cs` | Hash igual → OMITIDO, hash distinto → INICIADO, primera copia, archivo inexistente |
| `ProcesadorPilaTests.cs` | Transiciones COMPRIMIENDO → ENVIANDO → COMPLETADO, fallo de envío |
| `ServicioTemporizadorTests.cs` | Anti-solapamiento (tick omitido si anterior sigue en curso) |
| `PilaEnviosMemoriaTests.cs` | Behavior LIFO (A,B,C → C,B,A) |
| `LogTxtAdapterTests.cs` | Round-trip escribir/leer, 100 escrituras concurrentes, hash confirmado |
| `Sha256HashAdapterTests.cs` | Hash de string vacío conocido, formato hex 64 chars |
| `ConfiguracionAdapterTests.cs` | Lectura de config.json, excepción si falta clave |

**Framework:** xUnit 2.9.3 | **Mocking:** NSubstitute 6.0.0 | **Arquitectura:** NetArchTest.Rules 1.3.2

---

## Flujo del sistema

```
Timer/Botón → OrquestadorRespaldos.EjecutarAsync()
                    │
                    ├── CalcularSha256Async()
                    ├── ObtenerÚltimoHashConfirmadoAsync()
                    │
                    ├── Si CAMBIÓ:
                    │     ├── Registrar "INICIADO"
                    │     ├── Apilar tarea en PilaEnviosPort
                    │     └── ProcesadorPila drena la pila:
                    │           ├── ICompressionService (Grupo 2)
                    │           ├── ITransmissionService (Grupo 3)
                    │           └── Registrar "COMPLETADO" o "FALLIDO"
                    │
                    └── Si NO cambió:
                          └── Registrar "OMITIDO_SIN_CAMBIOS"
```

---

## Estados del respaldo

| Estado | Descripción |
|---|---|
| `OMITIDO_SIN_CAMBIOS` | Archivo sin modificaciones desde el último respaldo |
| `INICIADO` | Respaldo aceptado, en cola de procesamiento |
| `COMPRIMIENDO_VOLUMENES` | Grupo 2 comprimiendo archivos |
| `ENVIANDO` | Grupo 3 transmitiendo por red |
| `COMPLETADO` | Respaldo exitoso |
| `FALLIDO` | Error en cualquier etapa |

---

## Ejecución

```powershell
# Compilar
dotnet build

# Ejecutar tests
dotnet test

# Ejecutar la app WinForms
dotnet run --project src/Backups.UI
```

---

## Tecnologías

| Componente | Tecnología |
|---|---|
| Lenguaje | C# (LangVersion=latest) |
| Framework | .NET 8.0 |
| UI | Windows Forms |
| Testing | xUnit 2.9.3 + NSubstitute 6.0.0 |
| Arquitectura | NetArchTest.Rules 1.3.2 |
| Coverage | coverlet.collector 10.0.1 |
| Config | Microsoft.Extensions.Configuration |
| CI/CD | GitHub Actions + SonarCloud |
| Docker | SQL Server 2019 + Pure-FTPD + atmoz/sftp |
| Contrato | OpenAPI 3.0.3 |

---

## Contrato (SDD)

Definido en `doc/contratoSDD.yaml`:

- `POST /backups/ejecutar` — Ejecuta un respaldo (Timer o botón)
- `GET /backups/historial` — Obtiene el registro de respaldos
- `POST /config/destinos` — Registra un destino de red

---

## Requerimientos funcionales

Definidos en `doc/RFS.txt`:

- **RF2** — Soporte Legacy (DBF/Fox): detectar archivos bloqueados antes de respaldar
- **RF3** — Backup de archivos locales: registrar carpetas y leer recursivamente
- **RF4** — Sincronización dinámica: detectar cambios por hash/fecha y solo copiar lo modificado
