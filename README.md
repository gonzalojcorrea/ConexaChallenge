# Conexa Challenge

**Proyecto**: Backend en .NET 8 (Clean Architecture) con PostgreSQL para gestión de películas y series.

## 📋 Descripción

Este repositorio contiene una API RESTful desarrollada en .NET 8 siguiendo la **Clean Architecture**. Permite:

* Registro y autenticación de usuarios con **JWT**.
* Roles: **Master** y **Padawan**.
* CRUD completo de entidades `Movie`.
* Sincronización de películas de Star Wars desde la API pública (\[swapi.tech]) con manejo de personajes.
* Persistencia en **PostgreSQL** (uso de `JSONB` para listas de personajes).
* Políticas de resiliencia con **Polly** (Retry, Circuit Breaker, Timeout).
* Documentación automática con **Swagger / OpenAPI**.

## 📂 Estructura de carpetas

```
/API             ← Proyecto WebApi (.NET 8)
/Application     ← Lógica de negocio, casos de uso (MediatR, Validators)
/Domain          ← Entidades y modelos de dominio puros
/Infrastructure  ← EF Core, repositorios, cliente SWAPI, middleware, configuración
ConexaChallenge.sln
docker-compose.yml
```

## 🚀 Tecnologías

* .NET 8, C#
* MediatR
* FluentValidation
* Entity Framework Core (Npgsql)
* PostgreSQL (JSONB)
* Polly (Retry, Circuit Breaker, Timeout)
* Microsoft.AspNetCore.Authentication.JwtBearer
* Swashbuckle (Swagger / OpenAPI)

## ⚙️ Requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* PostgreSQL 12+
* Docker & Docker Compose (opcional)

## 📝 Configuración

1. Clona el repositorio:

   ```bash
   git clone https://github.com/gonzalojcorrea/ConexaChallenge.git
   cd ConexaChallenge
   ```
2. Levanta PostgreSQL + API.NET con Docker Compose:

   ```bash
   docker-compose up --build
   ```

## 🔧 Migraciones & Seed

Una vez que el proyecto se levanta las migraciones corren de manera automática, en caso de que no las vea aplicadas en la base de datos puede ejecutar el siguiente comando:

```
dotnet ef database update --project Infrastructure --startup-project API
```

## ▶️ Ejecutar la API

```bash
dotnet run --project API
```

Se expondrá en `https://localhost:5001` por defecto.

## 📑 Documentación Swagger

Accede a la UI en:

```
https://localhost:5001/swagger/index.html
```

Allí encontrarás todos los endpoints con ejemplos y modelos.

## 📌 Endpoints principales

| Ruta                 | Método | Descripción                   | Roles           |
| -------------------- | ------ | ----------------------------- | --------------- |
| `/api/auth/register` | POST   | Registrar nuevo usuario (JWT) | *anónimo*       |
| `/api/auth/login`    | POST   | Autenticar usuario (JWT)      | *anónimo*       |
| `/api/movies`        | GET    | Listar películas              | Master, Padawan |
| `/api/movies/{id}`   | GET    | Detalle de película           | Master, Padawan |
| `/api/movies`        | POST   | Crear nueva película          | Master          |
| `/api/movies/{id}`   | PUT    | Actualizar película existente | Master          |
| `/api/movies/{id}`   | DELETE | Eliminar película existente   | Master          |
| `/api/movies/sync`   | POST   | Sincronizar desde SWAPI       | Master          |

## 🛡️ Resiliencia SWAPI

El cliente SWAPI está configurado con **Polly** para:

* **Retry** lineal (3 intentos, +5s cada vez).
* **Circuit Breaker** (3 fallos → circuito abierto 60s).
* **Timeout** (20s por petición).

## 💡 Buenas prácticas

* Uso de **Clean Architecture** para separar capas.
* Validaciones centralizadas con **FluentValidation**.
* Manejo global de errores con middleware (`ProblemDetails`).
* Seguridad mediante **JWT** y roles tipados.

## 🚀 Próximos pasos / Mejora

* Agregar pruebas unitarias y de integración.
* Paginación en listados.
* Caché de llamadas SWAPI para optimizar.
* Despliegue en la nube (Azure, AWS, Heroku).

---

**¡Que la Fuerza del código limpio te acompañe!**
