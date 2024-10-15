FROM mcr.microsoft.com/dotnet/runtime:8.0.10-alpine3.20-arm32v7 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/LupuServ.csproj", "src/"]
RUN dotnet restore "src/LupuServ.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "LupuServ.csproj" -c Release --no-self-contained -o /app/build

FROM build AS publish
RUN dotnet publish "LupuServ.csproj" -c Release --no-self-contained -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LupuServ.dll"]
