# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["SecureFileDelivery.API/SecureFileDelivery.API.csproj", "SecureFileDelivery.API/"]
COPY ["SecureFileDelivery.Application/SecureFileDelivery.Application.csproj", "SecureFileDelivery.Application/"]
COPY ["SecureFileDelivery.Domain/SecureFileDelivery.Domain.csproj", "SecureFileDelivery.Domain/"]
COPY ["SecureFileDelivery.Infrastructure/SecureFileDelivery.Infrastructure.csproj", "SecureFileDelivery.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "SecureFileDelivery.API/SecureFileDelivery.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/SecureFileDelivery.API"
RUN dotnet build "SecureFileDelivery.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "SecureFileDelivery.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create directories for file storage
RUN mkdir -p /app/FileStorage

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "SecureFileDelivery.API.dll"]