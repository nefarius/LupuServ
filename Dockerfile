FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/LupuServ.csproj", "src/"]
RUN dotnet restore "src/LupuServ.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "LupuServ.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LupuServ.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LupuServ.dll"]
