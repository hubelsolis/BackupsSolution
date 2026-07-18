using Backups.Core.Domain;
using Backups.Core.Domain.Entities;

namespace Backups.Domain.Tests;

public class SolicitudRespaldoTests
{
    private static SolicitudRespaldo CrearSolicitudValida() => new()
    {
        NombreCopia = "Copia de prueba",
        TipoDisparo = TipoDisparo.TIMER,
        RutaOrigen = "/origen/archivo.dat",
        AlgoritmoCompresion = AlgoritmoCompresion.ZIP,
        LimiteVolumenMb = 100,
        IdDestinoConfig = "destino-1",
    };

    [Fact]
    public void Validar_SolicitudCompleta_NoDevuelveErrores()
    {
        Assert.Empty(CrearSolicitudValida().Validar());
    }

    [Theory]
    [InlineData(TipoDisparo.TIMER)]
    [InlineData(TipoDisparo.BOTON_MANUAL)]
    public void Validar_AmbosTiposDeDisparo_SonValidos(TipoDisparo tipoDisparo)
    {
        var solicitud = CrearSolicitudValida() with { TipoDisparo = tipoDisparo };
        Assert.Empty(solicitud.Validar());
    }

    [Theory]
    [InlineData(AlgoritmoCompresion.ZIP)]
    [InlineData(AlgoritmoCompresion.RAR)]
    [InlineData(AlgoritmoCompresion.LZMA)]
    public void Validar_LosTresAlgoritmosDeCompresion_SonAceptados(AlgoritmoCompresion algoritmo)
    {
        var solicitud = CrearSolicitudValida() with { AlgoritmoCompresion = algoritmo };
        Assert.Empty(solicitud.Validar());
    }

    [Fact]
    public void Validar_SinRutaOrigenNiIdDestino_DevuelveErrores()
    {
        var solicitud = CrearSolicitudValida() with { RutaOrigen = "  ", IdDestinoConfig = string.Empty, LimiteVolumenMb = 0 };
        Assert.Equal(3, solicitud.Validar().Count);
    }

    [Fact]
    public void UltimoHashConocido_EsOpcional_PorDefectoEsNull()
    {
        Assert.Null(CrearSolicitudValida().UltimoHashConocido);
    }
}
