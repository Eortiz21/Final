# ===== Etapa 1: Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Configurar zona horaria
ENV TZ=America/Guatemala
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Copiar archivos del proyecto
COPY *.sln ./
COPY Primera/*.csproj ./Primera/
RUN dotnet restore

COPY Primera/. ./Primera/
WORKDIR /app/Primera

# Compilar y publicar en Release
RUN dotnet publish -c Release -o /out

# ===== Etapa 2: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bullseye-slim AS runtime
WORKDIR /app

# Instalar soporte de locales y ICU para es-ES
RUN apt-get update && apt-get install -y \
        locales \
        icu-devtools \
    && locale-gen es_ES.UTF-8 \
    && rm -rf /var/lib/apt/lists/*

# Copiar la salida de compilaci�n
COPY --from=build /out ./

# Configurar variables de entorno
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=es_ES.UTF-8
ENV LANGUAGE=es_ES:es
ENV LC_ALL=es_ES.UTF-8

# Exponer puerto
EXPOSE 80

# Comando de inicio
ENTRYPOINT ["dotnet", "Primera.dll"]
