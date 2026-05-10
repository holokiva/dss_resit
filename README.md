# movie watchlist

## quick start (local)

- backend:
  - `cd backend`
  - `dotnet run`
  - api: `http://localhost:4058`
  - swagger: `http://localhost:4058/swagger`
- frontend:
  - `cd movie-watchlist-client`
  - `npm install`
  - `npm run dev`

## quick start (docker)

- `docker compose up --build`
- api: `http://localhost:4058`
- swagger: `http://localhost:4058/swagger`

## database

### development mode

By default (`backend/appsettings.Development.json`) the api uses an in-memory database for easy local startup.

To use PostgreSQL locally, set:

- `UseInMemoryDatabase=false`
- `ConnectionStrings:DefaultConnection` to your postgres connection string

### migrations

This project can use EF Core migrations.

- install tools (once):
  - `dotnet tool install --global dotnet-ef`
- create a migration:
  - `cd backend`
  - `dotnet ef migrations add initial_create`
- apply migrations:
  - `dotnet ef database update`

## auth

Auth uses JWT bearer tokens.

- `POST /api/auth/register`
- `POST /api/auth/login`
- use the returned `token` as: `Authorization: Bearer <token>`

## cors

CORS allows local frontend origins by default:

- `http://localhost:5173`
- `http://localhost:4173`

Configure via `Cors:Origins` (or `Cors__Origins__*` environment variables in docker).
