FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM node:14.20.0 as node-build
WORKDIR /web/src
COPY ACSAppBox/ClientApp/package.json .
COPY ACSAppBox/ClientApp/package-lock.json .
RUN npm install
COPY ACSAppBox/ClientApp .
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore

COPY --from=node-build /web/src/build /src/ACSAppBox/wwwroot/
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ACSAppBox.dll"]
