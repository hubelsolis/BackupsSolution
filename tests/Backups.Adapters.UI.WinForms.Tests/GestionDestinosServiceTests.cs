using Backups.Adapters.UI.WinForms.Logica;

namespace Backups.Adapters.UI.WinForms.Tests;

public class GestionDestinosServiceTests
{
    [Fact]
    public void TryAgregar_ConValorNuevo_LoAgregaYDevuelveTrue()
    {
        var destinos = new List<string> { "DEST_SFTP_SECURE" };

        var agregado = GestionDestinosService.TryAgregar("DEST_FTP_LOCAL", destinos, out var error);

        Assert.True(agregado);
        Assert.Null(error);
        Assert.Contains("DEST_FTP_LOCAL", destinos);
    }

    [Fact]
    public void TryAgregar_ConValorVacio_NoAgregaYDevuelveError()
    {
        var destinos = new List<string>();

        var agregado = GestionDestinosService.TryAgregar("   ", destinos, out var error);

        Assert.False(agregado);
        Assert.NotNull(error);
        Assert.Empty(destinos);
    }

    [Fact]
    public void TryAgregar_ConValorDuplicado_NoAgregaYDevuelveError()
    {
        var destinos = new List<string> { "DEST_SFTP_SECURE" };

        var agregado = GestionDestinosService.TryAgregar("dest_sftp_secure", destinos, out var error);

        Assert.False(agregado);
        Assert.NotNull(error);
        Assert.Single(destinos);
    }
}
