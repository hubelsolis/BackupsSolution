namespace Backups.Core.Domain.Entities;

/// <summary>Unidad de trabajo apilada en IPilaEnviosPort (LIFO) para que ProcesadorPila la drene.</summary>
public sealed record TareaRespaldo(Guid BackupId, SolicitudRespaldo Solicitud, string HashActual);
