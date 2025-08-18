# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5068 \
    ASPNETCORE_HTTP_PORTS=5068
EXPOSE 5068

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["site-de-receita-api.csproj", "./"]
RUN dotnet restore "site-de-receita-api.csproj"

COPY . .
RUN dotnet publish "site-de-receita-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "site-de-receita-api.dll"]
