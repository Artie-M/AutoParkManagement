FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AutoParkManagement.App/AutoParkManagement.App.csproj", "src/AutoParkManagement.App/"]
COPY ["AutoParkManagement.Implementations/AutoParkManagement.Implementations.csproj", "AutoParkManagement.Implementations/"]
COPY ["AutoParkManagement.Common/AutoParkManagement.Common.csproj", "AutoParkManagement.Common/"]
RUN dotnet restore "src/AutoParkManagement.App/AutoParkManagement.App.csproj"
COPY . .
WORKDIR "/src/src/AutoParkManagement.App"
RUN dotnet build "AutoParkManagement.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AutoParkManagement.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutoParkManagement.App.dll"]