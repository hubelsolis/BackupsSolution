using Backups.Adapters.UI.WinForms.Adapters;
using Backups.Core.Domain;
using Backups.Core.Domain.Entities;

namespace Backups.Adapters.UI.WinForms.Tests;

/// <summary>
/// INTEGRACIÓN Grupo4+Grupo1: a la fecha, main no trae el OrquestadorRespaldos
/// real (vive sin mergear en nucleo-de-control), así que no hay caso de uso
/// real contra el cual probar el flujo completo UI -> Grupo1. Se valida en su
/// lugar el adaptador temporal propio que mantiene viva la UI mientras tanto;
/// el test de integración con el caso de uso real se agregará al mergear
/// nucleo-de-control (ver CHANGELOG.md).
/// </summary>
public class AdaptadorPendienteIntegracionGrupo1Tests
{
    [Fact]
    public async Task EjecutarAsync_RegistraElResultadoYQuedaDisponibleEnElHistorial()
    {
        var adaptador = new AdaptadorPendienteIntegracionGrupo1();
        var solicitud = new SolicitudRespaldo
        {
            NombreCopia = "Respaldo_Test",
            TipoDisparo = TipoDisparo.BOTON_MANUAL,
            RutaOrigen = @"D:\Origen",
            AlgoritmoCompresion = AlgoritmoCompresion.ZIP,
            LimiteVolumenMb = 100,
            IdDestinoConfig = "DEST_SFTP_SECURE",
        };

        var respuesta = await adaptador.EjecutarAsync(solicitud, CancellationToken.None);
        var historial = await adaptador.ObtenerHistorialAsync(CancellationToken.None);

        Assert.NotEqual(Guid.Empty, respuesta.BackupId);
        var registro = Assert.Single(historial);
        Assert.Equal(respuesta.BackupId, registro.BackupId);
        Assert.Contains("Respaldo_Test", registro.MensajeTxt);
    }
}
