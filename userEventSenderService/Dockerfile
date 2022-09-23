#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["userEventSenderService/DiscordServerUserTrackingBot.UserEventSenderService.csproj", "userEventSenderService/"]

RUN dotnet restore "userEventSenderService/DiscordServerUserTrackingBot.UserEventSenderService.csproj"
COPY . .
WORKDIR "/src/userEventSenderService"
RUN dotnet build "DiscordServerUserTrackingBot.UserEventSenderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiscordServerUserTrackingBot.UserEventSenderService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordServerUserTrackingBot.UserEventSenderService.dll"]