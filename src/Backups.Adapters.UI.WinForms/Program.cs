using System.Diagnostics.CodeAnalysis;
using Backups.Adapters.UI.WinForms.Adapters;
using Backups.Adapters.UI.WinForms.Logica;
using Backups.Adapters.UI.WinForms.Configuration;
using Backups.Core.Ports.In;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backups.Adapters.UI.WinForms;

// Bootstrapping de la app (composition root): sin lógica de negocio que testear.
[ExcludeFromCodeCoverage(Justification = "Composition root: arma DI y arranca el mensaje de Windows. Sin lógica propia.")]
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        using var serviceProvider = ConfigurarServicios(configuration).BuildServiceProvider();

        Application.Run(serviceProvider.GetRequiredService<FormPrincipal>());
    }

    private static IServiceCollection ConfigurarServicios(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        services.AddSingleton(configuration);
        services.Configure<UiDefaultsOptions>(configuration.GetSection(UiDefaultsOptions.SeccionConfiguracion));
        services.AddLogging(builder => builder.AddDebug());

        services.AddSingleton<AdaptadorPendienteIntegracionGrupo1>();
        services.AddSingleton<IEjecutarRespaldoUseCase>(sp => sp.GetRequiredService<AdaptadorPendienteIntegracionGrupo1>());
        services.AddSingleton<IObtenerHistorialUseCase>(sp => sp.GetRequiredService<AdaptadorPendienteIntegracionGrupo1>());

        services.AddTransient<SolicitarRespaldoManualService>();

        services.AddTransient<FormPrincipal>();
        services.AddTransient<FormHistorialLogs>();
        services.AddTransient<FormConfiguracion>();
        services.AddTransient<Func<FormHistorialLogs>>(sp => () => sp.GetRequiredService<FormHistorialLogs>());
        services.AddTransient<Func<FormConfiguracion>>(sp => () => sp.GetRequiredService<FormConfiguracion>());

        return services;
    }
}
