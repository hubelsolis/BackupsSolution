# BackupsSolution — Grupo 1: Núcleo de Control y Orquestación

## Responsabilidad (doc/RFS.txt)

El Grupo 1 (Mamani, Fabian, Yllescas) es responsable del **Núcleo de Control y Orquestación** del sistema de respaldos. Este módulo gestiona:

- **F1.** Ciclo de vida del proceso de respaldo (Pendiente → En cola → En ejecución → Completado → Registrado)
- **F2.** Ejecución automática de respaldos mediante temporizador (Timer)
- **F3.** Ejecución manual de respaldos desde la interfaz gráfica (botón)
- **F4.** Validación de cambios antes del respaldo (hash SHA-256)
- **F5.** Gestión de la cola de respaldos (tareas pendientes, evitar duplicados)
- **F6.** Registro del estado en archivo TXT físico local

---

## Qué se implementó

### Orquestador (`OrquestadorRespaldos.cs`)

Caso de uso principal que orquesta el flujo completo:

1. Valida la solicitud (campos requeridos)
2. Calcula hash SHA-256 del archivo origen
3. Compara con el último hash confirmado en el log
4. Si **no hay cambios** → descarta y registra `OMITIDO_SIN_CAMBIOS`
5. Si **hay cambios** → registra `INICIADO` y apila la tarea en la pila LIFO

Implementa: `IEjecutarRespaldoUseCase`

### Procesador de pila (`ProcesadorPila.cs`)

Drena la pila LIFO de tareas y ejecuta el flujo de compresión + transmisión:

1. Desapila tarea (LIFO: última en entrar, primera en salir)
2. Transición a `COMPRIMIENDO_VOLUMENES` → llama a `ICompressionService` (Grupo 2)
3. Transición a `ENVIANDO` → llama a `ITransmissionService` (Grupo 3)
4. Registra `COMPLETADO` o `FALLIDO` según el resultado

### Servicio de temporizador (`ServicioTemporizador.cs`)

Bucle de ticks periódicos con anti-solapamiento:

- Usa `PeriodicTimer` (.NET 8) expuesto como `IAsyncEnumerable<DateTimeOffset>`
- **Anti-solapamiento**: si el tick anterior sigue en curso, omite el siguiente (SemaphoreSlim)
- Ejecuta la acción de respaldo en background por cada tick

### Dominio (`Domain/`)

| Archivo | Descripción |
|---|---|
| `Enums.cs` | `TipoDisparo` (TIMER, BOTON_MANUAL), `AlgoritmoCompresion` (ZIP, RAR, LZMA), `Protocolo` (FTP, SFTP, SSH), `EstadoRespaldo` — serializables como JSON string |
| `MensajesContrato.cs` | Constantes de texto del contrato |
| `SolicitudRespaldo.cs` | Record inmutable con solicitud de respaldo y método `Validar()` |
| `TareaRespaldo.cs` | Record ligero (BackupId, Solicitud, HashActual) apilado en la pila |
| `RespuestaEjecucion.cs` | DTO de respuesta: BackupId + EstadoInicial |
| `RegistroFisico.cs` | Record de una línea del log TXT |
| `LogRespaldo.cs` | DTO delgado para el historial |
| `DestinoConfig.cs` | Record de configuración de destino de red |

### Puertos (`Ports/`)

**Puertos de entrada (Use Cases):**

| Puerto | Función |
|---|---|
| `IEjecutarRespaldoUseCase` | `EjecutarAsync(SolicitudRespaldo, CancellationToken)` |
| `IObtenerHistorialUseCase` | `ObtenerHistorialAsync(CancellationToken)` |

**Puertos de salida (interfaces que implementan los adaptadores):**

| Puerto | Función |
|---|---|
| `IArchivoPort` | Existe, ObtenerÚltimaModificación, ObtenerTamaño, AbrirLectura |
| `ICalculadoraHashPort` | `CalcularSha256Async(ruta, ct)` |
| `ICompressionService` | `ComprimirYSegmentar(ruta, algoritmo, límiteMb)` — Grupo 2 |
| `IConfiguracionPort` | ObtenerRutaLogTxt, ObtenerIntervaloTimerSegundos, ObtenerAlgoritmoHashDefault |
| `IPilaEnviosPort` | Pila LIFO FILO: Apilar, TryDesapilar, Cantidad |
| `IRegistroLogPort` | EscribirAsync, LeerHistorialAsync, ObtenerÚltimoHashConfirmadoAsync |
| `ITemporizadorPort` | `IAsyncEnumerable<DateTimeOffset> TicksAsync` + `IAsyncDisposable` |
| `ITransmissionService` | `EnviarArchivos(List<string>, string idDestino)` — Grupo 3 |

