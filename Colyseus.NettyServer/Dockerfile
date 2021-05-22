#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Colyseus.NettyServer/Colyseus.NettyServer.csproj", "Colyseus.NettyServer/"]
RUN dotnet restore "Colyseus.NettyServer/Colyseus.NettyServer.csproj"
COPY . .
WORKDIR "/src/Colyseus.NettyServer"
RUN dotnet build "Colyseus.NettyServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Colyseus.NettyServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Colyseus.NettyServer.dll"]