# Conexa Challenge

**Proyecto**: Backend en .NET 8 (Clean Architecture) con PostgreSQL para gestión de películas y series.

## 📋 Descripción

Esta API RESTful, construida en .NET 8 con Clean Architecture, ofrece:

* **Autenticación y autorización** con JWT.
* **Roles**: `Master` → administrador y `Padawan` → usuario regular.
* **CRUD** de la entidad `Movie`, con campos `createdAt` y `deletedAt` para *soft delete*.
* **Sincronización** de películas desde la API pública SWAPI, incluyendo nombres de personajes.
* **Persistencia** en PostgreSQL (JSONB para lista de personajes).
* **Resiliencia** con Polly: Retry, Circuit Breaker y Timeout.
* **Documentación** automática via Swagger / OpenAPI.

## 📂 Estructura de carpetas

```bash
/ API
  ├─ Controllers           ← Endpoints y filtros (AuthController, MoviesController)
  ├─ Program.cs            ← Configuración de servicios y middleware
  ├─ Properties
  └─ appsettings*.json     ← Configuraciones de entorno

/ Application
  ├─ Common               ← Dtos, Models, Interfaces genéricos (IUnitOfWork, IJwtService)
  ├─ Features
  │   ├─ Auth             ← Commands, Queries, Handlers, Validators para autenticación
  │   └─ Movies           ← Commands, Queries, Handlers, Validators para películas
  └─ Behaviors            ← Pipelines MediatR (Validation, Logging)

/ Domain
  ├─ Entities             ← Clases de dominio (Movie, User, BaseEntity)
  ├─ Interfaces           ← Interfaces de dominio (repositorios, servicios)
  └─ Constants            ← Definición de Roles, mensajes globales

/ Infrastructure
  ├─ Persistence
  │   ├─ Configurations   ← DbContext, EF Core mappings, QueryFilters
  │   ├─ Repositories      ← Implementaciones de repos para cada entidad
  │   └─ Migrations        ← Migraciones EF Core
  ├─ Services
  │   └─ SwapiClient      ← Cliente HTTP y resiliencia con Polly
  ├─ Configurations       ← Middleware global, Filtros, Extensiones DI
  └─ Seed                 ← Datos iniciales (usuarios, roles)

ConexaChallenge.sln

docker-compose.yml       ← Levanta API + PostgreSQL con migraciones y seed automáticos
```

## 🚀 Tecnologías

* .NET 8 / C# 12
* MediatR
* FluentValidation
* Entity Framework Core + Npgsql
* PostgreSQL 12+ (JSONB)
* Polly (Retry, Circuit Breaker, Timeout)
* JWT Bearer
* Swashbuckle (Swagger)

## ⚙️ Requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* Docker & Docker Compose

## 🏁 Ejecución con Docker

La forma recomendada de levantar todo es con Docker Compose. Desde la raíz del proyecto:

```bash
docker-compose up --build
```

* Levanta PostgreSQL y la API.
* Aplica migraciones automáticamente.
* Inserta datos *seed* (usuarios, roles, ejemplos de películas).

> Una vez que el proyecto esté levantado correctamente, verifica en Docker que exista un contenedor llamado **conexachallenge** con los servicios **api-1** y **db-1** ejecutándose. Si ambos están activos, puedes continuar a la sección de documentación.

## 📑 Documentación Swagger

🔗 `http://localhost:8080/swagger/index.html`

Si la interfaz no carga correctamente, haz un **hard refresh** con **Ctrl + F5** para vaciar la caché del navegador.

## 📬 Postman – Colección de Prueba

🎯 [Conexa StarWarsAPI Challenge](https://www.postman.com/mission-astronomer-45032345/workspace/conexa-starwarsapi-challenge/collection/38312395-ef8eed31-fd14-4b25-ab0f-fc673d6aa32c?action=share&creator=38312395)

* Al autenticarte con los endpoints **Login**, un script guarda el token en la variable `tokenChallenge`.
* En **Get All Movies**, otro script extrae `firstMovieId` para usar en los endpoints que requieren un ID.

## 👥 Usuarios de prueba

| Username   | Rol     | Contraseña              |
| ---------- | ------- | ----------------------- |
| anakin     | Padawan | youunderestimatemypower |
| obiwan     | Master  | ihavethehighground      |
| yoda       | Master  | sizemattersnot          |
| ahsokatano | Padawan | chosenonePadawan        |

## 📌 Endpoints principales

| Ruta                 | Método | Descripción                                   | Roles           |
| -------------------- | ------ | --------------------------------------------- | --------------- |
| `/api/auth/register` | POST   | Registra un usuario y devuelve JWT            | Anónimo         |
| `/api/auth/login`    | POST   | Autentica y devuelve JWT                      | Anónimo         |
| `/api/movies`        | GET    | Lista todas las películas                     | Master, Padawan |
| `/api/movies/{id}`   | GET    | Detalle de una película                       | Master, Padawan |
| `/api/movies`        | POST   | Crea una nueva película                       | Master          |
| `/api/movies/{id}`   | PUT    | Actualiza una película existente              | Master          |
| `/api/movies/{id}`   | DELETE | *Soft delete* de película (marca `deletedAt`) | Master          |
| `/api/movies/sync`   | POST   | Sincroniza películas desde SWAPI              | Master          |

## 🛡️ Resiliencia SWAPI

Configurado con Polly para:

* **Retry** lineal: 3 intentos, +5 s cada uno.
* **Circuit Breaker**: abre tras 3 fallos consecutivos y se mantiene 60 s.
* **Timeout**: cada petición externa tiene un límite de 20 s.

## 🗂️ Migraciones manuales

En caso de necesitar ejecutar migraciones manualmente:

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

## 💡 Próximas mejoras

* Pruebas unitarias e integración.
* Paginación y filtrado en listados.
* Caché de SWAPI para optimizar llamadas.
* Despliegue automatizado en la nube.

---

**¡Que la Fuerza del código limpio te acompañe!**
