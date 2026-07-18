using System.Globalization;
using System.Text;
using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.Out;

namespace Backups.Adapters.Infrastructure;

/// <summary>
/// Log fisico (TXT). Ruta desde IConfiguracionPort, nunca hardcodeada.
/// Formato: timestampISO8601|backupId|estado|rutaOrigen|hashSha256|tipoDisparo|mensajeTxt
/// Append-only, UTF-8 sin BOM, lecturas/escrituras serializadas con SemaphoreSlim.
/// </summary>
public sealed class LogTxtAdapter : IRegistroLogPort
{
    private const char Separador = '|';
    private static readonly UTF8Encoding Utf8SinBom = new(encoderShouldEmitUTF8Identifier: false);

    private readonly IConfiguracionPort _configuracion;
    private readonly SemaphoreSlim _semaforo = new(1, 1);

    public LogTxtAdapter(IConfiguracionPort configuracion)
    {
        _configuracion = configuracion;
    }

    public async Task EscribirAsync(RegistroFisico registro, CancellationToken ct)
    {
        var ruta = _configuracion.ObtenerRutaLogTxt();
        var linea = FormatearLinea(registro);

        await _semaforo.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            var carpeta = Path.GetDirectoryName(ruta);
            if (!string.IsNullOrEmpty(carpeta) && !Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            var stream = new FileStream(ruta, FileMode.Append, FileAccess.Write, FileShare.Read);
            await using (stream.ConfigureAwait(false))
            {
                var writer = new StreamWriter(stream, Utf8SinBom);
                await using (writer.ConfigureAwait(false))
                {
                    await writer.WriteLineAsync(linea.AsMemory(), ct).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            _semaforo.Release();
        }
    }

    public async Task<IReadOnlyList<LogRespaldo>> LeerHistorialAsync(CancellationToken ct)
    {
        var registros = await LeerTodosAsync(ct).ConfigureAwait(false);
        return registros
            .Select(r => new LogRespaldo { BackupId = r.BackupId, Estado = r.Estado, MensajeTxt = r.MensajeTxt })
            .ToList();
    }

    public async Task<string?> ObtenerUltimoHashConfirmadoAsync(string rutaOrigen, CancellationToken ct)
    {
        var registros = await LeerTodosAsync(ct).ConfigureAwait(false);
        return registros
            .Where(r => string.Equals(r.RutaOrigen, rutaOrigen, StringComparison.OrdinalIgnoreCase)
                        && (r.Estado == EstadoRespaldo.COMPLETADO || r.Estado == EstadoRespaldo.OMITIDO_SIN_CAMBIOS))
            .OrderBy(r => r.Timestamp)
            .Select(r => r.HashSha256)
            .LastOrDefault();
    }

    private async Task<List<RegistroFisico>> LeerTodosAsync(CancellationToken ct)
    {
        var ruta = _configuracion.ObtenerRutaLogTxt();

        await _semaforo.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            if (!File.Exists(ruta))
            {
                return [];
            }

            var lineas = await File.ReadAllLinesAsync(ruta, Encoding.UTF8, ct).ConfigureAwait(false);
            var registros = new List<RegistroFisico>(lineas.Length);
            foreach (var linea in lineas)
            {
                if (TryParsearLinea(linea, out var registro))
                {
                    registros.Add(registro);
                }
            }

            return registros;
        }
        finally
        {
            _semaforo.Release();
        }
    }

    private static string FormatearLinea(RegistroFisico r) => string.Join(
        Separador,
        r.Timestamp.ToString("O", CultureInfo.InvariantCulture),
        r.BackupId,
        r.Estado,
        r.RutaOrigen,
        r.HashSha256,
        r.TipoDisparo,
        r.MensajeTxt);

    private static bool TryParsearLinea(string linea, out RegistroFisico registro)
    {
        var partes = linea.Split(Separador);
        if (partes.Length == 7
            && DateTimeOffset.TryParse(partes[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var timestamp)
            && Guid.TryParse(partes[1], out var backupId)
            && Enum.TryParse<EstadoRespaldo>(partes[2], out var estado)
            && Enum.TryParse<TipoDisparo>(partes[5], out var tipoDisparo))
        {
            registro = new RegistroFisico(timestamp, backupId, estado, partes[3], partes[4], tipoDisparo, partes[6]);
            return true;
        }

        registro = null!;
        return false;
    }
}
