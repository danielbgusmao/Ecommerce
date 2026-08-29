FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/Ecommerce.Api/Ecommerce.Api.csproj", "src/Ecommerce.Api/"]
COPY ["src/Ecommerce.Application/Ecommerce.Application.csproj", "src/Ecommerce.Application/"]
COPY ["src/Ecommerce.Domain/Ecommerce.Domain.csproj", "src/Ecommerce.Domain/"]
COPY ["src/Ecommerce.Infrastructure/Ecommerce.Infrastructure.csproj", "src/Ecommerce.Infrastructure/"]

RUN dotnet restore "src/Ecommerce.Api/Ecommerce.Api.csproj"

COPY . .

RUN dotnet publish "src/Ecommerce.Api/Ecommerce.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]