FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.sln .
COPY photo-album.Api/*.csproj photo-album.Api/
COPY photo-album.Application/*.csproj photo-album.Application/
COPY photo-album.Domain/*.csproj photo-album.Domain/
COPY photo-album.Infrastructure/*.csproj photo-album.Infrastructure/

RUN dotnet restore

COPY . .
WORKDIR /src/photo-album.Api

RUN dotnet publish -c Release -o /src/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS run
WORKDIR /src

COPY --from=build /src/publish .

ENTRYPOINT ["dotnet", "photo-album.Api.dll"]
