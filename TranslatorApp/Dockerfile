#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TranslatorApp/TranslatorApp.csproj", "TranslatorApp/"]
RUN dotnet restore "./TranslatorApp/TranslatorApp.csproj"
COPY . .
WORKDIR "/src/TranslatorApp"
RUN dotnet build "./TranslatorApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TranslatorApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final 
WORKDIR /app
USER root
RUN apt update -yq && \
apt-get install -y curl && \
curl https://packages.microsoft.com/keys/microsoft.asc | tee /etc/apt/trusted.gpg.d/microsoft.asc && \
curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list | tee /etc/apt/sources.list.d/mssql-release.list && \
apt-get update -yq && \
ACCEPT_EULA=Y apt-get install -yq mssql-tools18 unixodbc-dev
USER app
COPY --from=publish /app/publish .
COPY TranslatorApp/DbInitScripts/DbInit.sql /app/
COPY entrypoint.sh /entrypoint.sh
ENTRYPOINT bash /entrypoint.sh