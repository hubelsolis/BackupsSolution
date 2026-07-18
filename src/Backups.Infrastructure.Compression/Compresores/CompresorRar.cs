using System.Diagnostics;
using Backups.Infrastructure.Compression.Configuration;

namespace Backups.Infrastructure.Compression.Compresores;

internal sealed class CompresorRar : ICompresor
{
    private readonly IOpcionesCompresion _opciones;

    public CompresorRar(IOpcionesCompresion opciones)
    {
        _opciones = opciones;
    }

    public string Comprimir(string rutaOrigen, string carpetaDestino)
    {
        string? rutaEjecutable = _opciones.RutaEjecutableRar;
        if (string.IsNullOrWhiteSpace(rutaEjecutable) || !File.Exists(rutaEjecutable))
        {
            throw new NotSupportedException(
                "El formato RAR es propietario y no puede crearse sin un ejecutable de WinRAR. " +
                "Configure IOpcionesCompresion.RutaEjecutableRar apuntando a rar.exe para habilitarlo.");
        }

        string rutaRar = ConstruirRutaSalida(rutaOrigen, carpetaDestino);
        var inicio = ConstruirProcessStartInfo(rutaEjecutable, rutaRar, rutaOrigen);

        using Process proceso = Process.Start(inicio)
            ?? throw new InvalidOperationException("No se pudo iniciar el proceso de compresión RAR.");
        proceso.WaitForExit();
        if (proceso.ExitCode != 0)
        {
            string error = proceso.StandardError.ReadToEnd();
            throw new InvalidOperationException($"El proceso RAR finalizó con código {proceso.ExitCode}: {error}");
        }

        return rutaRar;
    }

    internal static string ConstruirRutaSalida(string rutaOrigen, string carpetaDestino)
    {
        string nombreBase = Path.GetFileName(rutaOrigen.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        return Path.Combine(carpetaDestino, nombreBase + ".rar");
    }

    internal static ProcessStartInfo ConstruirProcessStartInfo(string rutaEjecutable, string rutaRar, string rutaOrigen)
    {
        var inicio = new ProcessStartInfo(rutaEjecutable)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        inicio.ArgumentList.Add("a");
        inicio.ArgumentList.Add("-ep1");
        inicio.ArgumentList.Add(rutaRar);
        inicio.ArgumentList.Add(rutaOrigen);
        return inicio;
    }
}
