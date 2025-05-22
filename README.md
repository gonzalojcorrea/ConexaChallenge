# Conexa MovieApi Challenge

**Proyecto**: Backend en .NET 8 (Clean Architecture) con PostgreSQL para gestión de películas y series.

## 📋 Descripción

Esta API RESTful, construida en .NET 8 con Clean Architecture, ofrece:

* **Autenticación y autorización** con JWT (usando `IPasswordHasher` para hashear contraseñas antes de persistir).
* **Roles**: `Master` → administrador y `Padawan` → usuario regular.
* **CRUD** de la entidad `Movie`, con campos `createdAt` y `deletedAt` para *soft delete*.
* **Sincronización** de películas desde la API pública SWAPI, incluyendo nombres de personajes.
* **Persistencia** en PostgreSQL (JSONB para lista de personajes).
* **Resiliencia** con Polly: Retry, Circuit Breaker y Timeout.
* **Documentación** automática vía Swagger / OpenAPI.
* **Principios SOLID**: la arquitectura y la estructura de comandos, queries y handlers están alineadas con los principios SOLID.

## 📂 Estructura de carpetas

```bash
/ API
  ├─ Controllers           ← Endpoints y filtros (AuthController, MoviesController)
  ├─ Program.cs            ← Configuración de servicios y middleware
  ├─ Properties
  └─ appsettings*.json     ← Configuraciones de entorno

/ Application
  ├─ Common               ← DTOs, Interfaces genéricos (IUnitOfWork, IJwtService)
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
  └─ Seed                 ← Datos iniciales (usuarios, roles, ejemplos de películas)

/ Tests
  └─ API.Test             ← Proyectos de pruebas unitarias e integración basado en MediatR Commands

ConexaChallenge.sln

docker-compose.yml       ← Levanta API + PostgreSQL con migraciones y seed automáticos
```

## 🚀 Tecnologías

* .NET 8 / C# 12
* MediatR
* FluentValidation
* Entity Framework Core + Npgsql
* PostgreSQL 12+ (JSONB)
* Polly (Retry, Circuit Breaker, Timeout)
* JWT Bearer
* Swashbuckle (Swagger)
* xUnit

## 🧪 Testing

Los tests unitarios se realizan de forma granular usando MediatR Commands:

* **Command Handlers**: se prueban individualmente cada handler, validaciones y lógica de negocio.
* **Validators**: se asegura que FluentValidation rechace entradas inválidas.
* **Repository Mocks**: se usan repositorios simulados para aislar la lógica de aplicación.

Para ejecutar todos los tests:

```bash
cd Tests/API.Test
dotnet test --no-restore
```

## ⚙️ Requisitos

* Docker & Docker Compose

## 🏁 Ejecución con Docker

La forma recomendada de levantar todo es con Docker Compose. Desde la raíz del proyecto:

```bash
docker-compose up --build
```

* Levanta PostgreSQL y la API.
* Aplica migraciones automáticamente.
* Inserta datos *seed* (usuarios, roles, ejemplos de películas).

> Verifica en Docker que existan los contenedores **api-1** y **db-1** ejecutándose.

## 📑 Documentación Swagger

🎯 [Link Swagger UI](http://localhost:8080/swagger/index.html)

Si la interfaz no carga correctamente, haz un **hard refresh** con **Ctrl + F5**.

## 📬 Postman – Colección de Prueba

🎯 [Conexa - MovieApi Challenge](https://www.postman.com/mission-astronomer-45032345/workspace/conexa-starwarsapi-challenge/collection/38312395-ef8eed31-fd14-4b25-ab0f-fc673d6aa32c?action=share&creator=38312395)

* Al autenticarte, un script guarda el token en la variable `tokenChallenge`.
* En **Get All Movies**, otro script extrae `firstMovieId`.

> Si no puedes acceder al link anterior, importa la colección desde la carpeta `postman/` en la raíz del proyecto.

## 👥 Usuarios de prueba

| Username            | Rol     | Contraseña              |
| ----------          | ------- | ----------------------- |
| anakin@jedi.com     | Padawan | youunderestimatemypower |
| obiwan@jedi.com     | Master  | ihavethehighground      |

## 📌 Endpoints principales

| Ruta                 | Método | Descripción                                   | Roles           |
| -------------------- | ------ | --------------------------------------------- | --------------- |
| `/api/auth/register` | POST   | Registra un usuario y devuelve JWT            | Anónimo         |
| `/api/auth/login`    | POST   | Autentica y devuelve JWT                      | Anónimo         |
| `/api/movies`        | GET    | Lista todas las películas                     | Master, Padawan |
| `/api/movies/{id}`   | GET    | Detalle de una película                       | Padawan |
| `/api/movies`        | POST   | Crea una nueva película                       | Master          |
| `/api/movies/{id}`   | PUT    | Actualiza una película existente              | Master          |
| `/api/movies/{id}`   | DELETE | *Soft delete* de película (marca `deletedAt`) | Master          |
| `/api/movies/sync`   | POST   | Sincroniza películas desde SWAPI              | Master          |

## 🛡️ Resiliencia SWAPI

Configurado con Polly para:

* **Retry** lineal: 3 intentos, +5 s cada uno.
* **Circuit Breaker**: abre tras 3 fallos y dura 60 s.
* **Timeout**: límite de 20 s por petición externa.

## 🗂️ Migraciones manuales

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

## 💡 Próximas mejoras

* Paginación y filtrado en listados.
* Caché de SWAPI para optimizar llamadas.
* Despliegue automatizado en la nube.

---

**¡May The Force Be Whit You!**