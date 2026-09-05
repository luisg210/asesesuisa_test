# Consultora — Evaluación Full-Stack (2026-A)

Backend **.NET 8** + Frontend **React.js (Vite + TypeScript)** para la gestión de
paquetes de servicios (`paquetes`), consultores (`consultores`) y usuarios de una
consultora.

---

## 1. Estructura del repositorio

```
.
├─ backend/               Solución .NET 8 (Api, Application, Domain, Infrastructure, Tests)
├─ frontend/              React + Vite + TypeScript (pnpm, Material UI)
├─ database/              Scripts de SQL Server en orden de ejecución
├─ postman/               Colección de Postman
└─ README.md
```

---

## 2. Requisitos previos

| Herramienta | Versión            |
| ----------- | ------------------ |
| .NET SDK    | 8.0+               |
| SQL Server  | 2019+ (Express ok) |
| Node.js     | 20+                |
| pnpm        | 9+                 |

---

## 3. Configuración de la base de datos

Ejecuta los scripts **en este orden exacto** contra una instancia de SQL Server:

```powershell
# con SQL Server Management Studio o sqlcmd:
sqlcmd -S localhost -E -i database\01_Create_Database.sql
sqlcmd -S localhost -E -i database\02_Procedimientos.sql
sqlcmd -S localhost -E -i database\03_Seed_Data.sql
```

| Script                   | Propósito                                                                    |
| ------------------------ | ---------------------------------------------------------------------------- |
| `01_Create_Database.sql` | Crea `ConsultoraDb`, tablas (incl. `RefreshTokens`) e índices únicos         |
| `02_Procedimientos.sql`  | Procedimientos almacenados: login, CRUD, reportes, gestión de refresh-tokens |
| `03_Seed_Data.sql`       | Usuarios de demo, paquetes/consultores de ejemplo                            |

---

## 4. Variables de entorno

### Backend

La cadena de conexión se encuentra en `backend/src/Api/appsettings.Development.json`
(sin secretos reales, solo desarrollo local):

```json
{
  "ConnectionStrings": {
    "ConsultoraDb": "Server=localhost;Database=ConsultoraDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

La configuración de JWT está en `appsettings.json` (`Jwt:Issuer`, `Jwt:Audience`,
`Jwt:SecretKey`, `Jwt:ExpiryMinutes`, `Jwt:RefreshTokenExpiryMinutes`). No se
distribuye ningún secreto: la `Jwt:SecretKey` debe proporcionarse en tiempo de
ejecución (mínimo 32 caracteres) mediante la variable de entorno `Jwt__SecretKey`
o un `appsettings.Local.json` ignorado por git:

```powershell
$env:Jwt__SecretKey = "replace_with_a_random_key_of_at_least_32_chars"
dotnet run --project src/Api --launch-profile http
```

### Frontend

`frontend/.env.example` → copiar a `frontend/.env`:

```
VITE_API_URL=http://localhost:5058/api/v1
```

> Solo se requiere `VITE_API_URL`. No se deben enviar secretos reales al repositorio:
> `.env` está en `.gitignore`; `.env.example` sí se versiona.

---

## 5. Cómo ejecutar

### Backend (desde `backend/`)

```powershell
dotnet run --project src/Api --launch-profile http
# API:   http://localhost:5058
# Swagger: http://localhost:5058/swagger
```

### Frontend (desde `frontend/`)

```powershell
pnpm install
pnpm dev
# UI: http://localhost:5173
```

### Pruebas

```powershell
# backend (xUnit): pruebas unitarias + de integración
dotnet test Consultora.slnx

