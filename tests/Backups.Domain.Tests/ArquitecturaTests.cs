using Backups.Core.Domain.Entities;
using NetArchTest.Rules;

namespace Backups.Domain.Tests;

public class ArquitecturaTests
{
    // .That().ResideInNamespace(...) acota el escaneo a los tipos propios:
    // coverlet inyecta un tipo Coverlet.Core.Instrumentation.Tracker.* dentro
    // del propio ensamblado al instrumentar cobertura, y ese tipo si usa
    // System.IO, dando falso positivo sin este filtro.
    [Fact]
    public void BackupsCoreDomain_NoDependeDeAdaptersNiInfrastructure()
    {
        var resultado = Types.InAssembly(typeof(SolicitudRespaldo).Assembly)
            .That().ResideInNamespace("Backups.Core.Domain")
            .Should()
            .NotHaveDependencyOnAny("Backups.Adapters", "Backups.Infrastructure")
            .GetResult();

        Assert.True(resultado.IsSuccessful, string.Join(", ", resultado.FailingTypeNames ?? []));
    }

    [Fact]
    public void BackupsCoreDomain_NoUsaSystemIONiSystemNet()
    {
        var resultado = Types.InAssembly(typeof(SolicitudRespaldo).Assembly)
            .That().ResideInNamespace("Backups.Core.Domain")
            .Should()
            .NotHaveDependencyOnAny("System.IO", "System.Net")
            .GetResult();

        Assert.True(resultado.IsSuccessful, string.Join(", ", resultado.FailingTypeNames ?? []));
    }

    [Fact]
    public void BackupsCorePorts_NoDependeDeAdaptersNiInfrastructure()
    {
        var resultado = Types.InAssembly(typeof(SolicitudRespaldo).Assembly)
            .That().ResideInNamespace("Backups.Core.Ports")
            .Should()
            .NotHaveDependencyOnAny("Backups.Adapters", "Backups.Infrastructure")
            .GetResult();

        Assert.True(resultado.IsSuccessful, string.Join(", ", resultado.FailingTypeNames ?? []));
    }
}
