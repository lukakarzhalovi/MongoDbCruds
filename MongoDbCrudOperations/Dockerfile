FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MongoDbCrudOperations/MongoDbCrudOperations.csproj", "MongoDbCrudOperations/"]
RUN dotnet restore "MongoDbCrudOperations/MongoDbCrudOperations.csproj"
COPY . .
WORKDIR "/src/MongoDbCrudOperations"
RUN dotnet build "./MongoDbCrudOperations.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MongoDbCrudOperations.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MongoDbCrudOperations.dll"]
