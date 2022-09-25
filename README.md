# ACS app in a box

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fstmikhailfhl1.blob.core.windows.net%2Ffiles%2Fazuredeploy.json)

## Using pre-built image

We've published a pre-built Docker image that you can pull and run locally
quickly.

Pull:

```shell
docker pull crmikhailfhl.azurecr.io/acs-app-box
```

Run:

```shell
docker run `
-p 8080:80 `
-e ConnectionStrings__ACSConnectionString="<YOUR_ACS_CONNECTION_STRING>" `
-v C:\docker-volumes\acs-app-box:/home/ `
crmikhailfhl.azurecr.io/acs-app-box
```

* `-p 8080:80` maps your local <http://localhost:8080> to the port 80 that
the app is running on inside the container,

* `-e ConnectionStrings__ACSConnectionString="<YOUR_ACS_CONNECTION_STRING>"`
passes the ACS Connection String as an environment variable to the container,

* `-v C:\docker-volumes\acs-app-box:/home/` maps the `/home/` directory
inside the container to `C:\docker-volumes\acs-app-box` on your host.
The container is using an SQLite database (file named `ACSAppBox.db`) that
is stored in `/home/` in the container. Map this to something on your host
so that the data survives container restarts.

Access the image by opening <http://localhost:8080> and logging in with one
of the predefined user accounts.

## Local environment setup

Pre-requesites:

* .net 6 SDK

* `dotnet ef` tool. Install by doing `dotnet tool install --global dotnet-ef`
[Link for details](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

* (optional) [Docker Desktop](https://www.docker.com/products/docker-desktop/)

Local setup:

1. In the `src` directory run: `dotnet restore`

1. Add your ACS connection string secret:

   `dotnet user-secrets set "ConnectionStrings:ACSConnectionString" "endpoint=...;accesskey=..."`

1. Run `dotnet run --project ACSAppBox`

1. Navigate to [https://localhost:7094/](https://localhost:7094/) - this will
start the React dev server and redirect you to [https://localhost:44431/](https://localhost:44431/)
which hosts the website.

1. Optional.

## Local container build

We have some WIP functionality to build and run the app in a container locally.

1. To build an image, in the `src` directory run:

   ```shell
   docker build -t acs-app-box .
   ```

1. To run the image: use the same command as in the
[Using pre-built image](#using-pre-built-image) section, except the container
name should be just `acs-app-box` (instead of `crmikhailfhl.azurecr.io/acs-app-box`).

1. Test the image by opening <http://localhost:8080> and logging in with one
   of the predefined user accounts.

## Seed data

Whenever the app starts for the first time, it will create/migrade the
database (located in the `ACSAppBox.db` SQLite file) with dummy data.

User accounts (all with `Pass123$` as password):

* `gandalf@example.com`

* `frodo@example.com`

## Sql Server Db

By default the app uses an SQLite database, but it is possible to use SQL
Server instead:

* Add a ConnectionString key called `SqlServerConnectionString`

* Add a top-level config key `Provider` and set it to "`SqlServer`"

  * Alternatively, run the app passing `Provider` value via command line:

    ```shell
    dotnet run --project ACSAppBox -- --provider SqlServer
    ```