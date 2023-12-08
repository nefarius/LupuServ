FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim-amd64 AS build

ARG TARGETARCH
ARG TARGETOS

RUN arch=$TARGETARCH \
    && if [ "$arch" = "amd64" ]; then arch="x64"; fi \
    && echo $TARGETOS-$arch > /tmp/rid

WORKDIR /src
COPY ["src/LupuServ.csproj", "src/"]
RUN dotnet restore "src/LupuServ.csproj" -r $(cat /tmp/rid)
COPY . .
WORKDIR "/src/src"
RUN dotnet build "LupuServ.csproj" -c Release -r $(cat /tmp/rid) --no-self-contained -o /app/build

FROM build AS publish
RUN dotnet publish "LupuServ.csproj" -c Release -r $(cat /tmp/rid) --no-self-contained -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LupuServ.dll"]
