using System.Reflection;
using Backups.Core.Ports.Out;
using Backups.Infrastructure.Network;

namespace Backups.Network.Tests;

// Guarda de la firma congelada: ITransmissionService es el puerto que ProcesadorPila (Grupo 1)
// consume. Grupo 3 lo implementa, no lo rediseña. Si este test falla, alguien cambió la firma.
public sealed class ITransmissionServiceContratoTests
{
    [Fact]
    public void EnviarArchivos_TieneLaFirmaCongelada()
    {
        MethodInfo? metodo = typeof(ITransmissionService).GetMethod("EnviarArchivos");

        Assert.NotNull(metodo);
        Assert.Equal(typeof(bool), metodo.ReturnType);
        ParameterInfo[] parametros = metodo.GetParameters();
        Assert.Equal(2, parametros.Length);
        Assert.Equal(typeof(List<string>), parametros[0].ParameterType);
        Assert.Equal(typeof(string), parametros[1].ParameterType);
    }

    [Fact]
    public void NetworkTransmissionAdapter_ImplementaITransmissionService()
    {
        Assert.True(typeof(ITransmissionService).IsAssignableFrom(typeof(NetworkTransmissionAdapter)));
    }
}
