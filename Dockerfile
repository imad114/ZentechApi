# Étape 1 : Image de base pour la construction
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Définir le répertoire de travail
WORKDIR /src

# Copier le fichier .csproj et restaurer les dépendances
COPY ["ZentechAPI.csproj", "./"]
RUN dotnet restore "./ZentechAPI.csproj"

ENV ASPNETCORE_ENVIRONMENT=Production

# Copier tout le code source dans le conteneur
COPY . .

# Publier l'application dans le répertoire de sortie
RUN dotnet publish "./ZentechAPI.csproj" -c Release -o /app/publish

# Étape 2 : Image d'exécution (plus légère, pour exécuter l'application)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Définir le répertoire de travail
WORKDIR /app

# Copier les fichiers publiés depuis l'étape de construction
COPY --from=build /app/publish .

# Exposer le port 5033 pour l'application
EXPOSE 5033

# Définir l'exécution de l'application
ENTRYPOINT ["dotnet", "ZentechAPI.dll"]
