# Imagen ligera para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Instalar soporte de globalización
RUN apt-get update && apt-get install -y \
        locales \
        icu-devtools \
    && locale-gen es_ES.UTF-8 \
    && rm -rf /var/lib/apt/lists/*

# Copiar la salida de la compilación
COPY --from=build /out ./

# Configurar variables de entorno
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=es_ES.UTF-8
ENV LANGUAGE=es_ES:es
ENV LC_ALL=es_ES.UTF-8

# Exponer el puerto 80
EXPOSE 80

# Comando de inicio
ENTRYPOINT ["dotnet", "Primera.dll"]
