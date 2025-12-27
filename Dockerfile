FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy project files first to enable restore even when a solution file isn't present
COPY MothsOath.BlazorUI/*.csproj ./MothsOath.BlazorUI/
COPY MothsOath.Core/*.csproj ./MothsOath.Core/
COPY NuGet.config ./

# Ensure packages are restored into a container-friendly location
ENV NUGET_PACKAGES=/root/.nuget/packages

# Restore the BlazorUI project
RUN dotnet restore MothsOath.BlazorUI/MothsOath.BlazorUI.csproj

# Copy the rest of the source
COPY . .

WORKDIR /src/MothsOath.BlazorUI

RUN dotnet publish "MothsOath.BlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Copy Data files from MothsOath.Core to the publish directory
# This ensures BlueprintLoader can find the Data/Blueprints files
RUN mkdir -p /app/publish/Data/Blueprints && \
    cp -r /src/MothsOath.Core/Data/Blueprints/* /app/publish/Data/Blueprints/ || true

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remove default nginx static assets
RUN rm -rf ./*

# Copy published Blazor WebAssembly files
COPY --from=build /app/publish/wwwroot .

# Copy Data files to wwwroot so they're accessible via HTTP
COPY --from=build /app/publish/Data ./Data

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

ENTRYPOINT ["nginx", "-g", "daemon off;"]
