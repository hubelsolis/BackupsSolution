namespace Backups.Infrastructure.Compression.Volumenes;

internal static class SegmentadorVolumenes
{
    private const int TamanoBuffer = 81920;

    public static List<string> Segmentar(string rutaArchivo, int limiteVolumenMb)
    {
        if (limiteVolumenMb <= 0)
        {
            return new List<string> { rutaArchivo };
        }

        long limiteBytes = (long)limiteVolumenMb * 1024 * 1024;
        var info = new FileInfo(rutaArchivo);
        if (info.Length <= limiteBytes)
        {
            return new List<string> { rutaArchivo };
        }

        var rutasVolumenes = new List<string>();
        var buffer = new byte[TamanoBuffer];
        int numeroVolumen = 1;

        using (var origen = File.OpenRead(rutaArchivo))
        {
            while (origen.Position < origen.Length)
            {
                string rutaVolumen = $"{rutaArchivo}.{numeroVolumen:D3}";
                long restanteVolumen = limiteBytes;

                using (var destino = File.Create(rutaVolumen))
                {
                    while (restanteVolumen > 0)
                    {
                        int aLeer = (int)Math.Min(TamanoBuffer, restanteVolumen);
                        int leidos = origen.Read(buffer, 0, aLeer);
                        if (leidos == 0)
                        {
                            break;
                        }

                        destino.Write(buffer, 0, leidos);
                        restanteVolumen -= leidos;
                    }
                }

                rutasVolumenes.Add(rutaVolumen);
                numeroVolumen++;
            }
        }

        File.Delete(rutaArchivo);
        return rutasVolumenes;
    }
}
