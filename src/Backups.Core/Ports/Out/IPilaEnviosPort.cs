using Backups.Core.Domain.Entities;

namespace Backups.Core.Ports.Out;

/// <summary>Pila FILO (LIFO). PROHIBIDO Queue/ConcurrentQueue.</summary>
public interface IPilaEnviosPort
{
    void Apilar(TareaRespaldo tarea);

    bool TryDesapilar(out TareaRespaldo tarea);

    int Cantidad { get; }
}
