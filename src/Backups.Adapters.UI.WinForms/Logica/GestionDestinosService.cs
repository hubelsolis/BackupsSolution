namespace Backups.Adapters.UI.WinForms.Logica;

/// <summary>Reglas para administrar la lista de destinos disponibles en el formulario de configuración.</summary>
public static class GestionDestinosService
{
    public static bool TryAgregar(string valorCrudo, ICollection<string> destinos, out string? error)
    {
        var valor = valorCrudo?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(valor))
        {
            error = "El ID de destino no puede estar vacío.";
            return false;
        }

        if (destinos.Contains(valor, StringComparer.OrdinalIgnoreCase))
        {
            error = $"El destino '{valor}' ya está en la lista.";
            return false;
        }

        destinos.Add(valor);
        error = null;
        return true;
    }
}
