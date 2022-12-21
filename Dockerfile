#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RealDistanceAPI/RealDistanceAPI.csproj", "RealDistanceAPI/"]
RUN dotnet restore "RealDistanceAPI/RealDistanceAPI.csproj"
COPY . .
WORKDIR "/src/RealDistanceAPI"
RUN dotnet build "RealDistanceAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealDistanceAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealDistanceAPI.dll"]
