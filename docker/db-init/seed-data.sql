-- ============================================================
-- BackupsSolution - Datos de prueba (seed data)
-- ============================================================

USE BackupsSolutionDB;
GO

-- ------------------------------------------------------------
-- Configuracion
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM Configuracion WHERE Clave = N'RutaBackupPorDefecto')
BEGIN
    INSERT INTO Configuracion (Clave, Valor, FechaCreacion) VALUES
    (N'RutaBackupPorDefecto', N'/backups', GETDATE()),
    (N'CompresionHabilitada', N'true', GETDATE()),
    (N'FormatoCompresion', N'7z', GETDATE()),
    (N'IntervaloBackupHoras', N'24', GETDATE());
END
GO

-- ------------------------------------------------------------
-- RegistroBackup
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM RegistroBackup)
BEGIN
    INSERT INTO RegistroBackup (RutaOrigen, RutaDestino, HashArchivo, TamanoBytes, FechaBackup, Estado, Mensaje)
    VALUES
    (N'C:\Datos\Proyectos', N'/backups/proyectos_20260710.7z', N'9F86D081884C7D659A2FEAA0C55AD015A3BF4F1B2B0B822CD15D6C15B0F00A0', 104857600, '2026-07-10 22:00:00', N'Exitoso', N'Backup completado correctamente'),
    (N'C:\Datos\Facturacion', N'/backups/facturacion_20260711.rar', N'D2D2D2A3F1B0B0F5A0A1B2C3D4E5F60718293A4B5C6D7E8F90A1B2C3D4E5F60', 52428800, '2026-07-11 22:00:00', N'Exitoso', N'Backup completado correctamente'),
    (N'C:\Datos\ClientesDB', N'/backups/clientesdb_20260712.7z', N'A1B2C3D4E5F60718293A4B5C6D7E8F90A1B2C3D4E5F60718293A4B5C6D7E8F9', 209715200, '2026-07-12 22:00:00', N'Fallido', N'Error de conexión al servidor FTP'),
    (N'C:\Datos\Contratos', N'/backups/contratos_20260713.7z', N'3C4D5E6F708192A3B4C5D6E7F8091A2B3C4D5E6F708192A3B4C5D6E7F8091A2', 15728640, '2026-07-13 22:00:00', N'Exitoso', N'Backup completado correctamente'),
    (N'C:\Datos\RecursosHumanos', N'/backups/rrhh_20260714.rar', N'7E8F9081A2B3C4D5E6F708192A3B4C5D6E7F8091A2B3C4D5E6F708192A3B4C5', 78643200, '2026-07-14 22:00:00', N'EnProceso', N'Transferencia en curso hacia SFTP');
END
GO

-- ------------------------------------------------------------
-- Log
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM Log)
BEGIN
    INSERT INTO Log (Nivel, Mensaje, FechaRegistro)
    VALUES
    (N'Information', N'Servicio de backups iniciado correctamente', '2026-07-10 21:55:00'),
    (N'Error', N'No se pudo conectar al servidor FTP en el intento de backup de ClientesDB', '2026-07-12 22:05:00'),
    (N'Warning', N'El backup de RecursosHumanos está tardando más de lo esperado', '2026-07-14 22:20:00');
END
GO
