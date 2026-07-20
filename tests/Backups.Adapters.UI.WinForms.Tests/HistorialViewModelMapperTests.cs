using Backups.Adapters.UI.WinForms.Logica;
using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.In;
using NSubstitute;

namespace Backups.Adapters.UI.WinForms.Tests;

public class HistorialViewModelMapperTests
{
    [Fact]
    public void AFilas_MapeaBackupIdEstadoYMensajeDeCadaLogRespaldo()
    {
        var backupId = Guid.NewGuid();
        var historial = new List<LogRespaldo>
        {
            new()
            {
                BackupId = backupId,
                Estado = EstadoRespaldo.OMITIDO_SIN_CAMBIOS,
                MensajeTxt = "Copia cancelada: sin cambios.",
            },
        };

        var filas = HistorialViewModelMapper.AFilas(historial);

        var fila = Assert.Single(filas);
        Assert.Equal(backupId.ToString(), fila.BackupId);
        Assert.Equal("OMITIDO_SIN_CAMBIOS", fila.Estado);
        Assert.Equal("Copia cancelada: sin cambios.", fila.MensajeTxt);
    }

    [Fact]
    public void AFilas_ConHistorialVacio_DevuelveListaVacia()
    {
        var filas = HistorialViewModelMapper.AFilas(Array.Empty<LogRespaldo>());

        Assert.Empty(filas);
    }

    [Fact]
    public async Task LaUiLeeElHistorialInvocandoElCasoDeUsoDelGrupo1()
    {
        var useCase = Substitute.For<IObtenerHistorialUseCase>();
        var esperado = new List<LogRespaldo>
        {
            new() { BackupId = Guid.NewGuid(), Estado = EstadoRespaldo.COMPLETADO, MensajeTxt = "ok" },
        };
        useCase.ObtenerHistorialAsync(Arg.Any<CancellationToken>()).Returns(esperado);

        var historial = await useCase.ObtenerHistorialAsync(CancellationToken.None);
        var filas = HistorialViewModelMapper.AFilas(historial);

        Assert.Single(filas);
        await useCase.Received(1).ObtenerHistorialAsync(Arg.Any<CancellationToken>());
    }
}