# frontend (Vitest + Testing Library)
pnpm test
```

> Las **pruebas de integración** del backend levantan un SQL Server real dentro de
> **Docker** (Testcontainers) y ejercitan la API de extremo a extremo mediante
> `WebApplicationFactory`. Si Docker no está disponible se **omiten** (no fallan),
> por lo que el resto de la suite se sigue ejecutando. Total de pruebas: 49 unitarias
>
> - 16 de integración (solicita `dotnet test`).

### Compilaciones / análisis estático

```powershell
dotnet build Consultora.slnx          # backend
pnpm build                            # frontend (tsc + vite)
pnpm lint                             # frontend (oxlint)
```

---

## 6. Credenciales de demo y comportamiento por rol

| Email                   | Rol   | Contraseña  |
| ----------------------- | ----- | ----------- |
| `admin@consultora.test` | Admin | `Admin@123` |
| `user@consultora.test`  | User  | `User@123`  |

- **Admin**: CRUD completo de paquetes/consultores + asignación consultor–paquete +
  reportes + registro de auditoría + tema del visual / herramientas de gráficos.
- **User**: listado/ detalle de solo lectura de paquetes/consultores + reportes.
  Las acciones de escritura están **ocultas en la UI** y **rechazadas por el backend** (HTTP 403).

---

## 7. Convenciones de la API

Ruta base: `/api/v1`.

Envoltorio de respuesta uniforme:

```json
{ "success": true, "message": "", "data": {} }
```

Listados paginados:

```json
{
  "items": [],
  "totalCount": 0,
  "page": 1,
  "pageSize": 10,
  "totalPages": 0
}
```

### Catálogo de áreas

La UI (formularios y filtros de `Área`) usa una lista controlada por el backend para
que los usuarios solo puedan enviar áreas conocidas en lugar de texto arbitrario:

```text
GET /api/v1/areas        # requiere autenticación (Admin o User)
```

Respuesta:

```json
{
  "success": true,
  "message": "",
  "data": [
    "Comercial",
    "Estrategia",
    "Finanzas",
    "Recursos Humanos",
    "Tecnologia"
  ]
}
```

La lista se obtiene de los valores `Area` distintos presentes en las tablas
`Paquetes` y `Consultores` (`SELECT DISTINCT ... UNION`). Es una decisión
documentada: el modelo ER mínimo (ver `Implementacion.md` §5.2) no tiene una tabla
de catálogo, por lo que el catálogo se calcula a partir de los datos existentes en
lugar de añadir una tabla `dbo.Areas`. El cambio es puramente aditivo: los filtros
de área de listado/reportes siguen aceptando cualquier valor, y los formularios
recurren a un campo de texto libre si el endpoint no está disponible.

### Ejemplos de paginación / filtros / orden

```text
GET /api/v1/paquetes?page=2&pageSize=10&sortBy=Precio&sortDir=desc&area=Tecnologia&activo=true
GET /api/v1/consultores?page=1&pageSize=5&sortBy=TarifaHora&sortDir=desc&nombre=ana
GET /api/v1/reportes/paquetes-por-area?sortBy=TotalMonto&sortDir=desc&activo=true
GET /api/v1/reportes/consultores-top-facturacion?sortBy=FacturacionEstimada&sortDir=desc
```

| Parámetro  | Por defecto | Notas                                     |
| ---------- | ----------- | ----------------------------------------- |
| `page`     | 1           | mínimo 1                                  |
| `pageSize` | 10          | acotado entre 1 y 100                     |
| `sortBy`   | `Id`        | lista blanca de columnas por endpoint     |
| `sortDir`  | `asc`       | `asc`/`desc`                              |
| filtros    | —           | `nombre`, `area`, `activo` (por endpoint) |

En los procedimientos almacenados se usa `OFFSET-FETCH` + `COUNT(*) OVER()`, de modo
que `totalCount` refleja el conjunto de resultados **filtrado**.

### Asignación (consultor ↔ paquete)

Las relaciones se exponen bajo el recurso del consultor. Las operaciones de escritura
son **solo para Admin** (403 para User). Tras cada escritura, la respuesta es la lista
de paquetes **actualizada** del consultor.

```text
GET    /api/v1/consultores/{consultorId}/paquetes        # asignados (ambos roles)
POST   /api/v1/consultores/{consultorId}/paquetes        # body: { "paqueteId": 5 } — asignar
DELETE /api/v1/consultores/{consultorId}/paquetes/{paqueteId}  # desasignar
```

Reglas: el consultor y el paquete deben **existir y estar activos**; asignar un
paquete ya asignado → 409; desasignar un paquete no asignado → 404.

### Registro de auditoría (solo Admin)

```text
GET /api/v1/auditoria?page=1&pageSize=10&sortBy=FechaHora&sortDir=desc&entidad=&accion=&usuario=
```

Registra cada escritura con actor + IP: `LOGIN`, `CREATE`, `UPDATE`, `DELETE`,
`ASSIGN`, `UNASSIGN` sobre las entidades `Usuario`, `Paquete`, `Consultor`,
`ConsultorPaquete`. El actor proviene del claim de email del JWT; los fallos de
auditoría se **tragan** para que nunca bloqueen una operación de negocio.

### Columnas ordenables

- Paquetes: `Id`, `Nombre`, `Area`, `Precio`, `FechaCreacion`
- Consultores: `Id`, `NombreCompleto`, `Email`, `Area`, `TarifaHora`, `ProyectosActivos`, `FechaCreacion`
- Reporte paquetes por área: `Area`, `TotalPaquetes`, `TotalMonto`
- Reporte top facturación: `NombreCompleto`, `Area`, `TarifaHora`, `ProyectosActivos`, `FacturacionEstimada`

---

## 8. Reglas de negocio

| Regla                                   | Dónde se aplica                                                             |
| --------------------------------------- | --------------------------------------------------------------------------- |
| TarifaHora ∈ [30, 200]                  | FluentValidation + formulario del frontend                                  |
| Consultor `NombreCompleto`+`Area` único | comprobación en servicio + índice único (409)                               |
| Consultor `Email` único y válido        | comprobación en servicio + índice único (409)                               |
| `ProyectosActivos` ∈ [0, 5]             | FluentValidation + formulario del frontend                                  |
| `Precio` ≥ 0                            | FluentValidation                                                            |
| Máx. 5 paquetes asignados por consultor | comprobación en servicio + `sp_ConsultorPaquete_Assign` + guard del diálogo |
| `{id}` faltante                         | 404                                                                         |
| Sin token / sin permiso                 | 401 / 403                                                                   |
| Borrado                                 | **Lógico**: `Activo = 0`                                                    |

### Decisión de borrado (documentada)

La eliminación es **lógica** (`Activo = 0`) en todos los recursos para que los
registros históricos y los reportes sigan funcionando. Tras un borrado, `GET /{id}`
sigue devolviendo el registro con `activo=false`; las páginas de listado/reportes
exponen un filtro `activo` para ocultarlos.

### Fórmula de top-facturación (documentada)

```
facturacion_estimada = TarifaHora * 160 horas/mes * ProyectosActivos
```

No se añadió una tabla de horas (fuera del alcance); la fórmula usa los proyectos
activos y está documentada en la vista del reporte y en `02_Procedimientos.sql`.

---

## 9. Decisiones técnicas

- **Arquitectura en capas**: Api → Application (servicios/DTOs/validadores) →
  Infrastructure (repositorios ADO.NET) → Domain. Los puertos (interfaces de
  repositorio) viven en `Application`; las implementaciones con BD en `Infrastructure`.
- **Sin ORM**: `Microsoft.Data.SqlClient` invocando procedimientos almacenados.
- **DI + `ILogger` + middleware global de excepciones** que mapea a respuestas consistentes.
- **Autenticación JWT** (claim de rol) + políticas `[Authorize(Roles=...)]`.
- **Flujo de refresh-token**: tokens opacos de larga duración (Base64Url de 64 bytes),
  almacenados como **hash SHA-256** (nunca en texto plano), rotados en cada
  `/auth/refresh`, revocados de golpe en `/auth/logout`. La clave de firma del JWT se
  configura de la misma manera para generación y validación (`KeyId` compartido,
  requerido por los parches de Microsoft.IdentityModel), evitando discrepancias de firma.
- **FluentValidation** para la validación de peticiones; los errores se aplanan a
  `{ propertyName, errorMessage }` en HTTP 400.
- **BCrypt** (coste 12) para el hash de contraseñas.
- **Swagger/OpenAPI** con esquema de seguridad bearer.
- **Frontend**: Material UI, cliente axios centralizado `api.ts` que inyecta el token
  y redirige a `/login` en 401, `AuthContext`, `PrivateRoute`, UI basada en roles.
- **Catálogo de áreas**: `GET /api/v1/areas` devuelve los valores `Area` distintos de
  `Paquetes`/`Consultores` mediante una consulta directa de solo lectura (sin
  procedimiento almacenado, por lo que no hace falta volver a ejecutar scripts de BD).
  Los formularios renderizan un selector `Area` a partir de esta lista; los filtros
  usan la misma lista; si la petición falla, la UI recurre a texto libre.
- **Asignación + auditoría (Fase 3)**: implementadas con el patrón de capas existente
  (puertos en `Application`, repos ADO.NET en `Infrastructure`, controladores finos en
  `Api`). El contexto de auditoría (actor = email del JWT, IP del cliente) se captura
  en un `BaseApiController`; los métodos de escritura del servicio aceptan `actor`/`ip`
  opcionales para que la auditoría no se filtre en la lógica de dominio.
- **Gráficos (Fase 3)**: `recharts@3` en `ReportesPage`; el gráfico obtiene una página
  amplia (hasta 100 filas) con los mismos filtros que la tabla, que se mantiene paginada.
- **Tema visual (Fase 3)**: `AppThemeProvider` (MUI `ThemeProvider` + preferencia de
  claro/oscuro persistida en `localStorage`) envuelve la app; `Layout` tiene el switch.
- **Logging**: **Serilog** reemplaza al proveedor de consola por defecto. Los sinks
  (consola + archivo diario rotatorio `logs/consultora-.log`, 14 archivos retenidos) se
  declaran en la sección `Serilog` de `appsettings.json`, por lo que no hace falta
  cambiar código para ajustar niveles o destinos. Las inyecciones existentes de
  `ILogger<T>` siguen funcionando tal cual.

---

## 10. Pruebas

**Backend (65 pruebas: 49 unitarias + 16 de integración, xUnit)**

- Validadores de reglas de negocio (rango de tarifa, email, proyectos 0–5).
- Pruebas de servicios con repositorios en memoria (unicidad → 409, faltante → 404, borrado lógico).
- Servicio de asignación: asignar/desasignar, duplicados → 409, desasignar no asignado → 404,
  consultor/paquete inactivo rechazado, refresco de la lista expuesta.
- Servicio de auth: credenciales incorrectas → 401, login válido → token/rol, rotación de refresh,
  reutilización de refresh → 401, revocación en logout.
- Servicio de catálogo de áreas.
- Mapeo del middleware de excepciones (400/401/404/409/500).
- Claims del generador de JWT.
- **Integración (Testcontainers + WebApplicationFactory, Docker)** — SQL Server real en un
  contenedor inicializado con los scripts del repositorio, rondas HTTP completas:
  login/refresh/logout de auth (rotación, reutilización, token inválido), flujo CRUD de
  paquetes, validación y unicidad 400/404/409, User → escritura → 403, reportes, áreas, auditoría.

**Frontend (13 pruebas, Vitest + Testing Library)**

1. Validación del formulario de login (obligatorio/email inválido/envío válido).
2. Renderizado del listado de paquetes + estado vacío + el filtrado dispara la llamada API + carga del catálogo de áreas.
3. Visibilidad de acciones de escritura según rol (Admin vs User).
4. Feedback con toast en casos de éxito/error de creación y borrado.

Guía de prueba manual: usa la colección de Postman (`postman/`) para un recorrido
completo, incluidos los casos negativos (User → escritura → 403).

---

## 11. Postman

Importa `postman/Consultora_API.postman_collection.json`. Variables de la colección:

| Variable                    | Por defecto                                                        |
| --------------------------- | ------------------------------------------------------------------ |
| `baseUrl`                   | `http://localhost:5058/api/v1`                                     |
| `adminToken`                | (se auto-configura tras "Login Admin")                             |
| `userToken`                 | (se auto-configura tras "Login User")                              |
| `refreshToken`              | (se auto-configura tras "Login Admin"; rotado por "Refresh token") |
| `paqueteId` / `consultorId` | ids para probar update/delete                                      |

