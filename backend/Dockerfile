FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/ ./src/
RUN dotnet restore ./src/Api/Api.csproj
RUN dotnet publish ./src/Api/Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish
RUN rm /app/publish/appsettings.Development.json

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish ./

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]
