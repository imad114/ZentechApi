# Utiliser l'image de base pour la construction de l'application .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Définir le répertoire de travail dans l'image Docker
WORKDIR /app

# Copier les fichiers du projet vers le conteneur Docker
# Assure-toi que tu copies les fichiers .csproj et le reste du projet dans le conteneur
COPY ["ZentechAPI.csproj", "./"]

# Restaurer les dépendances
RUN dotnet restore "ZentechAPI.csproj"

# Copier tout le code source
COPY . .

# Publier l'application
RUN dotnet publish "ZentechAPI.csproj" -c Release -o /app/publish

# Créer l'image d'exécution avec l'image .NET ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5033

# Copier les fichiers publiés depuis l'étape de construction
COPY --from=build /app/publish .

# Démarrer l'application
ENTRYPOINT ["dotnet", "ZentechAPI.dll"]
