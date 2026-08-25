FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderEngine.Api/OrderEngine.Api.csproj", "OrderEngine.Api/"]
COPY ["OrderEngine.Application/OrderEngine.Application.csproj", "OrderEngine.Application/"]
COPY ["OrderEngine.Domain/OrderEngine.Domain.csproj", "OrderEngine.Domain/"]
COPY ["OrderEngine.Infrastructure/OrderEngine.Infrastructure.csproj", "OrderEngine.Infrastructure/"]
COPY ["OrderEngine.Tests/OrderEngine.Tests.csproj", "OrderEngine.Tests/"]

RUN dotnet restore "OrderEngine.Api/OrderEngine.Api.csproj"

COPY . .
WORKDIR "/src/OrderEngine.Api"
RUN dotnet publish "OrderEngine.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderEngine.Api.dll"]
