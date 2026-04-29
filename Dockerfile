FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/PrivateApi/Bupa.Books.PrivateApi/Bupa.Books.PrivateApi.csproj",     "src/PrivateApi/Bupa.Books.PrivateApi/"]
COPY ["src/Shared/Bupa.Books.Application/Bupa.Books.Application.csproj",       "src/Shared/Bupa.Books.Application/"]
COPY ["src/Shared/Bupa.Books.Domain/Bupa.Books.Domain.csproj",                 "src/Shared/Bupa.Books.Domain/"]
COPY ["src/Shared/Bupa.Books.Infrastructure/Bupa.Books.Infrastructure.csproj", "src/Shared/Bupa.Books.Infrastructure/"]
COPY ["src/Shared/Bupa.Books.Common/Bupa.Books.Common.csproj",                 "src/Shared/Bupa.Books.Common/"]

RUN dotnet restore "src/PrivateApi/Bupa.Books.PrivateApi/Bupa.Books.PrivateApi.csproj"

COPY . .

WORKDIR "/src/src/PrivateApi/Bupa.Books.PrivateApi"
RUN dotnet build "Bupa.Books.PrivateApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Bupa.Books.PrivateApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Bupa.Books.PrivateApi.dll"]
