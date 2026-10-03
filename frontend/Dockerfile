FROM mcr.microsoft.com/dotnet/sdk:10.0 AS run
WORKDIR /src

COPY *.sln .
COPY frontend/*.csproj frontend/

RUN dotnet restore

COPY . .
WORKDIR /src/frontend

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]
