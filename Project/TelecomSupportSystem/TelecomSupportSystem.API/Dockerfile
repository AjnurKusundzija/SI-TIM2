FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["TelecomSupportSystem.API/TelecomSupportSystem.API.csproj", "TelecomSupportSystem.API/"]
COPY ["TelecomSupportSystem.BLL/TelecomSupportSystem.BLL.csproj", "TelecomSupportSystem.BLL/"]
COPY ["TelecomSupportSystem.DAL/TelecomSupportSystem.DAL.csproj", "TelecomSupportSystem.DAL/"]

RUN dotnet restore "TelecomSupportSystem.API/TelecomSupportSystem.API.csproj"

COPY . .
WORKDIR "/src/TelecomSupportSystem.API"
RUN dotnet build "TelecomSupportSystem.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TelecomSupportSystem.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelecomSupportSystem.API.dll"]