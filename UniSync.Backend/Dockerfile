FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY UniSync.Shared/UniSync.Shared.csproj UniSync.Shared/
COPY UniSync.Backend/UniSync.Backend.csproj UniSync.Backend/
RUN dotnet restore UniSync.Backend/UniSync.Backend.csproj

COPY UniSync.Shared/ UniSync.Shared/
COPY UniSync.Backend/ UniSync.Backend/
RUN dotnet publish UniSync.Backend/UniSync.Backend.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "UniSync.Backend.dll"]
