# Weather In The City

Project aims to create a simple CRUD app with several integrations with external Services.
It's MVP created in Blazor without taking huge load and scalability into consideration.

## Functional requirements:

- Store at least one type of entity
- Data must be stored in a database - Any type of database (e.g. SQL, MongoDB), excluding file systems, operating memory and browser store.
- Add a new entity.
- Edit an entity.
- Delete an entity.
- List all entities.
- At least one entity should have a unique identifier
- Use the following property types in your data models:
    - Date
    - String
    - Boolean
    - Number
- Fetch data dynamically from an external source – for example weather forecast data, currency exchange rate. The fetched data should be processed and stored
  appropriately for further use within the application.
- Sorting
- Search
- Pagination

## Developer Guide:

Because app uses several external services, it might be hard to run it locally.

### Local Development

For local development it might be beneficial to deploy [Postgresql](https://www.postgresql.org.pl/) and [Seq](https://datalust.co/seq) via Docker.
Blazor app can run locally without Docker.

- Create `appsettings.local.json` in [`src/WeatherInTheCity`](src/WeatherInTheCity) based on following template. Add your credentials.

  ```json
  {
    "HygraphOptions": {
      "Endpoint": "",
      "Pat": ""
    },
    "SendGridOptions": {
      "Key": "",
      "EmailFrom": ""
    },
    "OpenAiOptions": {
      "ApiKey": ""
    }
  }
  ```

```bash
docker compose -f docker compose -f compose/compose-services-dev.yaml up -d seq postgres
```
```bash
dotnet run --project src/WeatherInTheCity/WeatherInTheCity.csproj
```

### Docker Setup

If you simply want to run the app and all needed services in Docker, you can use the following command.

- When running app in Docker, instead af `appsetting.local.json` create `.local.env` file  in [`src/WeatherInTheCity`](src/WeatherInTheCity) based on following template. Add your credentials.

  ```dotenv
  CITIESDBOPTIONS__CONNECTIONSTRING=Host=postgres;Port=5432;Database=cities;Username=postgres;Password=postgrespw;
  SEQOPTIONS__HOST=http://seq:80
  HYGRAPHOPTIONS__ENDPOINT=
  HYGRAPHOPTIONS__PAT=
  SENDGRIDOPTIONS__KEY=
  SENDGRIDOPTIONS__EMAILFROM=
  OPENAIOPTIONS__APIKEY=
  ```

```bash
docker compose -f docker compose -f compose/compose-services.yaml up -d
```

### Deployment

App can be easily deployed via [Portainer](https://www.portainer.io/) as well. Use [Business Edition](https://www.portainer.io/install) or [portainer-ee](https://hub.docker.com/r/portainer/portainer-ee) image.

Example [.env](.env/.example_prod.env) file for production deployment.

Easiest way to serve app is to use [Caddy](https://caddyserver.com/) as web server.