#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Teamo.Workspace.API/Teamo.Workspace.API.csproj", "Teamo.Workspace.API/"]
RUN dotnet restore "Teamo.Workspace.API/Teamo.Workspace.API.csproj"
COPY . .
WORKDIR "/src/Teamo.Workspace.API"
RUN dotnet build "Teamo.Workspace.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Teamo.Workspace.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Teamo.Workspace.API.dll"]