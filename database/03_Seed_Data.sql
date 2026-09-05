USE ConsultoraDb;

GO
-- ----------------------------------------------------------------------------
-- Usuarios demo (password hasheados con BCrypt, cost 12).
--   admin@consultora.test / Admin@123  (Rol: Admin)
--   user@consultora.test  / User@123   (Rol: User)
-- ----------------------------------------------------------------------------
INSERT INTO
    dbo.Usuarios (Email, PasswordHash, Rol)
VALUES
    (
        N'admin@consultora.test',
        N'$2b$12$uDt22GtrUYrts4yhcc5ZAudMpNrrc2H8o0xWlXZBRxz0aUxRUMdOq',
        N'Admin'
    ),
    (
        N'user@consultora.test',
        N'$2b$12$N.Lt2qzVRS2kqa8sf1BTWeVL9m6TVJUUoffFKGc3XKRaXUWJ.j.t.',
        N'User'
    );

GO
-- ----------------------------------------------------------------------------
-- Paquetes de servicio de ejemplo.
-- ----------------------------------------------------------------------------
INSERT INTO
    dbo.Paquetes (Nombre, Descripcion, Area, Precio, Activo)
VALUES
    (
        N'Diagnostico Estrategico',
        N'Evaluacion de la posicion competitiva y definicion de prioridades.',
        N'Estrategia',
        3500.00,
        1
    ),
    (
        N'Plan de Transformacion Digital',
        N'Hoja de ruta integral para digitalizar procesos de negocio.',
        N'Tecnologia',
        8500.00,
        1
    ),
    (
        N'Auditoria de Seguridad Informatica',
        N'Revision de vulnerabilidades y recomendaciones de remediacion.',
        N'Tecnologia',
        4200.00,
        1
    ),
    (
        N'Optimizacion de Costos',
        N'Analisis de estructura de costos e identificacion de ahorros.',
        N'Finanzas',
        2800.00,
        1
    ),
    (
        N'Modelo de Gestion por Competencias',
        N'Diseno de perfiles, evaluacion 360 y planes de desarrollo.',
        N'Recursos Humanos',
        5100.00,
        1
    ),
    (
        N'Implementacion de CRM',
        N'Seleccion, configuracion y adopcion de una plataforma CRM.',
        N'Tecnologia',
        9800.00,
        1
    ),
    (
        N'Estudio de Mercado',
        N'Investigacion de mercado y validacion de propuesta de valor.',
        N'Comercial',
        3900.00,
        1
    ),
    (
        N'Gobierno de Datos',
        N'Marco de gobierno de datos y calidad de informacion.',
        N'Tecnologia',
        6100.00,
        0
    );

GO
-- ----------------------------------------------------------------------------
-- Consultores de ejemplo (nombres ficticios).
-- ----------------------------------------------------------------------------
INSERT INTO
    dbo.Consultores (
        NombreCompleto,
        Email,
        Area,
        TarifaHora,
        Activo,
        ProyectosActivos
    )
VALUES
    (
        N'Ana Martinez Ponce',
        N'ana.martinez@correo.test',
        N'Estrategia',
        95.00,
        1,
        3
    ),
    (
        N'Luis Fernando Rojas',
        N'luis.rojas@correo.test',
        N'Tecnologia',
        120.00,
        1,
        4
    ),
    (
        N'Maria Jose Vasquez',
        N'maria.vasquez@correo.test',
        N'Finanzas',
        80.00,
        1,
        2
    ),
    (
        N'Carlos Andres Diaz',
        N'carlos.diaz@correo.test',
        N'Recursos Humanos',
        70.00,
        1,
        2
    ),
    (
        N'Paula Andrea Lopez',
        N'paula.lopez@correo.test',
        N'Comercial',
        75.00,
        1,
        1
    ),
    (
        N'Jorge Eduardo Salas',
        N'jorge.salas@correo.test',
        N'Tecnologia',
        140.00,
        1,
        5
    ),
    (
        N'Rocio del Pilar Gil',
        N'rocio.gil@correo.test',
        N'Finanzas',
        90.00,
        0,
        0
    );

GO

PRINT N'Datos de demostracion insertados correctamente.';
GO

GO