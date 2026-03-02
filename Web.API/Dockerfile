FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Web.API/Web.API.csproj Web.API/
RUN dotnet restore Web.API/Web.API.csproj
COPY . .
WORKDIR /src/Web.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.API.dll"]