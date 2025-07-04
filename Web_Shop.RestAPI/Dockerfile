#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Web_Shop.RestAPI/Web_Shop.RestAPI.csproj", "Web_Shop.RestAPI/"]
COPY ["Web_Shop.Application/Web_Shop.Application.csproj", "Web_Shop.Application/"]
COPY ["Web_Shop.Persistence/Web_Shop.Persistence.csproj", "Web_Shop.Persistence/"]
COPY ["Web_Shop.Persistence.MySQL/Web_Shop.Persistence.MySQL.csproj", "Web_Shop.Persistence.MySQL/"]
RUN dotnet restore "./Web_Shop.RestAPI/./Web_Shop.RestAPI.csproj"
COPY . .
WORKDIR "/src/Web_Shop.RestAPI"
RUN dotnet build "./Web_Shop.RestAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web_Shop.RestAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web_Shop.RestAPI.dll"]