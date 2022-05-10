FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .

RUN dotnet restore "FatCamel.Host/FatCamel.Host.csproj" --disable-parallel
RUN dotnet publish "FatCamel.Host/FatCamel.Host.csproj" -c Release -o /app/publish --no-restore

#COPY "TelegramFatCamel.Module/TelegramFatCamel.Module.json" /app/publish/Settings

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app/publish

COPY --from=build /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "FatCamel.Host.dll"]