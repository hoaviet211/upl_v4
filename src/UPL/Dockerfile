# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY UPL.sln ./
COPY src/UPL/UPL.csproj src/UPL/
RUN dotnet restore src/UPL/UPL.csproj

# Copy everything and publish
COPY . .
WORKDIR /src/src/UPL
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "UPL.dll"]

