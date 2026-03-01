# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["CitytripPlanner.Web/CitytripPlanner.Web.csproj", "CitytripPlanner.Web/"]
COPY ["CitytripPlanner.Features/CitytripPlanner.Features.csproj", "CitytripPlanner.Features/"]
COPY ["CitytripPlanner.Infrastructure/CitytripPlanner.Infrastructure.csproj", "CitytripPlanner.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "CitytripPlanner.Web/CitytripPlanner.Web.csproj"

# Copy all source files
COPY CitytripPlanner.Web/ CitytripPlanner.Web/
COPY CitytripPlanner.Features/ CitytripPlanner.Features/
COPY CitytripPlanner.Infrastructure/ CitytripPlanner.Infrastructure/

# Build and publish
WORKDIR /src/CitytripPlanner.Web
RUN dotnet build "CitytripPlanner.Web.csproj" -c Release -o /app/build
RUN dotnet publish "CitytripPlanner.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CitytripPlanner.Web.dll"]
