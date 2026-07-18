-- ============================================================
-- BackupsSolution - Script de creación de base de datos y tablas
-- ============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BackupsSolutionDB')
BEGIN
    CREATE DATABASE BackupsSolutionDB;
END
GO

USE BackupsSolutionDB;
GO

-- ------------------------------------------------------------
-- Tabla: Configuracion
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT object_id FROM sys.tables WHERE name = N'Configuracion')
BEGIN
    CREATE TABLE Configuracion
    (
        ConfiguracionId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Clave           NVARCHAR(100)     NOT NULL,
        Valor           NVARCHAR(MAX)     NOT NULL,
        FechaCreacion   DATETIME          NOT NULL CONSTRAINT DF_Configuracion_FechaCreacion DEFAULT (GETDATE()),
        CONSTRAINT UQ_Configuracion_Clave UNIQUE (Clave)
    );
END
GO

-- ------------------------------------------------------------
-- Tabla: RegistroBackup
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT object_id FROM sys.tables WHERE name = N'RegistroBackup')
BEGIN
    CREATE TABLE RegistroBackup
    (
        RegistroId    INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        RutaOrigen    NVARCHAR(500)     NOT NULL,
        RutaDestino   NVARCHAR(500)     NOT NULL,
        HashArchivo   NVARCHAR(64)      NOT NULL,
        TamanoBytes   BIGINT            NOT NULL,
        FechaBackup   DATETIME          NOT NULL CONSTRAINT DF_RegistroBackup_FechaBackup DEFAULT (GETDATE()),
        Estado        NVARCHAR(20)      NOT NULL,
        Mensaje       NVARCHAR(500)     NULL
    );
END
GO

-- ------------------------------------------------------------
-- Tabla: Log
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT object_id FROM sys.tables WHERE name = N'Log')
BEGIN
    CREATE TABLE Log
    (
        LogId          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nivel          NVARCHAR(20)      NOT NULL,
        Mensaje        NVARCHAR(MAX)     NOT NULL,
        FechaRegistro  DATETIME          NOT NULL CONSTRAINT DF_Log_FechaRegistro DEFAULT (GETDATE())
    );
END
GO

-- ------------------------------------------------------------
-- Índices
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_RegistroBackup_FechaBackup')
BEGIN
    CREATE INDEX IX_RegistroBackup_FechaBackup ON RegistroBackup (FechaBackup);
END
GO

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_RegistroBackup_Estado')
BEGIN
    CREATE INDEX IX_RegistroBackup_Estado ON RegistroBackup (Estado);
END
GO

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Log_FechaRegistro')
BEGIN
    CREATE INDEX IX_Log_FechaRegistro ON Log (FechaRegistro);
END
GO
