FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Для сборки .NET проектов
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копируем файл решения и восстанавливаем зависимости
COPY ["SkillsetLab.sln", "."]
COPY ["SkillsetLab.api/SkillsetLab.api.csproj", "SkillsetLab.api/"]
COPY ["SkillsetLab.Application/SkillsetLab.Application.csproj", "SkillsetLab.Application/"]
COPY ["SkillsetLab.Domain/SkillsetLab.Domain.csproj", "SkillsetLab.Domain/"]
COPY ["SkillsetLab.Infrastructure/SkillsetLab.Infrastructure.csproj", "SkillsetLab.Infrastructure/"]

RUN dotnet restore "SkillsetLab.sln"

# Копируем все файлы и билдим проекты
COPY . .
WORKDIR "/src"
RUN dotnet build "SkillsetLab.sln" -c Release -o /app/build

# Публикация .NET проектов
FROM build AS publish
RUN dotnet publish "SkillsetLab.sln" -c Release -o /app/publish /p:UseAppHost=false

# Подготовка Angular проекта с использованием Node.js 18.x
FROM node:18 AS frontend-build
WORKDIR /frontend-app

# Установка зависимостей и сборка Angular проекта
COPY ["SkillsetLab.Frontend/package.json", "SkillsetLab.Frontend/package-lock.json", "./"]
RUN npm install

COPY SkillsetLab.Frontend/. .
RUN npm run build --prod

# Финальный этап для объединения всего вместе
FROM base AS final

# Установка PostgreSQL
RUN apt-get update && apt-get install -y postgresql postgresql-contrib

USER postgres

RUN service postgresql start

USER root

WORKDIR /app

# Копируем опубликованные .NET файлы из предыдущего этапа публикации 
COPY --from=publish /app/publish .

# Копируем скомпилированный фронтенд из предыдущего этапа сборки фронтенда 
COPY --from=frontend-build /frontend-app/dist ./wwwroot

EXPOSE 5432

ENTRYPOINT service postgresql start && dotnet SkillsetLab.api.dll