La carpeta **Auth** incluye casos negativos (credenciales inválidas → 401, refresh
inválido → 401) además de un **Refresh token** funcional (rota y vuelve a guardar las
variables) y **Logout** (limpia el refresh token). La carpeta **Casos de error /
autorizacion** incluye más casos negativos (401, 403, validación 400, unicidad 409).

---

## 12. Supuestos, limitaciones y pendientes

### Supuestos

- Los datos y credenciales de demo son **solo para entornos de prueba** (no hay
  empresas ni personas reales; los emails de seed usan `@consultora.test` /
  `@correo.test`).
- SQL Server local con autenticación de Windows; ajusta la cadena de conexión a tu instancia.

### Limitaciones

- Refrescar y hacer logout están soportados **desde backend + frontend**; la colección
  de Postman y las pruebas de integración los cubren de extremo a extremo.
- Los endpoints de listado devuelven por defecto tanto activos como inactivos (usa el
  filtro `activo`).

### Pendiente / opcional

- **Implementado en la Fase 3**: registro de auditoría, gráficos, tema visual, despliegue
  con Docker, asignación consultor–paquete.
- **Implementado como follow-up**: refresh tokens y una suite de pruebas de integración
  verificada (se ejecuta contra un contenedor real de SQL Server cuando hay Docker).
