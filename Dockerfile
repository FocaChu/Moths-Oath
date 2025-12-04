FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy project files first to enable restore even when a solution file isn't present
COPY MothsOath.UI/*.csproj ./MothsOath.UI/
COPY MothsOath.Core/*.csproj ./MothsOath.Core/
COPY NuGet.config ./

# Ensure packages are restored into a container-friendly location
ENV NUGET_PACKAGES=/root/.nuget/packages

# Restore the UI project
RUN dotnet restore MothsOath.UI/MothsOath.UI.csproj

# Copy the rest of the source
COPY . .

WORKDIR /src/MothsOath.UI

RUN dotnet publish "MothsOath.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "MothsOath.UI.dll"]
