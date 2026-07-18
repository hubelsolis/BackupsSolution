using Backups.Core.Ports.Out;
using Microsoft.Extensions.Configuration;

namespace Backups.Adapters.Infrastructure;

/// <summary>Lee config.json. La ruta se recibe por parametro (la resuelve el composition root); nada hardcodeado.</summary>
public sealed class ConfiguracionAdapter : IConfiguracionPort
{
    private readonly IConfigurationRoot _configuracion;

    public ConfiguracionAdapter(string rutaConfigJson)
    {
        _configuracion = new ConfigurationBuilder()
            .AddJsonFile(rutaConfigJson, optional: false, reloadOnChange: false)
            .Build();
    }

    public string ObtenerRutaLogTxt() =>
        _configuracion["logging:path"]
        ?? throw new InvalidOperationException("Falta la clave 'logging:path' en config.json.");

    public int ObtenerIntervaloTimerSegundos() =>
        _configuracion.GetValue<int?>("timer:intervaloSegundos")
        ?? throw new InvalidOperationException("Falta la clave 'timer:intervaloSegundos' en config.json.");

    public string ObtenerAlgoritmoHashDefault() =>
        _configuracion["hash:algoritmoDefault"]
        ?? throw new InvalidOperationException("Falta la clave 'hash:algoritmoDefault' en config.json.");
}
