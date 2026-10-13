FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY TodoApp/TodoApp.csproj TodoApp/
RUN dotnet restore TodoApp/TodoApp.csproj
COPY TodoApp/ TodoApp/
RUN dotnet publish TodoApp/TodoApp.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
# time zone database: needed for per-player time zones (e.g. Europe/Berlin)
RUN apt-get update && apt-get install -y --no-install-recommends tzdata && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoApp.dll"]