- **Aún pendiente**: CI/CD, tabla de horas (mejoraría la fórmula de facturación).

---

## 13. Despliegue con Docker (Fase 3)

El repositorio incluye un stack de 3 servicios: SQL Server, la API .NET 8 y el
frontend React servido por nginx (que también hace de proxy de `/api` hacia el
contenedor de la API, de modo que la SPA corre **mismo origen** en
`http://localhost:8080`).

### Requisitos

- Docker Engine con Compose v2 (`docker compose version`).
- Un archivo `.env` **en la raíz del repositorio** (en `.gitignore`); usa como
  plantilla el `.env.example` versionado, que ya incluye ambas variables:

```env
MSSQL_SA_PASSWORD=YourStrong!Passw0rd
JWT_SECRET_KEY=replace_with_a_random_key_of_at_least_32_chars
```

### Inicio

```powershell
docker compose up -d --build
docker compose ps
```

- Web (SPA): http://localhost:8080
- Swagger (API): http://localhost:8080/api/swagger — _ver nota más abajo_

### Inicializar la base de datos (una sola vez)

La imagen de mssql **no** ejecuta scripts SQL automáticamente. Una vez que `sqlserver`
esté healthy, ejecuta los scripts **en orden** desde dentro del contenedor (o con
sqlcmd desde el host apuntando a `localhost,1433`):

