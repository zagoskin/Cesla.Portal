FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Cesla.Portal.WebUI/Cesla.Portal.WebUI.csproj", "src/Cesla.Portal.WebUI/"]
COPY ["src/Cesla.Portal.Application/Cesla.Portal.Application.csproj", "src/Cesla.Portal.Application/"]
COPY ["src/Cesla.Portal.Domain/Cesla.Portal.Domain.csproj", "src/Cesla.Portal.Domain/"]
COPY ["src/Cesla.Portal.Infrastructure/Cesla.Portal.Infrastructure.csproj", "src/Cesla.Portal.Infrastructure/"]
RUN dotnet restore "src/Cesla.Portal.WebUI/Cesla.Portal.WebUI.csproj"
COPY . .
WORKDIR "src/Cesla.Portal.WebUI"
RUN dotnet build "./Cesla.Portal.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cesla.Portal.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["cert.pfx", "/https/cert.pfx"]
ENTRYPOINT ["dotnet", "Cesla.Portal.WebUI.dll"]

#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /app
#COPY . ./
#RUN dotnet restore "src/Cesla.Portal.WebUI/Cesla.Portal.WebUI.csproj"
#WORKDIR "src/Cesla.Portal.WebUI"
#RUN dotnet publish "Cesla.Portal.WebUI.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM nginx:alpine AS final
#COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
#COPY ["cert.pfx", "/https/cert.pfx"]

#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /app
#
## Copy everything
#COPY . ./
## Restore as distinct layers
#RUN dotnet restore
## Build and publish a release
#RUN dotnet publish -c Release -o out
#
## Build runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0
#WORKDIR /app
#COPY --from=build /app/out .
## Run this to generate it: dotnet dev-certs https -ep cert.pfx -p Test1234!
#COPY ["cert.pfx", "/https/cert.pfx"]
#ENTRYPOINT ["dotnet", "Customers.WebApp.dll"]
#