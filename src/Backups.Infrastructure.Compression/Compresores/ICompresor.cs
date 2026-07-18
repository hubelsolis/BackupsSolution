namespace Backups.Infrastructure.Compression.Compresores;

internal interface ICompresor
{
    string Comprimir(string rutaOrigen, string carpetaDestino);
}
