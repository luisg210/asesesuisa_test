USE ConsultoraDb;
GO

-- ============================================================================
-- AUTENTICACION
-- ============================================================================

GO
IF OBJECT_ID(N'dbo.sp_Login', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Login;
GO
CREATE PROCEDURE dbo.sp_Login
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Email, PasswordHash, Rol
    FROM dbo.Usuarios
    WHERE Email = @Email;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Usuario_GetById', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Usuario_GetById;
GO
CREATE PROCEDURE dbo.sp_Usuario_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Email, PasswordHash, Rol
    FROM dbo.Usuarios
    WHERE Id = @Id;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_RefreshTokens_Insert', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_RefreshTokens_Insert;
GO
CREATE PROCEDURE dbo.sp_RefreshTokens_Insert
    @UsuarioId INT,
    @TokenHash CHAR(64),
    @ExpiresAt DATETIME2,
    @Ip        NVARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.RefreshTokens (UsuarioId, TokenHash, ExpiresAt, Ip)
    VALUES (@UsuarioId, @TokenHash, @ExpiresAt, @Ip);

    SELECT SCOPE_IDENTITY() AS Id;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_RefreshTokens_GetByHash', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_RefreshTokens_GetByHash;
GO
CREATE PROCEDURE dbo.sp_RefreshTokens_GetByHash
    @TokenHash CHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UsuarioId, TokenHash, ExpiresAt, RevokedAt, Ip
    FROM dbo.RefreshTokens
    WHERE TokenHash = @TokenHash;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_RefreshTokens_Revoke', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_RefreshTokens_Revoke;
GO
CREATE PROCEDURE dbo.sp_RefreshTokens_Revoke
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.RefreshTokens
    SET RevokedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND RevokedAt IS NULL;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_RefreshTokens_RevokeAllByUser', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_RefreshTokens_RevokeAllByUser;
GO
CREATE PROCEDURE dbo.sp_RefreshTokens_RevokeAllByUser
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.RefreshTokens
    SET RevokedAt = SYSUTCDATETIME()
    WHERE UsuarioId = @UsuarioId AND RevokedAt IS NULL;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

-- ============================================================================
-- PAQUETES
-- ============================================================================

GO
IF OBJECT_ID(N'dbo.sp_Paquetes_List', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Paquetes_List;
GO
CREATE PROCEDURE dbo.sp_Paquetes_List
    @Page      INT           = 1,
    @PageSize  INT           = 10,
    @SortBy    NVARCHAR(50)  = N'Id',
    @SortDir   NVARCHAR(4)   = N'asc',
    @Nombre    NVARCHAR(120) = NULL,
    @Area      NVARCHAR(80)  = NULL,
    @Activo    BIT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT Id, Nombre, Descripcion, Area, Precio, Activo, FechaCreacion,
           COUNT(*) OVER() AS TotalCount
    FROM dbo.Paquetes
    WHERE (@Nombre IS NULL OR Nombre LIKE N'%' + @Nombre + N'%')
      AND (@Area   IS NULL OR Area = @Area)
      AND (@Activo IS NULL OR Activo = @Activo)
    ORDER BY
        CASE WHEN @SortBy = N'Nombre'       AND @SortDir = N'asc'  THEN Nombre       END ASC,
        CASE WHEN @SortBy = N'Nombre'       AND @SortDir = N'desc' THEN Nombre       END DESC,
        CASE WHEN @SortBy = N'Area'         AND @SortDir = N'asc'  THEN Area         END ASC,
        CASE WHEN @SortBy = N'Area'         AND @SortDir = N'desc' THEN Area         END DESC,
        CASE WHEN @SortBy = N'Precio'       AND @SortDir = N'asc'  THEN Precio       END ASC,
        CASE WHEN @SortBy = N'Precio'       AND @SortDir = N'desc' THEN Precio       END DESC,
        CASE WHEN @SortBy = N'FechaCreacion' AND @SortDir = N'asc' THEN FechaCreacion END ASC,
        CASE WHEN @SortBy = N'FechaCreacion' AND @SortDir = N'desc' THEN FechaCreacion END DESC,
        Id ASC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Paquetes_GetById', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Paquetes_GetById;
GO
CREATE PROCEDURE dbo.sp_Paquetes_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Nombre, Descripcion, Area, Precio, Activo, FechaCreacion
    FROM dbo.Paquetes
    WHERE Id = @Id;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Paquetes_Insert', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Paquetes_Insert;
GO
CREATE PROCEDURE dbo.sp_Paquetes_Insert
    @Nombre      NVARCHAR(120),
    @Descripcion NVARCHAR(500),
    @Area        NVARCHAR(80),
    @Precio      DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Paquetes (Nombre, Descripcion, Area, Precio)
    VALUES (@Nombre, @Descripcion, @Area, @Precio);

    SELECT SCOPE_IDENTITY() AS Id;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Paquetes_Update', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Paquetes_Update;
GO
CREATE PROCEDURE dbo.sp_Paquetes_Update
    @Id          INT,
    @Nombre      NVARCHAR(120),
    @Descripcion NVARCHAR(500),
    @Area        NVARCHAR(80),
    @Precio      DECIMAL(12,2),
    @Activo      BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Paquetes
    SET Nombre = @Nombre,
        Descripcion = @Descripcion,
        Area = @Area,
        Precio = @Precio,
        Activo = @Activo
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Paquetes_Delete', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Paquetes_Delete;
GO
CREATE PROCEDURE dbo.sp_Paquetes_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Eliminacion logica: preserve historial en reportes.
    UPDATE dbo.Paquetes
    SET Activo = 0
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

-- ============================================================================
-- CONSULTORES
-- ============================================================================

GO
IF OBJECT_ID(N'dbo.sp_Consultores_List', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Consultores_List;
GO
CREATE PROCEDURE dbo.sp_Consultores_List
    @Page     INT           = 1,
    @PageSize INT           = 10,
    @SortBy   NVARCHAR(50)  = N'Id',
    @SortDir  NVARCHAR(4)   = N'asc',
    @Nombre   NVARCHAR(150) = NULL,
    @Area     NVARCHAR(80)  = NULL,
    @Activo   BIT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT Id, NombreCompleto, Email, Area, TarifaHora, Activo, ProyectosActivos, FechaCreacion,
           COUNT(*) OVER() AS TotalCount
    FROM dbo.Consultores
    WHERE (@Nombre IS NULL OR NombreCompleto LIKE N'%' + @Nombre + N'%')
      AND (@Area   IS NULL OR Area = @Area)
      AND (@Activo IS NULL OR Activo = @Activo)
    ORDER BY
        CASE WHEN @SortBy = N'NombreCompleto' AND @SortDir = N'asc'  THEN NombreCompleto  END ASC,
        CASE WHEN @SortBy = N'NombreCompleto' AND @SortDir = N'desc' THEN NombreCompleto  END DESC,
        CASE WHEN @SortBy = N'Email'          AND @SortDir = N'asc'  THEN Email           END ASC,
        CASE WHEN @SortBy = N'Email'          AND @SortDir = N'desc' THEN Email           END DESC,
        CASE WHEN @SortBy = N'Area'           AND @SortDir = N'asc'  THEN Area            END ASC,
        CASE WHEN @SortBy = N'Area'           AND @SortDir = N'desc' THEN Area            END DESC,
        CASE WHEN @SortBy = N'TarifaHora'     AND @SortDir = N'asc'  THEN TarifaHora      END ASC,
        CASE WHEN @SortBy = N'TarifaHora'     AND @SortDir = N'desc' THEN TarifaHora      END DESC,
        CASE WHEN @SortBy = N'ProyectosActivos' AND @SortDir = N'asc' THEN ProyectosActivos END ASC,
        CASE WHEN @SortBy = N'ProyectosActivos' AND @SortDir = N'desc' THEN ProyectosActivos END DESC,
        CASE WHEN @SortBy = N'FechaCreacion'  AND @SortDir = N'asc'  THEN FechaCreacion   END ASC,
        CASE WHEN @SortBy = N'FechaCreacion'  AND @SortDir = N'desc' THEN FechaCreacion   END DESC,
        Id ASC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Consultores_GetById', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Consultores_GetById;
GO
CREATE PROCEDURE dbo.sp_Consultores_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, NombreCompleto, Email, Area, TarifaHora, Activo, ProyectosActivos, FechaCreacion
    FROM dbo.Consultores
    WHERE Id = @Id;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Consultores_Insert', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Consultores_Insert;
GO
CREATE PROCEDURE dbo.sp_Consultores_Insert
    @NombreCompleto  NVARCHAR(150),
    @Email           NVARCHAR(150),
    @Area            NVARCHAR(80),
    @TarifaHora      DECIMAL(10,2),
    @ProyectosActivos INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Consultores (NombreCompleto, Email, Area, TarifaHora, ProyectosActivos)
    VALUES (@NombreCompleto, @Email, @Area, @TarifaHora, @ProyectosActivos);

    SELECT SCOPE_IDENTITY() AS Id;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Consultores_Update', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Consultores_Update;
GO
CREATE PROCEDURE dbo.sp_Consultores_Update
    @Id               INT,
    @NombreCompleto   NVARCHAR(150),
    @Email            NVARCHAR(150),
    @Area             NVARCHAR(80),
    @TarifaHora       DECIMAL(10,2),
    @Activo           BIT,
    @ProyectosActivos INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Consultores
    SET NombreCompleto = @NombreCompleto,
        Email = @Email,
        Area = @Area,
        TarifaHora = @TarifaHora,
        Activo = @Activo,
        ProyectosActivos = @ProyectosActivos
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Consultores_Delete', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Consultores_Delete;
GO
CREATE PROCEDURE dbo.sp_Consultores_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Eliminacion logica: preserve historial en reportes.
    UPDATE dbo.Consultores
    SET Activo = 0
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

-- ============================================================================
-- REPORTES
-- ============================================================================

GO
IF OBJECT_ID(N'dbo.sp_Reporte_PaquetesPorArea', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Reporte_PaquetesPorArea;
GO
CREATE PROCEDURE dbo.sp_Reporte_PaquetesPorArea
    @Page     INT           = 1,
    @PageSize INT           = 10,
    @SortBy   NVARCHAR(50)  = N'Area',
    @SortDir  NVARCHAR(4)   = N'asc',
    @Area     NVARCHAR(80)  = NULL,
    @Activo   BIT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT Area,
           COUNT(*)      AS TotalPaquetes,
           SUM(Precio)   AS TotalMonto,
           MIN(Precio)   AS PrecioMinimo,
           MAX(Precio)   AS PrecioMaximo,
           COUNT(*) OVER() AS TotalCount
    FROM dbo.Paquetes
    WHERE (@Area   IS NULL OR Area = @Area)
      AND (@Activo IS NULL OR Activo = @Activo)
    GROUP BY Area
    ORDER BY
        CASE WHEN @SortBy = N'Area'         AND @SortDir = N'asc'  THEN Area        END ASC,
        CASE WHEN @SortBy = N'Area'         AND @SortDir = N'desc' THEN Area        END DESC,
        CASE WHEN @SortBy = N'TotalPaquetes' AND @SortDir = N'asc' THEN COUNT(*)     END ASC,
        CASE WHEN @SortBy = N'TotalPaquetes' AND @SortDir = N'desc' THEN COUNT(*)    END DESC,
        CASE WHEN @SortBy = N'TotalMonto'   AND @SortDir = N'asc'  THEN SUM(Precio)  END ASC,
        CASE WHEN @SortBy = N'TotalMonto'   AND @SortDir = N'desc' THEN SUM(Precio)  END DESC,
        Area ASC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Reporte_ConsultoresTopFacturacion', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Reporte_ConsultoresTopFacturacion;
GO
CREATE PROCEDURE dbo.sp_Reporte_ConsultoresTopFacturacion
    @Page     INT          = 1,
    @PageSize INT          = 10,
    @SortBy   NVARCHAR(50) = N'FacturacionEstimada',
    @SortDir  NVARCHAR(4)  = N'desc',
    @Area     NVARCHAR(80) = NULL,
    @Activo   BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    -- Regla documentada: facturacion_estimada = TarifaHora * 160 horas/mes * ProyectosActivos.
    DECLARE @HorasMensuales INT = 160;

    SELECT Id,
           NombreCompleto,
           Email,
           Area,
           TarifaHora,
           ProyectosActivos,
           (TarifaHora * @HorasMensuales * ProyectosActivos) AS FacturacionEstimada,
           COUNT(*) OVER() AS TotalCount
    FROM dbo.Consultores
    WHERE (@Area   IS NULL OR Area = @Area)
      AND (@Activo IS NULL OR Activo = @Activo)
    ORDER BY
        CASE WHEN @SortBy = N'NombreCompleto'      AND @SortDir = N'asc'  THEN NombreCompleto       END ASC,
        CASE WHEN @SortBy = N'NombreCompleto'      AND @SortDir = N'desc' THEN NombreCompleto       END DESC,
        CASE WHEN @SortBy = N'Area'                AND @SortDir = N'asc'  THEN Area                 END ASC,
        CASE WHEN @SortBy = N'Area'                AND @SortDir = N'desc' THEN Area                 END DESC,
        CASE WHEN @SortBy = N'TarifaHora'          AND @SortDir = N'asc'  THEN TarifaHora           END ASC,
        CASE WHEN @SortBy = N'TarifaHora'          AND @SortDir = N'desc' THEN TarifaHora           END DESC,
        CASE WHEN @SortBy = N'ProyectosActivos'    AND @SortDir = N'asc'  THEN ProyectosActivos     END ASC,
        CASE WHEN @SortBy = N'ProyectosActivos'    AND @SortDir = N'desc' THEN ProyectosActivos     END DESC,
        CASE WHEN @SortBy = N'FacturacionEstimada' AND @SortDir = N'asc'  THEN (TarifaHora * @HorasMensuales * ProyectosActivos) END ASC,
        CASE WHEN @SortBy = N'FacturacionEstimada' AND @SortDir = N'desc' THEN (TarifaHora * @HorasMensuales * ProyectosActivos) END DESC,
        Id ASC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- ============================================================================
-- MEJORAS OPCIONALES: ASIGNACION CONSULTOR-PAQUETE Y AUDITORIA
-- ============================================================================

GO
IF OBJECT_ID(N'dbo.sp_ConsultorPaquete_Assign', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_ConsultorPaquete_Assign;
GO
CREATE PROCEDURE dbo.sp_ConsultorPaquete_Assign
    @ConsultorId INT,
    @PaqueteId   INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Regla de negocio: un consultor admite como maximo 5 paquetes.
    IF (SELECT COUNT(1) FROM dbo.ConsultorPaquete WHERE ConsultorId = @ConsultorId) >= 5
        THROW 51001, 'A consultant can have at most 5 paquetes assigned.', 1;

    INSERT INTO dbo.ConsultorPaquete (ConsultorId, PaqueteId)
    VALUES (@ConsultorId, @PaqueteId);

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_ConsultorPaquete_Unassign', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_ConsultorPaquete_Unassign;
GO
CREATE PROCEDURE dbo.sp_ConsultorPaquete_Unassign
    @ConsultorId INT,
    @PaqueteId   INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.ConsultorPaquete
    WHERE ConsultorId = @ConsultorId AND PaqueteId = @PaqueteId;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_ConsultorPaquete_ListByConsultor', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_ConsultorPaquete_ListByConsultor;
GO
CREATE PROCEDURE dbo.sp_ConsultorPaquete_ListByConsultor
    @ConsultorId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.Id,
           p.Nombre,
           p.Descripcion,
           p.Area,
           p.Precio,
           p.Activo,
           cp.FechaAsignacion
    FROM dbo.ConsultorPaquete cp
    INNER JOIN dbo.Paquetes p ON p.Id = cp.PaqueteId
    WHERE cp.ConsultorId = @ConsultorId
    ORDER BY p.Nombre ASC;
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Auditoria_Insert', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Auditoria_Insert;
GO
CREATE PROCEDURE dbo.sp_Auditoria_Insert
    @Usuario   NVARCHAR(150),
    @Accion    NVARCHAR(30),
    @Entidad   NVARCHAR(50),
    @EntidadId INT = NULL,
    @Detalle   NVARCHAR(500) = NULL,
    @Ip        NVARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Auditoria (Usuario, Accion, Entidad, EntidadId, Detalle, Ip)
    VALUES (@Usuario, @Accion, @Entidad, @EntidadId, @Detalle, @Ip);
END
GO

GO
IF OBJECT_ID(N'dbo.sp_Auditoria_List', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Auditoria_List;
GO
CREATE PROCEDURE dbo.sp_Auditoria_List
    @Page      INT           = 1,
    @PageSize  INT           = 10,
    @SortBy    NVARCHAR(50)  = N'FechaHora',
    @SortDir   NVARCHAR(4)   = N'desc',
    @Entidad   NVARCHAR(50)  = NULL,
    @Accion    NVARCHAR(30)  = NULL,
    @Usuario   NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT Id, Usuario, Accion, Entidad, EntidadId, Detalle, Ip, FechaHora,
           COUNT(*) OVER() AS TotalCount
    FROM dbo.Auditoria
    WHERE (@Entidad IS NULL OR Entidad = @Entidad)
      AND (@Accion  IS NULL OR Accion = @Accion)
      AND (@Usuario IS NULL OR Usuario LIKE N'%' + @Usuario + N'%')
    ORDER BY
        CASE WHEN @SortBy = N'FechaHora' AND @SortDir = N'asc'  THEN FechaHora END ASC,
        CASE WHEN @SortBy = N'FechaHora' AND @SortDir = N'desc' THEN FechaHora END DESC,
        CASE WHEN @SortBy = N'Usuario'   AND @SortDir = N'asc'  THEN Usuario   END ASC,
        CASE WHEN @SortBy = N'Usuario'   AND @SortDir = N'desc' THEN Usuario   END DESC,
        CASE WHEN @SortBy = N'Entidad'   AND @SortDir = N'asc'  THEN Entidad   END ASC,
        CASE WHEN @SortBy = N'Entidad'   AND @SortDir = N'desc' THEN Entidad   END DESC,
        CASE WHEN @SortBy = N'Accion'    AND @SortDir = N'asc'  THEN Accion    END ASC,
        CASE WHEN @SortBy = N'Accion'    AND @SortDir = N'desc' THEN Accion    END DESC,
        Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT N'Procedimientos almacenados creados correctamente.';
GO