### Adaptadores (`Backups.Adapters.Infrastructure/`)

| Adaptador | Implementa | Descripción |
|---|---|---|
| `ArchivoAdapter.cs` | `IArchivoPort` | Passthrough sobre `System.IO.File` |
| `ConfiguracionAdapter.cs` | `IConfiguracionPort` | Lee `config.json` via `Microsoft.Extensions.Configuration` |
| `LogTxtAdapter.cs` | `IRegistroLogPort` | Log append-only en TXT, serializado con pipe, concurrencia por `SemaphoreSlim` |
| `PilaEnviosMemoria.cs` | `IPilaEnviosPort` | Pila LIFO sobre `ConcurrentStack<TareaRespaldo>` |
| `Sha256HashAdapter.cs` | `ICalculadoraHashPort` | SHA-256 en streaming via `SHA256.HashDataAsync` |
| `TemporizadorPeriodico.cs` | `ITemporizadorPort` | `PeriodicTimer` (.NET 8) expuesto como `IAsyncEnumerable` |

---

## Flujo completo

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

## Cobertura por funcionalidad (RFS)

| Funcionalidad | Implementada | Archivos involucrados |
|---|---|---|
| **F1** Ciclo de vida del respaldo | ✅ | `OrquestadorRespaldos.cs`, `RegistroFisico.cs`, `LogTxtAdapter.cs` |
| **F2** Ejecución automática (Timer) | ✅ | `ServicioTemporizador.cs`, `TemporizadorPeriodico.cs` |
| **F3** Ejecución manual (botón) | ✅ | `OrquestadorRespaldos.cs` (recibe solicitud con `TipoDisparo`) |
| **F4** Validación de cambios (SHA-256) | ✅ | `OrquestadorRespaldos.cs`, `Sha256HashAdapter.cs` |
| **F5** Gestión de cola de respaldos | ✅ | `ProcesadorPila.cs`, `PilaEnviosMemoria.cs` |
| **F6** Registro en TXT | ✅ | `LogTxtAdapter.cs`, `RegistroFisico.cs` |

---

## Tests (51 pruebas)

### tests/Backups.Domain.Tests/

| Archivo | Qué testea |
|---|---|
| `SolicitudRespaldoTests.cs` | Validación de campos requeridos, tipos de disparo, algoritmos |
| `EnumSerializacionTests.cs` | Serialización JSON de enums al literal exacto del contrato |
| `ArquitecturaTests.cs` | Core.Domain y Core.Ports no dependen de Adapters (NetArchTest) |

### tests/Backups.Adapters.Tests/

| Archivo | Qué testea |
|---|---|
| `OrquestadorRespaldosTests.cs` | Hash igual → OMITIDO, hash distinto → INICIADO, primera copia, archivo inexistente |
| `ProcesadorPilaTests.cs` | Transiciones COMPRIMIENDO → ENVIANDO → COMPLETADO, fallo de envío |
| `ServicioTemporizadorTests.cs` | Anti-solapamiento (tick omitido si anterior sigue en curso) |
| `PilaEnviosMemoriaTests.cs` | Behavior LIFO (A,B,C → C,B,A) |
| `LogTxtAdapterTests.cs` | Round-trip escribir/leer, 100 escrituras concurrentes, hash confirmado |
| `Sha256HashAdapterTests.cs` | Hash de string vacío conocido, formato hex 64 chars |
| `ConfiguracionAdapterTests.cs` | Lectura de config.json, excepción si falta clave |

**Framework:** xUnit 2.9.3 | **Mocking:** NSubstitute 6.0.0 | **Cobertura:** Core 94.9% / Adapters 85.1%

---

## Ejecución

```powershell
dotnet build
dotnet test
```
