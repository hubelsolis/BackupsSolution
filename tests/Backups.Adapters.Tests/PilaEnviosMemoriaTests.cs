using Backups.Adapters.Infrastructure;
using Backups.Core.Domain;
using Backups.Core.Domain.Entities;

namespace Backups.Adapters.Tests;

public class PilaEnviosMemoriaTests
{
    private static TareaRespaldo CrearTarea(string hash) => new(
        Guid.NewGuid(),
        new SolicitudRespaldo
        {
            NombreCopia = "Copia",
            TipoDisparo = TipoDisparo.TIMER,
            RutaOrigen = "/origen",
            AlgoritmoCompresion = AlgoritmoCompresion.ZIP,
            LimiteVolumenMb = 10,
            IdDestinoConfig = "destino",
        },
        hash);

    [Fact]
    public void Pila_EsFILO_ApilarABC_DesapilarDaCBA()
    {
        var pila = new PilaEnviosMemoria();
        var a = CrearTarea("A");
        var b = CrearTarea("B");
        var c = CrearTarea("C");

        pila.Apilar(a);
        pila.Apilar(b);
        pila.Apilar(c);

        Assert.True(pila.TryDesapilar(out var primero));
        Assert.True(pila.TryDesapilar(out var segundo));
        Assert.True(pila.TryDesapilar(out var tercero));

        Assert.Equal(c, primero);
        Assert.Equal(b, segundo);
        Assert.Equal(a, tercero);
        Assert.False(pila.TryDesapilar(out _));
    }

    [Fact]
    public void Cantidad_ReflejaElNumeroDeElementosApilados()
    {
        var pila = new PilaEnviosMemoria();

        Assert.Equal(0, pila.Cantidad);
        pila.Apilar(CrearTarea("A"));
        pila.Apilar(CrearTarea("B"));
        Assert.Equal(2, pila.Cantidad);
        pila.TryDesapilar(out _);
        Assert.Equal(1, pila.Cantidad);
    }
}
