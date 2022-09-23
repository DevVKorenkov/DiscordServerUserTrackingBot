#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["discordBotClientService/DiscordServerUserTrackingBot.DiscordBotClientService.csproj", "discordBotClientService/"]
RUN dotnet restore "discordBotClientService/DiscordServerUserTrackingBot.DiscordBotClientService.csproj"
COPY . .
WORKDIR "/src/discordBotClientService"
RUN dotnet build "DiscordServerUserTrackingBot.DiscordBotClientService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiscordServerUserTrackingBot.DiscordBotClientService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordServerUserTrackingBot.DiscordBotClientService.dll"]