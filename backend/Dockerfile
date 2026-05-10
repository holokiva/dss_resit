FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 4058

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MovieWatchlist.Api.csproj", "./"]
RUN dotnet restore "MovieWatchlist.Api.csproj"
COPY . .
RUN dotnet build "MovieWatchlist.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieWatchlist.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:4058
ENTRYPOINT ["dotnet", "MovieWatchlist.Api.dll"]
