#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS http://*:80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["filestorage.csproj", "."]
RUN dotnet restore "filestorage.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "filestorage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "filestorage.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "filestorage.dll"]