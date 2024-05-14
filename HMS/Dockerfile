# Use an official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["HMS/HMS.csproj", "HMS/"]
RUN dotnet restore "HMS/HMS.csproj"
COPY . .
WORKDIR "/src/HMS"
RUN dotnet build "HMS.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "HMS.csproj" -c Release -o /app/publish

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HMS.dll"]