```powershell
docker compose exec sqlserver bash -c \
  '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /database/01_Create_Database.sql'
# repite para 02_Procedimientos.sql y 03_Seed_Data.sql
# ¿base de datos existente aún sin actualizar?
#   vuelve a ejecutar 01, 02 y 03 (son idempotentes: recrean tablas/procs y re-aplican el seed)
```

### Notas

- La API requiere `Jwt__SecretKey` (mín. 32 caracteres) al arrancar; se inyecta desde
  `JWT_SECRET_KEY`. Igual que la contraseña de la BD mediante `MSSQL_SA_PASSWORD`.
- Swagger solo se habilita con `ASPNETCORE_ENVIRONMENT=Development`. Establece
  `ASPNETCORE_ENVIRONMENT=Development` en el servicio `api` de `docker-compose.yml`
  para exponer Swagger en `/api/swagger` (comparte el proxy `/api/` de nginx).
- La carpeta `/database` se monta dentro del contenedor de mssql en `/database` para que
  los comandos de inicialización anteriores puedan referenciarla.
- Reconstruye con `docker compose up -d --build` tras cambios de código; los datos
  persisten en el volumen `sqlserver-data`. Los logs del backend
  (`logs/consultora-*.log`) persisten en el volumen `api-logs` y también salen por
  stdout (`docker compose logs api`).
