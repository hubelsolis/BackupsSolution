namespace Backups.Core.Ports.Out;

public interface IConfiguracionPort
{
    string ObtenerRutaLogTxt();

    int ObtenerIntervaloTimerSegundos();

    string ObtenerAlgoritmoHashDefault();
}
