using System.Collections.Concurrent;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.Out;

namespace Backups.Adapters.Infrastructure;

/// <summary>Pila FILO (LIFO): apilar A,B,C -> desapilar da C,B,A. PROHIBIDO Queue/ConcurrentQueue.</summary>
public sealed class PilaEnviosMemoria : IPilaEnviosPort
{
    private readonly ConcurrentStack<TareaRespaldo> _pila = new();

    public void Apilar(TareaRespaldo tarea) => _pila.Push(tarea);

    public bool TryDesapilar(out TareaRespaldo tarea)
    {
        var encontrada = _pila.TryPop(out var resultado);
        tarea = resultado!;
        return encontrada;
    }

    public int Cantidad => _pila.Count;
}
