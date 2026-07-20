using Backups.Core.Domain;

namespace Backups.Adapters.UI.WinForms.Tests;

/// <summary>
/// Guardia de regresión: los enums que consume la UI deben mantener exactamente
/// los literales del contrato OpenAPI v1.3.0 (SolicitudRespaldo.tipoDisparo /
/// algoritmoCompresion). Si el Grupo 1 cambia estos nombres, este test debe
/// romper para forzar la actualización de la UI.
/// </summary>
public class ContratoConsistenciaTests
{
    [Fact]
    public void TipoDisparo_TieneExactamenteLosLiteralesDelContrato()
    {
        var literales = Enum.GetNames<TipoDisparo>();

        Assert.Equal(new[] { "TIMER", "BOTON_MANUAL" }, literales);
    }

    [Fact]
    public void AlgoritmoCompresion_TieneExactamenteLosLiteralesDelContrato()
    {
        var literales = Enum.GetNames<AlgoritmoCompresion>();

        Assert.Equal(new[] { "ZIP", "RAR", "LZMA" }, literales);
    }

    [Fact]
    public void EstadoRespaldo_TieneExactamenteLosLiteralesDelContrato()
    {
        var literales = Enum.GetNames<EstadoRespaldo>();

        Assert.Equal(
            new[] { "OMITIDO_SIN_CAMBIOS", "PROCESANDO", "COMPRIMIENDO_VOLUMENES", "ENVIANDO", "COMPLETADO", "FALLIDO" },
            literales);
    }
}
