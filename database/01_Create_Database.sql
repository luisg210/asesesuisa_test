IF DB_ID (N'ConsultoraDb') IS NULL BEGIN CREATE DATABASE ConsultoraDb;
END
GO

USE ConsultoraDb;

GO
IF OBJECT_ID (N'dbo.RefreshTokens', N'U') IS NOT NULL
DROP TABLE dbo.RefreshTokens;

IF OBJECT_ID (N'dbo.Usuarios', N'U') IS NOT NULL
DROP TABLE dbo.Usuarios;

IF OBJECT_ID (N'dbo.ConsultorPaquete', N'U') IS NOT NULL
DROP TABLE dbo.ConsultorPaquete;

IF OBJECT_ID (N'dbo.Consultores', N'U') IS NOT NULL
DROP TABLE dbo.Consultores;

IF OBJECT_ID (N'dbo.Auditoria', N'U') IS NOT NULL
DROP TABLE dbo.Auditoria;

IF OBJECT_ID (N'dbo.Paquetes', N'U') IS NOT NULL
DROP TABLE dbo.Paquetes;

GO
-- ----------------------------------------------------------------------------
-- Paquetes: paquetes de servicio ofrecidos por la firma.
-- ----------------------------------------------------------------------------
CREATE TABLE
    dbo.Paquetes (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        Nombre NVARCHAR (120) NOT NULL,
        Descripcion NVARCHAR (500) NULL,
        Area NVARCHAR (80) NOT NULL,
        Precio DECIMAL(12, 2) NOT NULL CONSTRAINT CK_Paquetes_Precio CHECK (Precio >= 0),
        Activo BIT NOT NULL CONSTRAINT DF_Paquetes_Activo DEFAULT 1,
        FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Paquetes_FechaCreacion DEFAULT SYSUTCDATETIME ()
    );

-- ----------------------------------------------------------------------------
-- Consultores: consultores de la firma.
-- ----------------------------------------------------------------------------
CREATE TABLE
    dbo.Consultores (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        NombreCompleto NVARCHAR (150) NOT NULL,
        Email NVARCHAR (150) NOT NULL,
        Area NVARCHAR (80) NOT NULL,
        TarifaHora DECIMAL(10, 2) NOT NULL,
        Activo BIT NOT NULL CONSTRAINT DF_Consultores_Activo DEFAULT 1,
        ProyectosActivos INT NOT NULL CONSTRAINT DF_Consultores_ProyectosActivos DEFAULT 0,
        FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Consultores_FechaCreacion DEFAULT SYSUTCDATETIME ()
    );

-- ----------------------------------------------------------------------------
-- Usuarios: cuentas de acceso al sistema (Admin / User).
-- ----------------------------------------------------------------------------
CREATE TABLE
    dbo.Usuarios (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        Email NVARCHAR (150) NOT NULL,
        PasswordHash NVARCHAR (255) NOT NULL,
        Rol NVARCHAR (10) NOT NULL CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN (N'Admin', N'User'))
    );

-- ----------------------------------------------------------------------------
-- ConsultorPaquete: relacion N:N consultor <-> paquete
-- (funcionalidad opcional: asignacion de consultores a paquetes).
-- Se elimina logicamente en los padres, por lo que no requiere CASCADE.
-- ----------------------------------------------------------------------------
CREATE TABLE
    dbo.ConsultorPaquete (
        ConsultorId INT NOT NULL CONSTRAINT FK_ConsultorPaquete_Consultor REFERENCES dbo.Consultores (Id),
        PaqueteId INT NOT NULL CONSTRAINT FK_ConsultorPaquete_Paquete REFERENCES dbo.Paquetes (Id),
        FechaAsignacion DATETIME2 NOT NULL CONSTRAINT DF_ConsultorPaquete_FechaAsignacion DEFAULT SYSUTCDATETIME (),
        CONSTRAINT PK_ConsultorPaquete PRIMARY KEY (ConsultorId, PaqueteId)
    );

-- ----------------------------------------------------------------------------
-- Auditoria: bitacora de escrituras (quien, que entidad, cuando y desde donde).
-- ----------------------------------------------------------------------------
CREATE TABLE
    dbo.Auditoria (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        Usuario NVARCHAR (150) NOT NULL,
        Accion NVARCHAR (30) NOT NULL,
        Entidad NVARCHAR (50) NOT NULL,
        EntidadId INT NULL,
        Detalle NVARCHAR (500) NULL,
        Ip NVARCHAR (45) NULL,
        FechaHora DATETIME2 NOT NULL CONSTRAINT DF_Auditoria_FechaHora DEFAULT SYSUTCDATETIME ()
    );

CREATE UNIQUE INDEX UX_Consultores_NombreCompleto_Area ON dbo.Consultores (NombreCompleto, Area);

CREATE UNIQUE INDEX UX_Consultores_Email ON dbo.Consultores (Email);

CREATE UNIQUE INDEX UX_Usuarios_Email ON dbo.Usuarios (Email);

CREATE INDEX IX_Auditoria_FechaHora ON dbo.Auditoria (FechaHora DESC);

CREATE INDEX IX_ConsultorPaquete_PaqueteId ON dbo.ConsultorPaquete (PaqueteId);

-- ----------------------------------------------------------------------------
-- RefreshTokens: tokens opacos de refresco para renovar el JWT sin reautenticar.
-- Se almacena solo el hash (SHA-256) del token; el valor en claro solo se entrega
-- una vez en el login/refresh.
-- ----------------------------------------------------------------------------
CREATE TABLE
    dbo.RefreshTokens (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        UsuarioId INT NOT NULL CONSTRAINT FK_RefreshTokens_Usuario REFERENCES dbo.Usuarios (Id),
        TokenHash CHAR (64) NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT SYSUTCDATETIME (),
        RevokedAt DATETIME2 NULL,
        Ip NVARCHAR (45) NULL
    );

CREATE UNIQUE INDEX UX_RefreshTokens_TokenHash ON dbo.RefreshTokens (TokenHash);

CREATE INDEX IX_RefreshTokens_UsuarioId ON dbo.RefreshTokens (UsuarioId);

GO

PRINT N'Base de datos y tablas creadas correctamente.';

GO