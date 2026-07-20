using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.In;

namespace Backups.Adapters.UI.WinForms.Logica;

/// <summary>
/// Orquestación de UI -> caso de uso del Grupo 1. No comprime ni envía nada:
/// solo valida los datos capturados, arma el SolicitudRespaldo exacto del
/// contrato y lo entrega a IEjecutarRespaldoUseCase.
/// </summary>
public sealed class SolicitarRespaldoManualService
{
    private readonly IEjecutarRespaldoUseCase _useCase;

    public SolicitarRespaldoManualService(IEjecutarRespaldoUseCase useCase)
    {
        _useCase = useCase;
    }

    public IReadOnlyList<string> Validar(DatosFormularioRespaldo datos)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(datos.RutaOrigen))
        {
            errores.Add("Debe seleccionar la carpeta de origen.");
        }

        if (string.IsNullOrWhiteSpace(datos.NombreCopia))
        {
            errores.Add("Debe indicar un nombre para la copia.");
        }

        if (string.IsNullOrWhiteSpace(datos.IdDestinoConfig))
        {
            errores.Add("Debe seleccionar un destino.");
        }

        if (datos.LimiteVolumenMb <= 0)
        {
            errores.Add("El límite de volumen (MB) debe ser mayor a 0.");
        }

        return errores;
    }

    public SolicitudRespaldo Construir(DatosFormularioRespaldo datos) => new()
    {
        NombreCopia = datos.NombreCopia,
        TipoDisparo = TipoDisparo.BOTON_MANUAL,
        RutaOrigen = datos.RutaOrigen,
        AlgoritmoCompresion = datos.AlgoritmoCompresion,
        LimiteVolumenMb = datos.LimiteVolumenMb,
        IdDestinoConfig = datos.IdDestinoConfig,
    };

    public async Task<ResultadoRespaldo> EjecutarAsync(DatosFormularioRespaldo datos, CancellationToken ct)
    {
        var errores = Validar(datos);
        if (errores.Count > 0)
        {
            return ResultadoRespaldo.Invalido(errores);
        }

        var solicitud = Construir(datos);
        var respuesta = await _useCase.EjecutarAsync(solicitud, ct).ConfigureAwait(true);
        return ResultadoRespaldo.Ok(respuesta);
    }
}
