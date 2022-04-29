#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["FatCamel.Host/FatCamel.Host.csproj", "FatCamel.Host/"]
RUN dotnet restore "FatCamel.Host/FatCamel.Host.csproj"
COPY . .
WORKDIR "/src/FatCamel.Host"
RUN dotnet build "FatCamel.Host.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "FatCamel.Host.csproj" -c Release -o /app/publish

WORKDIR /src
COPY ["Storage.Module/Storage.Module.csproj", "Storage.Module/"]
RUN dotnet restore "Storage.Module/Storage.Module.csproj"
COPY . .
WORKDIR "/src/Storage.Module"
COPY ["Storage.Module.json", "/app/build/Settings/"]
WORKDIR "/src/Storage.Module"
RUN dotnet build "Storage.Module.csproj" -c Release -o /app/build/Modules/Storage.Module
FROM build AS publish
RUN dotnet publish "Storage.Module.csproj" -c Release -o /app/publish/Modules/Storage.Module

WORKDIR /src
COPY ["TelegramFatCamel.Module/TelegramFatCamel.Module.csproj", "TelegramFatCamel.Module/"]
RUN dotnet restore "TelegramFatCamel.Module/TelegramFatCamel.Module.csproj"
COPY . .
WORKDIR "/src/TelegramFatCamel.Module"
COPY ["TelegramFatCamel.Module.json", "/app/build/Settings/"]
WORKDIR "/src/TelegramFatCamel.Module"
RUN dotnet build "TelegramFatCamel.Module.csproj" -c Release -o /app/build/Modules/TelegramFatCamel.Module
FROM build AS publish
RUN dotnet publish "TelegramFatCamel.Module.csproj" -c Release -o /app/publish/Modules/TelegramFatCamel.Module

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FatCamel.Host.dll", "--server.urls", "http://+:80;https://+:443"]